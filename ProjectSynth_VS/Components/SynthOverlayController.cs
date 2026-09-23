using ProjectSynth.Mod;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectSynth.Components
{
    public class SynthOverlayController : MonoBehaviour
    {
        private Animator animator;
        private bool initialized;
        private GameObject currentBodyObject;
        private CharacterBody body;
        private HUD hud;
        private float lastPhase;
        private bool pendingOngoing;
        
        protected SynthMetroRuntime metro;

        private void Awake()
        {
            initialized = false;

            hud = GetComponentInParent<HUD>();
            animator = GetComponent<Animator>();

            initialized = true;
        }

        private void OnDisable()
        {
            if (animator)
            {
                animator.SetBool("Ongoing", false);
                animator.SetBool("Inside", false);
            }

            if (metro)
            {
                metro.cooldownStartedThisFrame = false;
            }
        }

        private void Update()
        {
            if (!initialized) return;

            ResolveTarget();

            if (body == null || metro == null) return;

            if (metro.Ongoing && !animator.GetBool("Ongoing"))
            {
                pendingOngoing = true;
            }
            else if (!metro.Ongoing)
            {
                animator.SetBool("Ongoing", false);
                pendingOngoing = false;
            }

            animator.SetFloat("SpeedMult", metro.SpeedMult);

            SequenceLoopVisual();

            ApplyOneShotSignals();
        }

        private void ApplyOneShotSignals()
        {
            if (metro.cooldownStartedThisFrame)
            {
                animator.SetFloat("RechargeTimerSpeedMult", metro.cooldownSpeedMult);

                animator.ResetTrigger("StartTimer");
                animator.SetTrigger("StartTimer");

                metro.cooldownStartedThisFrame = false;
            }
        }

        private void SequenceLoopVisual()
        {
            float phase = metro.BeatPhase01;
            bool beatJustWrapped = phase < lastPhase;

            if (pendingOngoing && beatJustWrapped)
            {
                animator.SetBool("Ongoing", true);
                pendingOngoing = false;
                TriggerBeatAnimations(phase);
            }

            bool ongoing = animator.GetBool("Ongoing");

            if (phase < lastPhase && ongoing)
            {
                TriggerBeatAnimations(phase);
            }

            lastPhase = phase;
        }

        private void TriggerBeatAnimations(float phase)
        {
            animator.Play("animSynthCrosshairSquarePulse", 5, 0f);
            animator.Play("animSynthCrosshairIndicatorOnBeatMove", 1, 0f);
            animator.Play("animSynthCrosshairIndicatorOnBeatEnd", 6, 0f);
        }

        private void ResolveTarget()
        {
            GameObject newBodyObject = hud ? hud.targetBodyObject : null;
            if (newBodyObject == null) return;
            if (newBodyObject == currentBodyObject) return;

            currentBodyObject = newBodyObject;

            body = currentBodyObject.GetComponent<CharacterBody>();
            if (body == null)
            {
                Log.Warning($"{nameof(SynthOverlayController)}: Target has no CharacterBody");
                return;
            }

            // bind new body
            if (metro == null)
            {
                metro = body.gameObject?.GetComponent<SynthMetroRuntime>();
                if (!metro) metro = body.gameObject?.AddComponent<SynthMetroRuntime>();
            }

            animator.SetBool("Ongoing", false);
            animator.SetBool("Inside", false);
        }
    }
}