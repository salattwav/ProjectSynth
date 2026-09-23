using ProjectSynth.Character.Synth.Content;

namespace ProjectSynth.States.Synth.Metro
{
    public sealed class MetroCooldownState(float overdiveMeterValue) : BaseMetroState
    {
        private float _cooldownTime;
        private readonly float _overdiveMeterValue = overdiveMeterValue;

        public override void OnEnter()
        {
            base.OnEnter();

            _cooldownTime = _overdiveMeterValue * SynthValues.MetroCooldownOverdriveInfluenceCoefficient;

            float speed = (_metro.SpeedMult > 0f) ? _metro.SpeedMult : 2f;
            _metro.cooldownSpeedMult = speed / _cooldownTime;
            _metro.cooldownStartedThisFrame = true;

            shaderOverlay.SetActive(overdriveOverlayMaterial, false, _cooldownTime);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= _cooldownTime)
            {
                outer.SetNextState(new MetroWaitForInputState());
            }
        }
    }
}