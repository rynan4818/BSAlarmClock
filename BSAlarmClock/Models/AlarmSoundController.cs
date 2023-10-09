using BSAlarmClock.Configuration;
using IPA.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace BSAlarmClock.Models
{
    public class AlarmSoundController : IInitializable
    {
        public static readonly string SoundFolder = Path.Combine(UnityGame.UserDataPath, "BSAlarmClockSound");
        private readonly AudioClipAsyncLoader _audioClipAsyncLoader;
        public AudioClip _AlarmClip;

        public AlarmSoundController(AudioClipAsyncLoader audioClipAsyncLoader)
        {
            this._audioClipAsyncLoader = audioClipAsyncLoader;
        }
        public void Initialize()
        {
            if (!Directory.Exists(SoundFolder))
                Directory.CreateDirectory(SoundFolder);
            this.SetupDefaultFiles();
            _ = this.LoadAlarmClipAsync();
        }

        public void SetupDefaultFiles()
        {
            var path = Path.Combine(SoundFolder, "Alarm1.wav");
            if (!File.Exists(path))
                File.WriteAllBytes(path, BeatSaberMarkupLanguage.Utilities.GetResource(Assembly.GetExecutingAssembly(), "BSAlarmClock.Resources.Alarm1.wav"));
            path = Path.Combine(SoundFolder, "Alarm2.wav");
            if (!File.Exists(path))
                File.WriteAllBytes(path, BeatSaberMarkupLanguage.Utilities.GetResource(Assembly.GetExecutingAssembly(), "BSAlarmClock.Resources.Alarm2.wav"));
        }

        public async Task LoadAlarmClipAsync()
        {
            if(this._AlarmClip != null)
            {
                this._audioClipAsyncLoader.Unload(this._AlarmClip);
                this._AlarmClip = null;
            }
            var path = Path.Combine(SoundFolder, PluginConfig.Instance.AlarmSound);
            if (File.Exists(path))
            {
                try
                {
                    this._AlarmClip = await this._audioClipAsyncLoader.Load(path);
                }
                catch
                {
                    Plugin.Log.Error($"{path}:Alarm Sound Load Error");
                }
            }
        }

        public List<string> GetAlarmFiles()
        {
            var result = new List<string>();
            var di = new DirectoryInfo(SoundFolder);
            var files = di.GetFiles("*.wav");
            foreach(var f in files)
            {
                result.Add(f.Name);
            }
            return result;
        }
    }
}
