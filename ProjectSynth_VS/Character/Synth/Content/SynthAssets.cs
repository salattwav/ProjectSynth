using ProjectSynth.Components;
using ProjectSynth.Mod;
using ProjectSynth.Modules;
using ProjectSynth.States.Synth.Diva;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ProjectSynth.Character.Synth.Content
{
    public static class SynthAssets
    {
        private static AssetBundle _ab;

        #region asset refs
        public static CharacterCameraParams ccpMikuBeam;
        public static GameObject mdlSynth;
        public static GameObject synthSurvivorPod; // TODO:

        // Effects
        public static GameObject vfx_stunningPerformanceDistortion; // TODO:
        public static GameObject vfx_stunningPerformanceShock; // TODO:
        public static GameObject vfx_cultureShock;
        public static GameObject vfx_divaExplosion; // TODO:
        public static GameObject vfx_encoreExplosion;
        public static GameObject vfx_mikuBeamEffect; // TODO:
        public static GameObject vfx_tnmMuzzleFlash; // TODO:

        // Projectiles
        public static GameObject proj_ThirtyNineMusic; // TODO:
        public static GameObject proj_Diva; // TODO:

        // UI
        public static GameObject ui_synthCrosshair; // TODO:
        public static GameObject ui_synthMetroOverlay; // TODO:
        public static GameObject ui_synthAnotherOverlay; // TODO:

        // Textures
        public static Sprite icon_Metro; // TODO:
        public static Sprite icon_ThirtyNineMusic; // TODO:
        public static Sprite icon_Diva; // TODO:
        public static Sprite icon_RollingGirl; // TODO:
        public static Sprite icon_MikuBeam; // TODO:
        public static Sprite icon_EncoreBuff; // TODO:
        public static Sprite icon_WeakEndBuff; // TODO:
        public static Sprite icon_PumpedUpBuff; // TODO:
        public static Texture tex_synthPortrait; // TODO:

        // Materials
        public static Material mat_cultureShockOverlayMain; // TODO:
        public static Material mat_DivaStunningPerformanceSphere;
        public static Material mat_PumpedUpOverlayEffectMaterial;
        public static Material mat_OverdriveOverlayEffectMaterial;

        // Sound strings
        public static readonly string[] snd_CultureShock =
        {
            "Play_CultureShockStart_1",
            "Play_CultureShockStart_2",
            "Play_CultureShockStart_3",
            "Play_CultureShockStart_4",
            "Play_CultureShockStart_5",
            "Play_CultureShockStart_6",
            "Play_CultureShockStart_7"
        };

        // Sound events
        public static NetworkSoundEventDef snd_ThirtyNineMusicHitSoundEvent;
        #endregion

        public static void Init(AssetBundle assetBundle)
        {
            _ab = assetBundle;

            CreateSoundEvents();

            CreateCrosshairAndOverlay();

            RegisterTextures();
            RegisterMisc();

            CreateMaterials();

            CreateEffects();
            CreateProjectiles();
        }

        private static void CreateSoundEvents()
        {
            snd_ThirtyNineMusicHitSoundEvent = CreateSoundEvent("Play_synth_ThirtyNineMusic_shot");
        }

        private static NetworkSoundEventDef CreateSoundEvent(string eventName)
        {
            NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            networkSoundEventDef.akId = AkSoundEngine.GetIDFromString(eventName);
            networkSoundEventDef.eventName = eventName;

            ContentAddition.AddNetworkSoundEventDef(networkSoundEventDef);

            return networkSoundEventDef;
        }

        private static void CreateCrosshairAndOverlay()
        {
            ui_synthCrosshair = _ab.LoadAsset<GameObject>("SynthCrosshair");

            ui_synthMetroOverlay = _ab.LoadAsset<GameObject>("MetroOverlay");
            ui_synthMetroOverlay.AddComponent<SynthOverlayController>();

            ui_synthAnotherOverlay = _ab.LoadAsset<GameObject>("AnotherOverlay");
        }

        private static void RegisterTextures()
        {
            icon_Metro = _ab.LoadAsset<Sprite>("texSynthMetro");
            icon_ThirtyNineMusic = _ab.LoadAsset<Sprite>("texSynthTNM");
            icon_Diva = _ab.LoadAsset<Sprite>("texSynthDiva");
            icon_RollingGirl = _ab.LoadAsset<Sprite>("texSynthRollingGirl");
            icon_MikuBeam = _ab.LoadAsset<Sprite>("texSynthMikuBeam");

            tex_synthPortrait = _ab.LoadAsset<Texture>("texHenryIcon");
            icon_EncoreBuff = Addressables.LoadAssetAsync<Sprite>("RoR2/DLC3/Items/SharedSuffering/texSharedSufferingDebuffIcon.png").WaitForCompletion();
            icon_WeakEndBuff = Addressables.LoadAssetAsync<Sprite>("RoR2/DLC2/Items/TeleportOnLowHealth/texBuffTeleportOnLowHealthIcon.png").WaitForCompletion();
            icon_PumpedUpBuff = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texDroneIconOutlined.png").WaitForCompletion();
        }

        private static void RegisterMisc()
        {
            mdlSynth = _ab.LoadAsset<GameObject>("mdlSynth");

            synthSurvivorPod = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorPod.prefab").WaitForCompletion();

            ccpMikuBeam = ScriptableObject.CreateInstance<CharacterCameraParams>();
            ccpMikuBeam.name = "ccpMikuBeam";
            ccpMikuBeam.data.minPitch = -70;
            ccpMikuBeam.data.maxPitch = 70;
            ccpMikuBeam.data.wallCushion = 0.1f;
            ccpMikuBeam.data.pivotVerticalOffset = 0f;
            ccpMikuBeam.data.idealLocalCameraPos = new Vector3(2, -0.5f, -1.5f);
            ccpMikuBeam.data.fov = 95f;
        }

        private static void CreateMaterials()
        {
            foreach (var mat in _ab.LoadAllAssets<Material>())
            {
                if (!mat.shader.name.Contains("StubbedRoR2"))
                {
#if DEBUG
                    Log.Info($"Material {mat.name} should not and will not be processed.");
#endif
                    continue;
                }

                if (mat.shader.name.Contains("HGStandard")) mat.ConvertStubbedShaderToHopoo_Standart();
                else if (mat.shader.name.Contains("HGCloudRemap")) mat.ConvertStubbedShaderToHopoo_CloudRemap();
                else if (mat.shader.name.Contains("HGIntersectionCloudRemap")) mat.ConvertStubbedShaderToHopoo_Intersection();
                else if (mat.shader.name.Contains("HGOpaqueCloudRemap")) mat.ConvertStubbedShaderToHopoo_OpaqueCloudRemap();
#if DEBUG
                else Log.Warning($"Material {mat.name} does not have a recognized shader type.");
#endif
            }

            mat_DivaStunningPerformanceSphere = _ab.LoadAsset<Material>("matDivaStunningPerformanceSphere");
            mat_cultureShockOverlayMain = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/matIsShocked.mat").WaitForCompletion();
            mat_PumpedUpOverlayEffectMaterial = _ab.LoadAsset<Material>("matPumpedUpOverlayEffectMaterial");
            mat_OverdriveOverlayEffectMaterial = _ab.LoadAsset<Material>("matOverdriveOverlay");
        }

        private static void CreateEffects()
        {
            Effect_StunningPerformance();
            Effect_DivaExplosion();
            Effect_MikuBeam();
            Effect_CultureShock();
            Effect_EncoreExplosion();

            MuzzleFlash_ThirtyNineMusic();
        }

        private static void CreateProjectiles()
        {
            Projectile_ThirtyNineMusic();

            Projectile_Diva();
        }

        private static void Effect_StunningPerformance()
        {
            vfx_stunningPerformanceDistortion = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC3/Drone Tech/DroneHeadbuttImpact.prefab").WaitForCompletion();

            vfx_stunningPerformanceShock = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainTazerSupplyDropNova.prefab").WaitForCompletion();
            vfx_stunningPerformanceShock.transform.Find("Nova Sphere").GetComponent<ParticleSystemRenderer>().material = mat_DivaStunningPerformanceSphere;
        }

        private static void Effect_DivaExplosion()
        {
            vfx_divaExplosion = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Mage/MageLightningBombExplosion.prefab").WaitForCompletion();
        }

        private static void Effect_MikuBeam()
        {
            vfx_mikuBeamEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorBeamCorrupt.prefab").WaitForCompletion();
        }

        private static void Effect_CultureShock()
        {
            vfx_cultureShock = _ab.LoadAsset<GameObject>("CultureShock");
            var cshock_psDration = vfx_cultureShock.AddComponent<ScaleParticleSystemDuration>();
            cshock_psDration.initialDuration = 2f;
            cshock_psDration.particleSystems = vfx_cultureShock.GetComponentsInChildren<ParticleSystem>();

            var cshock_vfxAttributes = vfx_cultureShock.AddComponent<VFXAttributes>();
            cshock_vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
            cshock_vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;

            vfx_cultureShock.AddComponent<HoverOverHead>();
            vfx_cultureShock.AddComponent<Rigidbody>().isKinematic = true;

            vfx_cultureShock.transform.Find("Bonk").gameObject.AddComponent<HoverOverHead>();
            ContentAddition.AddEffect(vfx_cultureShock);
        }

        private static void Effect_EncoreExplosion()
        {
            vfx_encoreExplosion = _ab.LoadAsset<GameObject>("EncoreExplosion");
            var encoreExplosion_effectComponent = vfx_encoreExplosion.AddComponent<EffectComponent>();
            encoreExplosion_effectComponent.soundName = "";
            encoreExplosion_effectComponent.applyScale = true;

            var encoreExplosion_vfxAttributes = vfx_encoreExplosion.AddComponent<VFXAttributes>();
            encoreExplosion_vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Medium;
            encoreExplosion_vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;

            var encoreExplosion_particleEnd = vfx_encoreExplosion.AddComponent<DestroyOnParticleEnd>();
            encoreExplosion_particleEnd.trackedParticleSystem = vfx_encoreExplosion.GetComponentInChildren<ParticleSystem>();
            ContentAddition.AddEffect(vfx_encoreExplosion);
        }

        private static void MuzzleFlash_ThirtyNineMusic()
        {
            vfx_tnmMuzzleFlash = _ab.LoadAsset<GameObject>("SynthTNMMuzzleFlash");
            var tnmMuzzleFlash_vfxAttributes = vfx_tnmMuzzleFlash.AddComponent<VFXAttributes>();
            tnmMuzzleFlash_vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Medium;
            tnmMuzzleFlash_vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;

            var tnmMuzzleFlash_effectComponent = vfx_tnmMuzzleFlash.AddComponent<EffectComponent>();
            tnmMuzzleFlash_effectComponent.soundName = "";
            tnmMuzzleFlash_effectComponent.positionAtReferencedTransform = true;
            tnmMuzzleFlash_effectComponent.parentToReferencedTransform = true;

            var tnmMuzzleFlash_destroyOnTimer = vfx_tnmMuzzleFlash.AddComponent<DestroyOnTimer>();
            tnmMuzzleFlash_destroyOnTimer.duration = 1.0f;
            ContentAddition.AddEffect(vfx_tnmMuzzleFlash);
        }

        private static void Projectile_ThirtyNineMusic()
        {
            proj_ThirtyNineMusic = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/SyringeProjectile.prefab")
                .WaitForCompletion()?
                .InstantiateClone("ThirtyNineMusicProjectile", true);
            proj_ThirtyNineMusic.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            var tnm_simple = proj_ThirtyNineMusic.GetComponent<ProjectileSimple>();
            tnm_simple.desiredForwardSpeed = 140f;
            tnm_simple.lifetime = 4f;
            tnm_simple.lifetimeExpiredEffect = null;

            var tnm_targetImpact = proj_ThirtyNineMusic.GetComponent<ProjectileSingleTargetImpact>();
            tnm_targetImpact.impactEffect = null;
            tnm_targetImpact.hitSound = snd_ThirtyNineMusicHitSoundEvent;

            var tnm_ghost = _ab.LoadAsset<GameObject>("ThirtyNineMusicGhost").InstantiateClone("ThirtyNineMusicGhost", true);
            tnm_ghost.AddComponent<VFXAttributes>();
            var tnm_ghostController = tnm_ghost.AddComponent<ProjectileGhostController>();
            tnm_ghostController.inheritScaleFromProjectile = true;

            var tnm_controller = proj_ThirtyNineMusic.GetComponent<ProjectileController>();
            tnm_controller.ghostPrefabAddress = null;
            tnm_controller.ghostPrefab = tnm_ghost;

            ContentAddition.AddProjectile(proj_ThirtyNineMusic);
        }
        private static void Projectile_Diva()
        {
            proj_Diva = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiMine.prefab")
                .WaitForCompletion()?
                .InstantiateClone("DivaProjectile", true);

            proj_Diva.DestroyChild("Ring");
            proj_Diva.DestroyChild("PrepEffect");
            proj_Diva.DestroyChild("WeakIndicator");
            proj_Diva.DestroyChild("StrongIndicator");

            UnityEngine.Object.DestroyImmediate(proj_Diva.GetComponent<ProjectileDeployToOwner>());
            UnityEngine.Object.DestroyImmediate(proj_Diva.GetComponent<Deployable>());
            UnityEngine.Object.DestroyImmediate(proj_Diva.GetComponent<ProjectileStickOnImpact>());
            foreach (var comp in proj_Diva.GetComponents<EntityStateMachine>())
            {
                UnityEngine.Object.DestroyImmediate(comp);
            }

            var divaVisuals = _ab.LoadAsset<GameObject>("DivaVisuals").InstantiateClone("DivaVisuals", true);
            Transform diva_sphere = divaVisuals.transform.Find("Hologram/Sphere");
            Transform diva_blink = divaVisuals.transform.Find("Blink");

            divaVisuals.transform.SetParent(proj_Diva.transform, false);

            var diva_pulse = proj_Diva.AddComponent<ParticlePulseSync>();
            diva_pulse.particleSystem = diva_blink.GetComponent<ParticleSystem>();

            var diva_esm1 = proj_Diva.AddComponent<EntityStateMachine>();
            diva_esm1.initialStateType = new EntityStates.SerializableEntityStateType(typeof(DivaArmingUnarmed));
            diva_esm1.mainStateType = new EntityStates.SerializableEntityStateType(typeof(DivaArmingUnarmed));
            diva_esm1.customName = "Arming";

            var diva_esm2 = proj_Diva.AddComponent<EntityStateMachine>();
            diva_esm2.initialStateType = new EntityStates.SerializableEntityStateType(typeof(WaitForStick));
            diva_esm2.mainStateType = new EntityStates.SerializableEntityStateType(typeof(StunningPerformance));
            diva_esm2.customName = "Main";

            var diva_networkEsm = proj_Diva.GetComponent<NetworkStateMachine>();
            diva_networkEsm.stateMachines = [diva_esm1, diva_esm2];

            proj_Diva.AddComponent<DivaAnimator>();

            var diva_stick = proj_Diva.AddComponent<ProjectileStickOnImpactByNormal>();
            diva_stick.minGroundNormalY = 0.65f;
            diva_stick.ignoreCharacters = true;
            diva_stick.ignoreWorld = false;
            diva_stick.alignNormals = true;

            var diva_target = proj_Diva.GetComponent<ProjectileSphereTargetFinder>();
            diva_target.lookRange = 14f;
            diva_sphere.localScale *= diva_target.lookRange * 2f;

            var diva_lifetime = proj_Diva.AddComponent<DivaLifetime>();
            diva_lifetime.flyingLifetime = 5f;
            diva_lifetime.stuckLifetime = 10f;

            var diva_ghost = _ab.LoadAsset<GameObject>("DivaGhost")?.InstantiateClone("DivaProjectileGhost", true);
            diva_ghost.AddComponent<ProjectileGhostController>();
            diva_ghost.AddComponent<VFXAttributes>().DoNotPool = true;

            var diva_controller = proj_Diva.GetComponent<ProjectileController>();
            diva_controller.ghostPrefabAddress = null;
            diva_controller.ghostPrefab = diva_ghost;

            ContentAddition.AddProjectile(proj_Diva);
        }
    }
}
