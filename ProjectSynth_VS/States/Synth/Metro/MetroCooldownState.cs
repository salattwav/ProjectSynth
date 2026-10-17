using ProjectSynth.Character.Synth.Content;
using RoR2;
using SYNClib.API;
using UnityEngine;

namespace ProjectSynth.States.Synth.Metro
{
    public sealed class MetroCooldownState(float overdiveMeterValue) : BaseMetroState
    {
        private long _enterBeatIndex;
        private readonly int _cooldownBeats = Mathf.Max(1, (int)overdiveMeterValue);
        private readonly float _maxFadeDuration = 2.0f;
        private readonly float _minFadeDuration = 0.35f;

        public override void OnEnter()
        {
            base.OnEnter();
            _enterBeatIndex = _metro.BeatIndex;

            float speed = (_metro.SpeedMult > 0f) ? _metro.SpeedMult : 2f;
            float cooldownSpeedMult = speed / _cooldownBeats;

            _metro.cooldownSpeedMult = cooldownSpeedMult;
            _metro.cooldownStartedThisFrame = true;

            float secondsPerBeat = 60 / (float)Sync.BPM;
            float fadeDuration = Mathf.Clamp(_cooldownBeats * (secondsPerBeat * 0.5f), _minFadeDuration, _maxFadeDuration);
            shaderOverlay.SetActive(overdriveOverlayMaterial, false, fadeDuration);
        }

        public override void Update()
        {
            base.Update();
            if (_metro.BeatIndex - _enterBeatIndex >= _cooldownBeats)
            {
                outer.SetNextState(new MetroWaitForInputState());
            }
        }
    }
}