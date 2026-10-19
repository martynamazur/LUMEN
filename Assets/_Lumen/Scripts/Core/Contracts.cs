using System;
using System.Threading.Tasks;
namespace Lumen.Core
{
    public interface ISceneLoader { bool IsLoading { get; } Task LoadAsync(string scene); }
    public interface ISettingsStore { float Volume { get; } bool Muted { get; } void SetVolume(float value); void SetMuted(bool value); }
    public enum SessionState { Menu, Playing, Paused, Completed }
    public sealed class GameSession
    {
        public SessionState State { get; private set; } = SessionState.Menu;
        public event Action<SessionState> Changed;
        public void EnterMenu() => Set(SessionState.Menu);
        public void Complete() => Set(SessionState.Completed);
        public void BeginPlay() => Set(SessionState.Playing);
        public void Pause() { if (State == SessionState.Playing) Set(SessionState.Paused); }
        public void Resume() { if (State == SessionState.Paused) Set(SessionState.Playing); }
        private void Set(SessionState state) { if (State == state) return; State = state; Changed?.Invoke(state); }
    }
}
