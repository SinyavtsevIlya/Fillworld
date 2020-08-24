using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DG.Tweening
{
    public class DGExtensions
    {
        
    }

    [Serializable]
    public class TweenData
    {
        public Tweener Tweener;
        public float Duration;
        public float Delay;
        public Ease Ease;
    }

    [System.Serializable]
    public class ShakeTweenData
    {
        public Tweener Tweener;
        public float Duration;
        public float Strength;
        public int Vibrato;
    }
}
