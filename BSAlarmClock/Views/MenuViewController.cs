using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.ViewControllers;
using BSAlarmClock.Configuration;
using BSAlarmClock.Models;
using CameraUtils.Core;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using VRUIControls;
using Zenject;
using SiraUtil.Zenject;
using BeatSaberMarkupLanguage;

namespace BSAlarmClock.Views
{
    /// デンパ時計さんの、BeatmapInformationを流用・参考にしてます。
    /// 参考元ソース:https://github.com/denpadokei/BeatmapInformation/blob/master/BeatmapInformation/Views/BeatmapInformationViewController.cs
    /// ライセンス:https://github.com/denpadokei/BeatmapInformation/blob/master/LICENSE

    [HotReload]
    public class MenuViewController : BSMLAutomaticViewController, IAsyncInitializable, IDisposable
    {
        private bool _disposedValue;
        private AudioSource _audioSource;
        private BSAlarmClockController _bsAlarmClockController;
        private AlarmSoundController _alarmSoundController;
        private SettingTabViewController _settingTabViewController;
        public static readonly string SceneMenu = "MainMenu";
        public readonly GameObject _screenObject = new GameObject("BSAlarmClockMenuScreen");
        public readonly GameObject _audioSourceObject = new GameObject("BSAlarmMenuAudioSource");

        public bool _init;
        public bool _active;
        public FloatingScreen _alarmClockScreen;
        public int _sortinglayerOrder;
        public bool _alarmActive;

        [UIComponent("timeValue")]
        public readonly TextMeshProUGUI _timeValue;
        [UIComponent("timerValue")]
        public readonly TextMeshProUGUI _timerValue;
        [UIComponent("AlarmStopButton")]
        public readonly Button _alarmStopButton;

        [Inject]
        private void Constractor(BSAlarmClockController bsAlarmClockController, AlarmSoundController alarmSoundController, SettingTabViewController settingTabViewController)
        {
            this._bsAlarmClockController = bsAlarmClockController;
            this._alarmSoundController = alarmSoundController;
            this._settingTabViewController = settingTabViewController;
        }

        public async Task InitializeAsync(CancellationToken token)
        {
            this._alarmActive = false;
            this._init = false;
            this._active = true;
            SceneManager.activeSceneChanged += this.OnActiveSceneChanged;
            this._bsAlarmClockController._timeUpdated += this.OnTimeUpdate;
            this._bsAlarmClockController._alarmPing += this.OnAlarmPing;
            if (_alarmClockScreen != null)
                return;
            var screenSize = new Vector2(PluginConfig.Instance.MenuScreenSize * PluginConfig.Instance.ScreenSizeX, PluginConfig.Instance.MenuScreenSize * PluginConfig.Instance.ScreenSizeY);
            var screenPosition = new Vector3(PluginConfig.Instance.MenuScreenPosX, PluginConfig.Instance.MenuScreenPosY, PluginConfig.Instance.MenuScreenPosZ);
            this._alarmClockScreen = FloatingScreen.CreateFloatingScreen(screenSize, true, screenPosition, Quaternion.identity);
            this._alarmClockScreen.SetRootViewController(this, AnimationType.None);
            this._alarmClockScreen.transform.SetParent(this._screenObject.transform);
            this._alarmStopButton.SetButtonTextSize(PluginConfig.Instance.MenuScreenSize * PluginConfig.Instance.AlarmStopButtonSize);
            DontDestroyOnLoad(this._screenObject);
            var canvas = this._alarmClockScreen.GetComponentsInChildren<Canvas>(true).FirstOrDefault();
            canvas.renderMode = RenderMode.WorldSpace;
            this._alarmClockScreen.transform.rotation = Quaternion.Euler(PluginConfig.Instance.MenuScreenRotX, PluginConfig.Instance.MenuScreenRotY, PluginConfig.Instance.MenuScreenRotZ);
            this._alarmClockScreen.HandleSide = FloatingScreen.Side.Top;
            this._alarmClockScreen.HandleReleased += this.OnHandleReleased;
            this._audioSource = _audioSourceObject.AddComponent<AudioSource>();
            DontDestroyOnLoad(this._audioSourceObject);
            this._alarmStopButton.gameObject.SetActive(false);
            await this.AudioSourceSetttingAsync(token);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                if (disposing)
                {
                    SceneManager.activeSceneChanged -= this.OnActiveSceneChanged;
                    this._bsAlarmClockController._timeUpdated -= this.OnTimeUpdate;
                    this._bsAlarmClockController._alarmPing -= this.OnAlarmPing;
                    if (this._alarmClockScreen != null)
                    {
                        this._alarmClockScreen.HandleReleased -= this.OnHandleReleased;
                        Destroy(this._alarmClockScreen);
                    }
                    if (this._screenObject != null)
                        Destroy(this._screenObject);
                    if (this._audioSourceObject != null)
                        Destroy(this._audioSourceObject);
                    Plugin.Log.Info("MenuViewController Destroy");
                }
                this._disposedValue = true;
            }
        }
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        [UIAction("#post-parse")]
        public void PostParse()
        {
            StartCoroutine(this.CanvasConfigUpdate());
        }

