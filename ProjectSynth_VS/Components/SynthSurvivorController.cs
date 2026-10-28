using ProjectSynth.Character.Synth.Content;
using ProjectSynth.Mod;
using RoR2;
using RoR2.HudOverlay;
using System.Collections;
using UnityEngine;

namespace ProjectSynth.Components
{
    public class SynthSurvivorController : MonoBehaviour
    {
        public GameObject OverlayPrefab = SynthAssets.ui_synthMetroOverlay;
        public string childLocatorEntry = "CrosshairExtras";

        private OverlayController overlayController;
        private CharacterBody characterBody;
        private EntityStateMachine bodyStateMachine;
        private EntityStateMachine weaponStateMachine;
        private SkillLocator skillLocator;

        void OnEnable()
        {
            OverlayCreationParams overlayParams = new()
            {
                prefab = OverlayPrefab,
                childLocatorEntry = childLocatorEntry
            };

            overlayController = HudOverlayManager.AddOverlay(gameObject, overlayParams);
            //overlayController.onInstanceAdded += OnOverlayInstanceAdded;
            //overlayController.onInstanceRemove += OnOverlayInstanceRemoved;

            characterBody = gameObject.GetComponent<CharacterBody>();
            bodyStateMachine = EntityStateMachine.FindByCustomName(gameObject, "Body");
            weaponStateMachine = EntityStateMachine.FindByCustomName(gameObject, "Weapon");
            skillLocator = gameObject.GetComponent<SkillLocator>();
        }

        void OnDisable()
        {
            if (overlayController != null)
            {
                //overlayController.onInstanceAdded -= OnOverlayInstanceAdded;
                //overlayController.onInstanceRemove -= OnOverlayInstanceRemoved;
                HudOverlayManager.RemoveOverlay(overlayController);
            }
        }

        void FixedUpdate()
        {
        }

        private void OnOverlayInstanceAdded(OverlayController controller, GameObject instance)
        {

        }

        private void OnOverlayInstanceRemoved(OverlayController controller, GameObject instance)
        {

        }
    }
}
