using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ProjectSynth.Character.Synth.Content
{
    public class SynthPassive
    {
        public static ItemDef MetroPassiveItem { get; protected set; }

        public static void Initialize()
        {
            var displayRules = new ItemDisplayRuleDict(null);
            var pickupModel = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mystery/PickupMystery.prefab").WaitForCompletion();
            var pickupIcon = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();

            MetroPassiveItem = ScriptableObject.CreateInstance<ItemDef>();
            MetroPassiveItem.name = "SynthMetroPassiveItem";
            MetroPassiveItem.deprecatedTier = ItemTier.NoTier;
            MetroPassiveItem._itemTierDef = null;
            MetroPassiveItem.nameToken = SynthSurvivor.SYNTH_PREFIX + "PASSIVE_ITEM_METRO_NAME";
            MetroPassiveItem.pickupToken = SynthSurvivor.SYNTH_PREFIX + "PASSIVE_ITEM_METRO_PICKUP";
            MetroPassiveItem.descriptionToken = SynthSurvivor.SYNTH_PREFIX + "PASSIVE_ITEM_METRO_DESCRIPTION";
            MetroPassiveItem.loreToken = SynthSurvivor.SYNTH_PREFIX + "PASSIVE_ITEM_METRO_LORE";
            MetroPassiveItem.unlockableDef = null;
            MetroPassiveItem.pickupModelPrefab = pickupModel;
            MetroPassiveItem.pickupIconSprite = pickupIcon;
            MetroPassiveItem.isConsumed = false;
            MetroPassiveItem.hidden = true;
            MetroPassiveItem.canRemove = false;
            ItemAPI.Add(new CustomItem(MetroPassiveItem, displayRules));
        }
    }
}
