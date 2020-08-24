using System;
using UnityEngine;

namespace FillWorld
{
    [CreateAssetMenu(fileName = "CoreView", menuName = "Settings/CoreView")]
    public class CoreViewSettings : ScriptableObject
    {
        public ActorViewSettings Actor;
    }
}
