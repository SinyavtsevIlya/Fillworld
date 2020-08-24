using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;
using CleanRx;

namespace FillWorld
{
    public class ActorFactory : PlaceholderFactory<ActorSettings, Entity> { } 
    public class ExtraActorFactory : PlaceholderFactory<string, Entity> { } 
    public class LetterFactory : PlaceholderFactory<char, Entity> { } 
    public class LevelFactory : PlaceholderFactory<int, Entity> { } 
}
