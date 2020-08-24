using System;
using UnityEngine;

namespace FillWorld
{
    [CreateAssetMenu(fileName = "Letter", menuName = "Settings/Letter")]
    public class LetterSettings : ScriptableObject
    {
        public GameObject prefab;
    }
}