        [UIAction("AlarmStop")]
        public void AlarmStop()
        {
            PluginConfig.Instance.AlarmEnabled = false;
            this._bsAlarmClockController.AlarmSet();
            this._alarmStopButton.gameObject.SetActive(false);
            this._settingTabViewController.AlarmStatusSet();
        }

        public void OnHandleReleased(object sender, FloatingScreenHandleEventArgs e)
        {
            PluginConfig.Instance.MenuScreenPosX = e.Position.x;
            PluginConfig.Instance.MenuScreenPosY = e.Position.y;
            PluginConfig.Instance.MenuScreenPosZ = e.Position.z;
            var rot = e.Rotation.eulerAngles;
            PluginConfig.Instance.MenuScreenRotX = rot.x;
            PluginConfig.Instance.MenuScreenRotY = rot.y;
            PluginConfig.Instance.MenuScreenRotZ = rot.z;
        }

        public void OnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            this.ScreenActiveCheck(arg1.name);
            if (arg1.name == SceneMenu)
            {
                this._settingTabViewController.AlarmStatusSet();
                if (PluginConfig.Instance.AlarmEnabled && this._alarmActive)
                    this._alarmStopButton.gameObject.SetActive(true);
                else
                    this._alarmStopButton.gameObject.SetActive(false);
            }
        }

        public void OnTimeUpdate(string time, string timer)
        {
            if (this._alarmActive && !PluginConfig.Instance.AlarmEnabled)
                this._alarmActive = false;
            if (!this._init || !this._active)
                return;
            this._timeValue.text = time;
            this._timerValue.text = timer;
            if (this._alarmActive)
            {
                this._timeValue.color = Color.red;
                this._timerValue.color = Color.red;
            }
            else
            {
                this._timeValue.color = Color.white;
                this._timerValue.color = Color.white;
            }
        }

        public void OnAlarmPing()
        {
            if (!this._init || !PluginConfig.Instance.AlarmEnabled)
                return;
            this.AlarmSoundPlay();
            if (!this._alarmActive)
            {
                this._alarmActive = true;
                this._alarmStopButton.gameObject.SetActive(true);
            }
        }
        public void AlarmSoundPlay()
        {
            if (this._alarmSoundController._AlarmClip != null && !this._audioSource.isPlaying && !this._bsAlarmClockController._gamePlayActive)
                this._audioSource.PlayOneShot(this._alarmSoundController._AlarmClip);
        }

        public IEnumerator CanvasConfigUpdate()
        {
            yield return new WaitWhile(() => this._alarmClockScreen == null || !this._alarmClockScreen);
            foreach (var canvas in this._alarmClockScreen.GetComponentsInChildren<Canvas>())
            {
                canvas.worldCamera = Camera.main;
                canvas.sortingOrder = PluginConfig.Instance.MenuUiSortingOrder;
            }
            this.CanvasLayerUpdate();
            foreach (var graphic in this._alarmClockScreen.GetComponentsInChildren<Graphic>())
                graphic.raycastTarget = false;
            try
            {
                Destroy(this._alarmClockScreen.GetComponent<VRGraphicRaycaster>());
            }
            catch (Exception e)
            {
                Plugin.Log.Error(e);
            }
            this._alarmClockScreen.ShowHandle = false;
            this._timeValue.color = Color.white;
            this._timeValue.overflowMode = TextOverflowModes.Overflow;
            this._timeValue.fontSize = PluginConfig.Instance.MenuScreenSize * PluginConfig.Instance.TimeFontSize;
            this._timerValue.color = Color.white;
            this._timerValue.overflowMode = TextOverflowModes.Overflow;
            this._timerValue.fontSize = PluginConfig.Instance.MenuScreenSize * PluginConfig.Instance.TimerFontSize;
            this._init = true;
            this._bsAlarmClockController.TimeUpdate();
            this.ScreenActiveCheck(null);
            this.HandleCheck();
        }
        public void ScreenSizeChange()
        {
            this._alarmClockScreen.ScreenSize = new Vector2(PluginConfig.Instance.MenuScreenSize * PluginConfig.Instance.ScreenSizeX, PluginConfig.Instance.MenuScreenSize * PluginConfig.Instance.ScreenSizeY);
            this._timeValue.fontSize = PluginConfig.Instance.MenuScreenSize * PluginConfig.Instance.TimeFontSize;
            this._timerValue.fontSize = PluginConfig.Instance.MenuScreenSize * PluginConfig.Instance.TimerFontSize;
            this._alarmClockScreen.transform.position = new Vector3(PluginConfig.Instance.MenuScreenPosX, PluginConfig.Instance.MenuScreenPosY, PluginConfig.Instance.MenuScreenPosZ);
            this._alarmClockScreen.transform.rotation = Quaternion.Euler(PluginConfig.Instance.MenuScreenRotX, PluginConfig.Instance.MenuScreenRotY, PluginConfig.Instance.MenuScreenRotZ);
            this._alarmStopButton.SetButtonTextSize(PluginConfig.Instance.MenuScreenSize * PluginConfig.Instance.AlarmStopButtonSize);
        }
        public void CanvasLayerUpdate()
        {
            foreach (var transform in this._alarmClockScreen.GetComponentsInChildren<Transform>())
            {
                if (PluginConfig.Instance.MenuHMDOnly)
                    transform.gameObject.SetLayer(PluginConfig.Instance.HMDOnlyLayer);
                else
                    transform.gameObject.SetLayer(PluginConfig.Instance.DefaultLayer);
            }
        }
        public void HandleCheck()
        {
            if (PluginConfig.Instance.MenuLockPosition)
            {
                if (this._alarmClockScreen == null || !this._active)
                    return;
                this._alarmClockScreen.ShowHandle = false;
            }
            else
            {
                if (this._alarmClockScreen == null || !this._active)
                    return;
                this._alarmClockScreen.ShowHandle = true;
            }
        }
        public void ScreenActiveCheck(string sceneName)
        {
            if (sceneName == null)
                sceneName = SceneMenu;
            if (sceneName == SceneMenu && !PluginConfig.Instance.MenuScreenHidden)
            {
                this._active = true;
                this._screenObject.SetActive(true);
            }
            else
            {
                this._active = false;
                this._screenObject.SetActive(false);
            }
        }

        private async Task AudioSourceSetttingAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested && this._audioSource == null)
            {
                await Task.Yield();
            }
            if (token.IsCancellationRequested)
            {
                return;
            }
            this._audioSource.volume = PluginConfig.Instance.AlarmVolume / 100f;
        }
        public void AudioSourceSetting()
        {
            if (this._audioSource == null)
                return;
            this._audioSource.volume = PluginConfig.Instance.AlarmVolume / 100f;
        }
    }
}
