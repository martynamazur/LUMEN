using UnityEngine;
namespace Lumen.Core
{
    public sealed class SettingsStore : ISettingsStore
    {
        private const string VolumeKey = "Lumen.Audio.Volume";
        private const string MutedKey = "Lumen.Audio.Muted";
        public float Volume { get; private set; }
        public bool Muted { get; private set; }
        public SettingsStore() { Volume = Mathf.Clamp01(PlayerPrefs.GetFloat(VolumeKey, .65f)); Muted = PlayerPrefs.GetInt(MutedKey, 0) == 1; Apply(); }
        public void SetVolume(float value) { Volume = Mathf.Clamp01(value); PlayerPrefs.SetFloat(VolumeKey, Volume); PlayerPrefs.Save(); Apply(); }
        public void SetMuted(bool value) { Muted = value; PlayerPrefs.SetInt(MutedKey, value ? 1 : 0); PlayerPrefs.Save(); Apply(); }
        private void Apply() => AudioListener.volume = Muted ? 0f : Volume;
    }
}
