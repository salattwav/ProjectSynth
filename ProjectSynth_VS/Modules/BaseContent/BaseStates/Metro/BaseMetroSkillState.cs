using EntityStates;
using ProjectSynth.Character.Synth.Content;
using ProjectSynth.States.Synth.Metro;
using RoR2;

namespace ProjectSynth.Modules.BaseContent.BaseStates.Metro
{
    public abstract class BaseMetroSkillState : BaseSkillState
    {
        public virtual string MetroEsmName => "Metro";
        public virtual bool UseMetronome => true;

        public override void OnEnter()
        {
            base.OnEnter();
            TryHandleMetronomeWindow();
        }

        private void TryHandleMetronomeWindow()
        {
            if (!UseMetronome || !characterBody) return;
            if (!SynthPassive.IsMetro(characterBody)) return;

            var esm = EntityStateMachine.FindByCustomName(characterBody.gameObject, MetroEsmName);
            if (!esm) return;

            if (esm.state is not MetroWaitForInputState readyState) return;

            bool hit = readyState.RegisterHit();

            if (hit) OnMetronomeHit(readyState);
            else OnMetronomeMiss(readyState);
        }

        public virtual void OnMetronomeHit(BaseMetroState metroState) { }
        public virtual void OnMetronomeMiss(BaseMetroState metroState) { }
    }
}