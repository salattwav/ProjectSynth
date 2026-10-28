using EntityStates;
using ProjectSynth.Character.Synth.Content;

namespace ProjectSynth.States
{
    public class CultureShockState : ShockState
    {
        public override void OnEnter()
        {
            overlayMaterial = SynthAssets.mat_cultureShockOverlayMain;
            stunVfxPrefab.SetDirectRef(SynthAssets.vfx_cultureShock, true);

            shockDuration = SynthValues.CultureShockDuration;
            enterSoundString = AssignRandomSoundString();
            exitSoundString = null;
            healthFractionToForceExit = 0.1f;

            base.OnEnter();
        }

        private string AssignRandomSoundString()
        {
            return SynthAssets.snd_CultureShock[UnityEngine.Random.Range(0, SynthAssets.snd_CultureShock.Length)];
        }
    }
}
