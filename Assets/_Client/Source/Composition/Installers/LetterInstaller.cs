using Zenject;
using CleanRx;

namespace FillWorld
{
    public class LetterInstaller : Installer<LetterInstaller>
    {
        [Inject] CleanRx.Context _context;
        [Inject] char _letter;

        public override void InstallBindings()
        {
            var letter = _context.CreateEntity();
            Container.BindInstance(letter).AsSingle();
            letter.Add<Letter>().Value = _letter;
        }
    }
}
