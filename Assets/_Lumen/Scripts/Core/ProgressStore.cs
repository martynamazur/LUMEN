using System;
using System.IO;
using UnityEngine;
namespace Lumen.Core
{
    public interface IProgressStore { bool TutorialCompleted { get; } void CompleteTutorial(); }
    public sealed class ProgressStore : IProgressStore
    {
        [Serializable] private sealed class Data { public int version = 1; public bool tutorialCompleted; }
        private Data data = new Data();
        private readonly string path;
        public bool TutorialCompleted => data.tutorialCompleted;
        public ProgressStore(string path)
        {
            this.path = path;
            try { if (File.Exists(path)) { var loaded = JsonUtility.FromJson<Data>(File.ReadAllText(path)); if (loaded != null && loaded.version == 1) data = loaded; else Debug.LogWarning("LUMEN: nieobsługiwany zapis postępu."); } }
            catch (Exception e) { Debug.LogWarning("LUMEN: nie udało się odczytać postępu: " + e.Message); }
        }
        public void CompleteTutorial()
        {
            if (data.tutorialCompleted) return;
            var next = new Data { tutorialCompleted = true };
            var folder = Path.GetDirectoryName(path); if (!string.IsNullOrEmpty(folder)) Directory.CreateDirectory(folder);
            var temporary = path + ".tmp";
            File.WriteAllText(temporary, JsonUtility.ToJson(next, true));
            if (File.Exists(path)) File.Replace(temporary, path, null); else File.Move(temporary, path);
            data = next;
        }
    }
}
