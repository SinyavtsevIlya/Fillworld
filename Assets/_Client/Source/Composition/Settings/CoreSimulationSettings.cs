using System;
using UnityEngine;

namespace FillWorld
{
    [CreateAssetMenu(fileName = "CoreSimulation", menuName = "Settings/CoreSimulation")]
    public class CoreSimulationSettings : ScriptableObject
    {
        public RoadmapSettings Roadmap;
    }
}
