using EntityStates;
using ProjectSynth.Character.Synth.Content;
using R2API;
using RoR2;
using SYNClib.API;
using UnityEngine;
using UnityEngine.Networking;

namespace ProjectSynth.States.Synth.Weapon
{
    public class MikuBeam : BaseSkillState
    {
        private enum Phase { Leap, Sustain }

        // Leap params
        public float upwardSpeed = SynthValues.MikuBeamLeapUpwardSpeed;
        public float forwardSpeed = SynthValues.MikuBeamLeapForwardSpeed;

        // Sustain params
        public GameObject beamVfxPrefab = SynthAssets.vfx_mikuBeamEffect;
        public double sustainDuration = Sync.BeatInterval * SynthValues.MikuBeamSustainDurationInBeats;
        public float maxDistance = SynthValues.MikuBeamLength;
        public float damageCoefficientPerSeconds = SynthValues.MikuBeamDamageCoefficient;
        public float TickRate => 24f - (float)sustainDuration;

        private Phase phase;
        private CameraTargetParams.CameraParamsOverrideHandle cameraParamsOverrideHandle;

        // Leap state
        private Vector3 worldLeapVector;
        private bool leapProcessing;
        private double peakTime;
        private float leapStopwatch;

        // Sustain state
        private GameObject beamVfxInstance;
        private double sustainDurationAdjusted;
        private float sustainAge;
        private float beamTickStopwatch;
        private bool hasBegunBeaming;

        public override void OnEnter()
        {
            base.OnEnter();
            phase = Phase.Leap;

            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.Slow80);
                base.characterBody.AddBuff(RoR2Content.Buffs.ElephantArmorBoost);
            }

            leapProcessing = false;
            peakTime = Sync.BeatInterval * 4f;

            if (!base.characterMotor.isGrounded)
            {
                upwardSpeed *= 0.6f;
            }
            base.characterMotor.Motor.ForceUnground();

            Vector3 direction = base.GetAimRay().direction;
            direction.y = 0f;
            direction.Normalize();

            worldLeapVector = Matrix4x4.TRS(
                base.transform.position,
                Util.QuaternionSafeLookRotation(direction, Vector3.up),
                Vector3.one
            ).MultiplyPoint3x4(new Vector3(0f, upwardSpeed, forwardSpeed)) - base.transform.position;
        }

        public override void Update()
        {
            base.Update();
            if (phase != Phase.Leap) return;

            if (Sync.OnBeat() && !leapProcessing)
            {
                leapProcessing = true;
                StartHoverParamsOverride((float)peakTime);
            }

            if (leapStopwatch >= peakTime - Sync.BeatInterval * 0.5f && Sync.OnBeat())
            {
                BeginSustain();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            switch (phase)
            {
                case Phase.Leap:
                    FixedUpdateLeap();
                    break;
                case Phase.Sustain:
                    FixedUpdateSustain();
                    break;
            }
        }

        private void FixedUpdateLeap()
        {
            if (!leapProcessing) return;

            leapStopwatch += base.GetDeltaTime();
            float t = leapStopwatch / (float)peakTime;
            float derivative = 3f * Mathf.Pow(1f - t, 2f);

            base.characterMotor.velocity = new Vector3(base.characterMotor.velocity.x, 0f, base.characterMotor.velocity.z);
            base.characterMotor.rootMotion += worldLeapVector * derivative * base.GetDeltaTime();
        }

        private void BeginSustain()
        {
            phase = Phase.Sustain;
            sustainDurationAdjusted = sustainDuration / this.attackSpeedStat;
            base.characterBody.SetAimTimer((float)sustainDurationAdjusted + 1f);
        }

        private void FixedUpdateSustain()
        {
            if (!hasBegunBeaming)
            {
                hasBegunBeaming = true;
                beamVfxInstance = UnityEngine.Object.Instantiate(beamVfxPrefab);
                beamVfxInstance.transform.SetParent(this.characterBody.aimOriginTransform, false);
                FireBeam();
            }

            if (beamVfxInstance)
            {
                Vector3 point = base.GetAimRay().GetPoint(maxDistance);
                if (Util.CharacterRaycast(base.gameObject, base.GetAimRay(), out RaycastHit raycastHit, maxDistance, LayerIndex.world.mask, QueryTriggerInteraction.UseGlobal))
                {
                    point = raycastHit.point;
                }
                beamVfxInstance.transform.forward = point - beamVfxInstance.transform.position;
            }

            beamTickStopwatch += Time.deltaTime;
            float beamCountdown = 1f / TickRate / this.attackSpeedStat;
            if (beamTickStopwatch > beamCountdown)
            {
                beamTickStopwatch -= beamCountdown;
                FireBeam();
            }

            if (base.isAuthority && base.characterMotor.velocity.y <= 0)
            {
                float num = base.characterMotor.velocity.y;
                num = Mathf.MoveTowards(num, -1, 60f * base.GetDeltaTime());
                base.characterMotor.velocity = new Vector3(base.characterMotor.velocity.x, num, base.characterMotor.velocity.z);
            }

            sustainAge += base.GetDeltaTime();
            if (sustainAge >= sustainDurationAdjusted && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        private void FireBeam()
        {
            Ray aimRay = base.GetAimRay();
            if (base.isAuthority)
            {
                BulletAttack ba = new()
                {
                    owner = gameObject,
                    weapon = gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = 0f,
                    damage = damageCoefficientPerSeconds * this.damageStat / TickRate,
                    force = 20f,
                    muzzleName = "SwingCenter",
                    isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
                    radius = 1.5f,
                    falloffModel = BulletAttack.FalloffModel.None,
                    stopperMask = LayerIndex.world.mask,
                    procCoefficient = 1f,
                    maxDistance = this.maxDistance,
                    smartCollision = true,
                    damageType = DamageType.Generic,
                    allowTrajectoryAimAssist = false
                };
                ba.damageType.damageSource = DamageSource.Special;

                if (ba.isCrit)
                {
                    ba.damageType.AddModdedDamageType(SynthDamageTypes.Encore);
                }

                ba.Fire();
            }
        }

        private void StartHoverParamsOverride(float transitionDuration)
        {
            if (cameraParamsOverrideHandle.isValid) return;

            cameraParamsOverrideHandle = base.cameraTargetParams.AddParamsOverride(new CameraTargetParams.CameraParamsOverrideRequest
            {
                cameraParamsData = SynthAssets.ccpMikuBeam.data,
                priority = 1.0f
            }, transitionDuration);
        }

        private void EndHoverParamsOverride(float transitionDuration)
        {
            if (cameraParamsOverrideHandle.isValid)
            {
                base.cameraTargetParams.RemoveParamsOverride(cameraParamsOverrideHandle, transitionDuration);
                cameraParamsOverrideHandle = default;
            }
        }

        public override void OnExit()
        {
            EndHoverParamsOverride(1.0f);

            if (beamVfxInstance)
            {
                VfxKillBehavior.KillVfxObject(beamVfxInstance);
            }

            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.Slow80);
                base.characterBody.RemoveBuff(RoR2Content.Buffs.ElephantArmorBoost);
            }

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}