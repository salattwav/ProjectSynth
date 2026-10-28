using ProjectSynth.Character.Synth.Content;
using RoR2;
using SYNClib.API;
using UnityEngine;

namespace ProjectSynth.Components
{
    public enum OverdriveLevel
    {
        None = 0,
        Practice = 1,
        Rehearsal = 2,
        Live = 3,
        Headliner = 4,
        RockStar = 5
    }

    public sealed class SynthMetroRuntime : MonoBehaviour
    {
        public float SpeedMult { get; private set; } = 1f;
        public long BeatIndex { get; private set; }
        public float BeatPhase01 { get; private set; }
        public bool SuccessWindowOpen { get; private set; }
        public float OverdriveMeter { get; private set; }
        public bool Ongoing { get; private set; }

        public bool cooldownStartedThisFrame;
        public float cooldownSpeedMult = 1f;

        private float _clock;
        private double _lastBeatTime = -1.0;
        private OverdriveLevel _level;
        private double _addToMeter;
        private readonly int _overdriveMaxLevel = 5;


        private void Awake()
        {
            if (SpeedMult <= 0f) SpeedMult = 1f;
        }

        private void Update()
        {
            _clock += Time.deltaTime;

            if (Sync.OnEntry())
            {
                ResetAll();
                return;
            }

            if (Sync.OnBeat())
            {
                _lastBeatTime = _clock;
                BeatIndex = Sync.BeatIndex;
            }

            ComputePhaseAndWindow();
            Ongoing = Sync.BeatInterval > 0.0;

            _level = (OverdriveLevel)OverdriveMeter;
        }

        private void ComputePhaseAndWindow()
        {
            SuccessWindowOpen = false;

            double interval = Sync.BeatInterval;

            if (interval <= 0.0 || _lastBeatTime < 0.0)
            {
                BeatPhase01 = 0f;
                return;
            }

            double t = _clock - _lastBeatTime;

            float phase = (float)(t / interval);
            phase -= Mathf.Floor(phase);
            BeatPhase01 = phase;

            double distanceFromBeat = Mathf.Min(phase, 1f - phase);
            double successWindow = Sync.BPM * SynthValues.MetroSuccessWindowBPMInfluenceCoefficient;
            _addToMeter = successWindow - distanceFromBeat;

            if (distanceFromBeat <= successWindow)
            {
                SuccessWindowOpen = true;
            }

            SpeedMult = (float)(1.0 / interval);
        }

        private void ResetAll()
        {
            _clock = 0f;

            _lastBeatTime = -1.0;

            SpeedMult = 1f;
            BeatIndex = 0L;

            BeatPhase01 = 0f;
            SuccessWindowOpen = false;
            _level = 0;

            ResetOverdriveMeter();

            Ongoing = false;

            cooldownStartedThisFrame = false;
            cooldownSpeedMult = 1f;
        }

        public void IncreaseOverdriveMeter()
        {
            OverdriveMeter = Mathf.Min(OverdriveMeter + ((float)_addToMeter * SynthValues.MetroOverdriveGrowthCoefficient), _overdriveMaxLevel);
        }

        public void ResetOverdriveMeter()
        {
            OverdriveMeter = 0f;
        }

        public OverdriveLevel GetOverdriveLevel()
        {
            return _level;
        }
    }
}
