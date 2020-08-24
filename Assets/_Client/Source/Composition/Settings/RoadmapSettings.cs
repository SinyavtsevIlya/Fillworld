using UnityEngine;

namespace FillWorld
{
    [CreateAssetMenu(fileName = "Roadmap", menuName = "Settings/Roadmap")]
    public class RoadmapSettings : ScriptableObject
    {
        public SceneReference [] Levels;

        public SceneReference GetLevel (int index)
        {
            return Levels[index % Levels.Length];
        }
    }
}
