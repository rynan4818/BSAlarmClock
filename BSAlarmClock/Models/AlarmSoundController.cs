using BSAlarmClock.Configuration;
using IPA.Utilities;
using IPA.Utilities.Async;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace BSAlarmClock.Models
{
    public class AlarmSoundController : IInitializable
    {
        public static readonly string SoundFolder = Path.Combine(UnityGame.UserDataPath, "BSAlarmClockSound");
        public AudioClip _AlarmClip;

        public void Initialize()
        {
            if (!Directory.Exists(SoundFolder))
                Directory.CreateDirectory(SoundFolder);
            this.SetupDefaultFiles();
            _ = Coroutines.AsTask(this.LoadAlarmClipCoroutine());
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

        public IEnumerator LoadAlarmClipCoroutine()
        {
            if(this._AlarmClip != null)
            {
                UnityEngine.Object.Destroy(this._AlarmClip);
                this._AlarmClip = null;
            }
            var path = Path.Combine(SoundFolder, PluginConfig.Instance.AlarmSound);
            if (File.Exists(path))
            {
                var clipResponse = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV);
                yield return clipResponse.SendWebRequest();
                if (clipResponse.error != null)
                {
                    Plugin.Log.Error($"Unity Web Request Failed! Error: {clipResponse.error}");
                    yield break;
                }
                else
                    this._AlarmClip = DownloadHandlerAudioClip.GetContent(clipResponse);
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
