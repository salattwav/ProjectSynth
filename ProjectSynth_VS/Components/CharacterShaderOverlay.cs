using System.Collections.Generic;
using RoR2;
using UnityEngine;

namespace ProjectSynth.Components
{
    public class CharacterShaderOverlay : MonoBehaviour
    {
        private class Entry
        {
            public Material material;
            public float target;
            public float current;
            public float velocity;
            public float blendTime;
            public bool registered;
        }

        private readonly List<Entry> entries = [];
        private CameraRigController cameraRigController;
        private SceneCamShaderEffect sceneEffect;

        private void ResolveTargets()
        {
            if (cameraRigController == null)
            {
                var body = GetComponent<CharacterBody>();
                foreach (var crc in CameraRigController.readOnlyInstancesList)
                {
                    if (crc.targetBody == body) { cameraRigController = crc; break; }
                }
            }

            if (cameraRigController != null && sceneEffect == null && cameraRigController.sceneCam != null)
            {
                sceneEffect = cameraRigController.sceneCam.GetComponent<SceneCamShaderEffect>();
                if (sceneEffect == null)
                    sceneEffect = cameraRigController.sceneCam.gameObject.AddComponent<SceneCamShaderEffect>();
            }
        }

        private Entry FindOrCreateEntry(Material material)
        {
            for (int i = 0; i < entries.Count; i++)
                if (entries[i].material == material) return entries[i];

            var entry = new Entry { material = material };
            entries.Add(entry);
            return entry;
        }

        public void SetActive(Material material, bool active, float duration = 0.2f)
        {
            ResolveTargets();
            var entry = FindOrCreateEntry(material);
            entry.target = active ? 1f : 0f;
            entry.blendTime = Mathf.Max(duration, 0.0001f);
        }

        public void SetIntensityFromValue(Material material, float value, float minValue, float maxValue, float smoothTime = 0.2f)
        {
            ResolveTargets();
            var entry = FindOrCreateEntry(material);
            entry.target = Mathf.InverseLerp(minValue, maxValue, value);
            entry.blendTime = smoothTime;
        }

        private void Update()
        {
            if (sceneEffect == null) return;

            for (int i = 0; i < entries.Count; i++)
            {
                var e = entries[i];
                e.current = Mathf.SmoothDamp(e.current, e.target, ref e.velocity, e.blendTime);
                e.material.SetFloat("_Intensity", e.current);

                bool shouldBeActive = e.current > 0.001f || e.target > 0f;
                if (shouldBeActive && !e.registered)
                {
                    sceneEffect.Register(e.material);
                    e.registered = true;
                }
                else if (!shouldBeActive && e.registered)
                {
                    sceneEffect.Unregister(e.material);
                    e.registered = false;
                }
            }
        }
    }
}