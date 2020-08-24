using System;
using UniRx;
using UnityEngine;
using CleanRx;

namespace FillWorld
{
    [Serializable] public class CharSequence : ReactiveCollection<Entity> { }

    [Serializable] public class OrderIndex : IntReactiveProperty { }
    [Serializable] public class CharPattern : ReactiveCollection<Entity> { }
    [Serializable] public class Letter : ReactiveProperty<Char> { }
    [Serializable] public class Name : StringReactiveProperty { }
    [Serializable] public class View : ReactiveProperty<GameObject> { }
    [Serializable] public class LevelIndex : IntReactiveProperty { }
    [Serializable] public class SceneAsset : ReactiveProperty<UnityEngine.SceneManagement.Scene> { }
    [Serializable] public class Next : ReactiveProperty<Entity> { }
    [Serializable] public class Previous : ReactiveProperty<Entity> { }
    [Serializable] public class Active { }
    [Serializable] public class IsExtra { }
    [Serializable] public class Matched { }
    [Serializable] public class Hinted { }
    [Serializable] public class Particle : ReactiveProperty<ParticleSystem> { }
    [Serializable] public class ComboResult : UniRx.Signal<bool> { }
    [Serializable] public class HintRequest : UniRx.Signal<Unit> { }
    [Serializable] public class CursorDown : BoolReactiveProperty { }
    [Serializable] public class Position : ReactiveProperty<Vector3> { }
    [Serializable] public class Position2D : ReactiveProperty<Vector2> { }
}
