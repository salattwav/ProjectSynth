using EntityStates;
using ProjectSynth.Components;
using RoR2;

namespace ProjectSynth.States.Synth.Metro
{
    public abstract class BaseMetroState : BaseState
    {
        public bool IsInSuccessWindow => _metro != null && _metro.SuccessWindowOpen;
        public float OverdriveMeter => _metro != null ? _metro.OverdriveMeter : 0f;

        protected SynthMetroRuntime _metro;

        private bool _windowHit;
        private bool _wasInWindow;
        private bool _wasMeterReset = true;
        private int _skippedBeatsCount;

        public override void OnEnter()
        {
            base.OnEnter();
            _metro = gameObject.GetComponent<SynthMetroRuntime>();
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