using EntityStates;
using ProjectSynth.Character.Synth.Content;
using ProjectSynth.Components;
using ProjectSynth.Mod;
using RoR2;
using UnityEngine;

namespace ProjectSynth.States.Synth.Metro
{
    public abstract class BaseMetroState : BaseState
    {
        public bool IsInSuccessWindow => _metro != null && _metro.SuccessWindowOpen;
        public float OverdriveMeter => _metro != null ? _metro.OverdriveMeter : 0f;
        public Material overdriveOverlayMaterial = SynthAssets.mat_OverdriveOverlayEffectMaterial;

        protected SynthMetroRuntime _metro;
        protected CharacterShaderOverlay shaderOverlay;

        private bool _windowHit;
        private bool _wasInWindow;
        private bool _wasMeterReset = true;
        private int _skippedBeatsCount;

        public override void OnEnter()
        {
            base.OnEnter();
            _metro = gameObject.GetComponent<SynthMetroRuntime>();

            shaderOverlay = characterBody.GetComponent<CharacterShaderOverlay>();
            if (shaderOverlay == null)
            {
                Log.Warning("Shader overlay component not found on character.");
            }
        }

        public override void Update()
        {
            base.Update();

            bool inWindow = IsInSuccessWindow;

            if (inWindow && !_wasInWindow)
            {
                _windowHit = false; // fresh window opened
            }
            else if (!inWindow && _wasInWindow && !_windowHit)
            {
                HandleMissedWindow();
            }

            _wasInWindow = inWindow;
        }

        public bool RegisterHit()
        {
            if (!IsInSuccessWindow || _windowHit) return false;

            _windowHit = true;
            _skippedBeatsCount = 0;
            _metro.IncreaseOverdriveMeter();
            _wasMeterReset = false;

            shaderOverlay.SetIntensityFromValue(overdriveOverlayMaterial, OverdriveMeter, 0, 5, 0.5f);

            return true;
        }

        private void HandleMissedWindow()
        {
            _skippedBeatsCount++;
            if (_skippedBeatsCount >= 5 && !_wasMeterReset)
            {
                float cachedMeter = OverdriveMeter;
                _wasMeterReset = true;
                _metro.ResetOverdriveMeter();
                outer.SetNextState(new MetroCooldownState(cachedMeter));
            }
        }
    }
}