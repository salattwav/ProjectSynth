using System.Collections.Generic;
using UnityEngine;

namespace ProjectSynth.Components
{
    // i hate it even more now
    public class SceneCamShaderEffect : MonoBehaviour
    {
        private readonly List<Material> activeMaterials = [];

        public void Register(Material material)
        {
            if (!activeMaterials.Contains(material))
                activeMaterials.Add(material);
        }

        public void Unregister(Material material)
        {
            activeMaterials.Remove(material);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (activeMaterials.Count == 0)
            {
                Graphics.Blit(source, destination);
                return;
            }

            RenderTexture current = source;
            for (int i = 0; i < activeMaterials.Count; i++)
            {
                bool isLast = i == activeMaterials.Count - 1;
                RenderTexture target = isLast
                    ? destination
                    : RenderTexture.GetTemporary(source.width, source.height, 0, source.format);

                Graphics.Blit(current, target, activeMaterials[i]);

                if (current != source)
                    RenderTexture.ReleaseTemporary(current);
                current = target;
            }
        }
    }
}