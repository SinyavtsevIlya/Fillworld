using System;
using UnityEngine;
using DG.Tweening;

namespace FillWorld
{
    [CreateAssetMenu(fileName = "ActorView", menuName = "Settings/ActorView")]
    public class ActorViewSettings : ScriptableObject
    {
        public Material MatchMateril;
        public DG.Tweening.TweenData Match;
        public GameObject ExplosionPS;
        public float ExplosionScaleMultiplier;
        public DG.Tweening.ShakeTweenData Hint;
    }
}
