using System;
using UniRx;
using UnityEngine;
using CleanRx;


namespace FillWorld
{
    public class LetterSelected : Signal<Entity> { }
    public class LevelCompleted : Signal { }
}
