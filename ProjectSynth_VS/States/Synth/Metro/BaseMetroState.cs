using EntityStates;
using ProjectSynth.Components;

namespace ProjectSynth.States.Synth.Metro
{
    public abstract class BaseMetroState : BaseState
    {
        protected SynthMetroRuntime metro;
        public bool IsOnCooldown { get; protected set; }

        public override void OnEnter()
        {
            base.OnEnter();

            metro = gameObject.GetComponent<SynthMetroRuntime>();
        }

        public bool IsInTimingWindow => metro != null && metro.TimingWindowOpen;

        public float OverdriveMeter => metro != null ? metro.OverdriveMeter : 0f;

        public void IncreaseOverdriveMeter()
        {
            metro.IncreaseOverdriveMeter();
        }

        public OverdriveLevel GetOverdriveLevel()
        {
            return metro.GetOverdriveLevel();
        }

        public void EnterCooldownState()
        {
            outer.SetNextState(new MetroCooldownState());
        }

        public void EnterMissedState()
        {
            outer.SetNextState(new MetroMissedState());
        }
    }
}