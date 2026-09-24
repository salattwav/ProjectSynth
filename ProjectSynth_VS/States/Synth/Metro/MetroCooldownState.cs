using ProjectSynth.Character.Synth.Content;

namespace ProjectSynth.States.Synth.Metro
{
    public sealed class MetroCooldownState(float overdiveMeterValue) : BaseMetroState
    {
        private float _overdriveScaledTime;
        private readonly float _overdiveMeterValue = overdiveMeterValue;

        public override void OnEnter()
        {
            base.OnEnter();

            _overdriveScaledTime = _overdiveMeterValue * SynthValues.MetroCooldownOverdriveInfluenceCoefficient;

            float speed = (_metro.SpeedMult > 0f) ? _metro.SpeedMult : 2f;
            float cooldownSpeed = speed / _overdriveScaledTime;

            _metro.cooldownSpeedMult = cooldownSpeed;
            _metro.cooldownStartedThisFrame = true;

            shaderOverlay.SetActive(overdriveOverlayMaterial, false, cooldownSpeed);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= _overdriveScaledTime)
            {
                outer.SetNextState(new MetroWaitForInputState());
            }
        }
    }
}