using ProjectSynth.Character.Synth.Content;
using RoR2;

namespace ProjectSynth.States.Synth.Metro
{
    public sealed class MetroCooldownState(float overdiveMeterValue) : BaseMetroState
    {
        private readonly int _cooldownBeats = (int)overdiveMeterValue;
        private long enterBeatIndex;

        public override void OnEnter()
        {
            base.OnEnter();
            enterBeatIndex = _metro.BeatIndex;

            float speed = (_metro.SpeedMult > 0f) ? _metro.SpeedMult : 2f;
            float cooldownSpeedMult = speed / _cooldownBeats;

            _metro.cooldownSpeedMult = cooldownSpeedMult;
            _metro.cooldownStartedThisFrame = true;

            shaderOverlay.SetActive(overdriveOverlayMaterial, false, cooldownSpeedMult);
        }

        public override void Update()
        {
            base.Update();
            if (_metro.BeatIndex - enterBeatIndex >= _cooldownBeats)
            {
                Chat.AddMessage("entering next state...");
                outer.SetNextState(new MetroWaitForInputState());
            }
        }
    }
}