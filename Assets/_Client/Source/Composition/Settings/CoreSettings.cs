using System;
using UnityEngine;

namespace FillWorld
{
    [CreateAssetMenu(fileName = "Core", menuName = "Settings/Core")]
    public class CoreSettings : ScriptableObject
    {
        public CoreSimulationSettings Simulation;
        public CoreViewSettings View;
    }
}
