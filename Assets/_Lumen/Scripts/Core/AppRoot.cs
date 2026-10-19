using UnityEngine;
namespace Lumen.Core
{
    public sealed class AppRoot : MonoBehaviour
    {
        public static AppRoot Instance { get; private set; }
        public GameSession Session { get; private set; }
        public ISceneLoader Scenes { get; private set; }
        public ISettingsStore Settings { get; private set; }
        public IProgressStore Progress { get; private set; }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ClearStatics() => Instance = null;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap() { if (Instance == null) new GameObject("AppRoot").AddComponent<AppRoot>(); }
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this; DontDestroyOnLoad(gameObject);
            Session = new GameSession(); Settings = new SettingsStore(); Scenes = new SceneLoader(Session);
            Progress = new ProgressStore(System.IO.Path.Combine(Application.persistentDataPath, "progress.json"));
            Session.Changed += ApplyState;
        }
        private void ApplyState(SessionState state) => Time.timeScale = (state == SessionState.Paused || state == SessionState.Completed) ? 0f : 1f;
        private void OnApplicationPause(bool paused) { if (paused) Session.Pause(); }
        private void OnApplicationFocus(bool focused) { if (!focused) Session.Pause(); }
        private void OnDestroy() { if (Instance != this) return; Session.Changed -= ApplyState; Time.timeScale = 1f; Instance = null; }
    }
}
