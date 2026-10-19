using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Lumen.Core
{
    public sealed class SceneLoader : ISceneLoader
    {
        private readonly GameSession session;
        public bool IsLoading { get; private set; }
        public SceneLoader(GameSession session) { this.session = session; }
        public async Task LoadAsync(string scene)
        {
            if (IsLoading) return;
            if (!Application.CanStreamedLevelBeLoaded(scene)) throw new System.ArgumentException("Scena nie jest dodana do builda: " + scene);
            IsLoading = true;
            try
            {
                Time.timeScale = 1f;
                var operation = SceneManager.LoadSceneAsync(scene);
                while (!operation.isDone) await Task.Yield();
                if (scene == "MainMenu") session.EnterMenu(); else session.BeginPlay();
            }
            finally { IsLoading = false; }
        }
    }
}
