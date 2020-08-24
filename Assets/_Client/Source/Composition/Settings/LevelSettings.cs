using System;
using UnityEngine;

namespace FillWorld
{
    [ExecuteInEditMode]
    public class LevelSettings : MonoBehaviour
    {
        public string Pattern;
        public string[] ExtraWords;
        public bool HasExtraWords
        {
            get
            {
                if (ExtraWords == null) return false;
                if (ExtraWords.Length == 0) return false;
                return true;
            }
        }
        [SerializeField]
        public EnvironmentSettings Environment;

#if UNITY_EDITOR
        void Update()
        {
            if (Application.isPlaying) return;

            Environment.SkyColor = RenderSettings.ambientSkyColor;
            Environment.EquatorColor = RenderSettings.ambientEquatorColor;
            Environment.GroundColor = RenderSettings.ambientGroundColor;
            Environment.Fog.Color = RenderSettings.fogColor;
            Environment.Fog.Mode = RenderSettings.fogMode;
            Environment.Fog.Dencity = RenderSettings.fogDensity;
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }

    [Serializable]
    public class EnvironmentSettings
    {
        [Serializable]
        public class FogSettings
        {
            public float Dencity;
            public Color Color;
            public FogMode Mode;
        }

        public FogSettings Fog;
        public Color SkyColor;
        public Color EquatorColor;
        public Color GroundColor;
    }
}
