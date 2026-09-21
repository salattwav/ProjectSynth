using ProjectSynth.Character.Synth.Content;

namespace ProjectSynth.States.Synth.Metro
{
    public sealed class MetroCooldownState : BaseMetroState
    {
        private readonly int CooldownBeats = SynthValues.MetroSuccessfulHitCooldownInBeats;
        private readonly float AnimEarlyOffsetBeats = 0.30f; // TODO: what even is that??
        private long startBeatIndex;

        public override void OnEnter()
        {
            base.OnEnter();
            IsOnCooldown = true;

            startBeatIndex = metro.BeatIndex;

            float speed = (metro.SpeedMult > 0f) ? metro.SpeedMult : 2f;
            float animBeats = CooldownBeats + AnimEarlyOffsetBeats;
            metro.cooldownSpeedMult = speed / animBeats;
            metro.cooldownStartedThisFrame = true;
        }

        public override void Update()
        {
            base.Update();
            if (metro.BeatIndex - startBeatIndex >= CooldownBeats)
            {
                outer.SetNextState(new MetroWaitForInputState());
            }
        }
    }
}