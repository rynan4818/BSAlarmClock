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
using VRUIControls;
using Zenject;
using SiraUtil.Zenject;
using IPA.Utilities;
using BeatSaberMarkupLanguage;

namespace BSAlarmClock.Views
{
    /// デンパ時計さんの、BeatmapInformationを流用・参考にしてます。
    /// 参考元ソース:https://github.com/denpadokei/BeatmapInformation/blob/master/BeatmapInformation/Views/BeatmapInformationViewController.cs
    /// ライセンス:https://github.com/denpadokei/BeatmapInformation/blob/master/LICENSE

    [HotReload]
    internal class PlayerViewController : BSMLAutomaticViewController, IAsyncInitializable
    {
        private PauseController _pauseController;
        private AudioSource _audioSource;
        private BSAlarmClockController _bsAlarmClockController;
        private AlarmSoundController _alarmSoundController;
        public readonly GameObject _screenObject = new GameObject("BSAlarmClockMenuScreen");
        public readonly GameObject _audioSourceObject = new GameObject("BSAlarmGameAudioSource");
        public bool _init;
        public FloatingScreen _alarmClockScreen;
        public int _sortinglayerOrder;
        public bool _alarmActive;
        public bool _hiddenAlarm;

        [UIComponent("timeValue")]
        public readonly TextMeshProUGUI _timeValue;
        [UIComponent("timerValue")]
        public readonly TextMeshProUGUI _timerValue;
        [UIComponent("AlarmStopButton")]
        public readonly Button _alarmStopButton;

        [Inject]
        private void Constractor(DiContainer container)
        {
            this._init = false;
            this._pauseController = container.TryResolve<PauseController>();
            this._bsAlarmClockController = container.Resolve<BSAlarmClockController>();
            this._alarmSoundController = container.Resolve<AlarmSoundController>();
        }
        public async Task InitializeAsync(CancellationToken token)
        {
            this._bsAlarmClockController._gamePlayActive = true;
            this._alarmActive = false;
            this._hiddenAlarm = false;
            this._bsAlarmClockController._timeUpdated += this.OnTimeUpdate;
            this._bsAlarmClockController._alarmPing += this.OnAlarmPing;
            if (this._pauseController != null)
            {
                this._pauseController.didPauseEvent += this.OnDidPauseEvent;
                this._pauseController.didResumeEvent += this.OnDidResumeEvent;
            }
            var screenSize = new Vector2(PluginConfig.Instance.GameScreenSize * PluginConfig.Instance.ScreenSizeX, PluginConfig.Instance.GameScreenSize * PluginConfig.Instance.ScreenSizeY);
            var screenPosition = new Vector3(PluginConfig.Instance.GameScreenPosX, PluginConfig.Instance.GameScreenPosY, PluginConfig.Instance.GameScreenPosZ);
            this._alarmClockScreen = FloatingScreen.CreateFloatingScreen(screenSize, true, screenPosition, Quaternion.Euler(0f, 0f, 0f));
            this._alarmClockScreen.transform.SetParent(this._screenObject.transform);
            this._alarmClockScreen.SetRootViewController(this, AnimationType.None);
            this._alarmStopButton.SetButtonTextSize(PluginConfig.Instance.MenuScreenSize * PluginConfig.Instance.AlarmStopButtonSize);
            var canvas = this._alarmClockScreen.GetComponentsInChildren<Canvas>(true).FirstOrDefault();
            canvas.renderMode = RenderMode.WorldSpace;
            this._alarmClockScreen.transform.rotation = Quaternion.Euler(PluginConfig.Instance.GameScreenRotX, PluginConfig.Instance.GameScreenRotY, PluginConfig.Instance.GameScreenRotZ);
            this._alarmClockScreen.HandleReleased += this.OnHandleReleased;
            this._alarmClockScreen.HandleSide = FloatingScreen.Side.Top;
            this._audioSource = _audioSourceObject.AddComponent<AudioSource>();
            this._alarmStopButton.gameObject.SetActive(false);
            await this.AudioSourcesSetttingAsync(token);
        }

        protected override void OnDestroy()
        {
            this._bsAlarmClockController._gamePlayActive = false;
            this._bsAlarmClockController._timeUpdated -= this.OnTimeUpdate;
            this._bsAlarmClockController._alarmPing -= this.OnAlarmPing;
            if (this._pauseController != null)
            {
                this._pauseController.didPauseEvent -= this.OnDidPauseEvent;
                this._pauseController.didResumeEvent -= this.OnDidResumeEvent;
            }
            if (this._alarmClockScreen != null)
            {
                this._alarmClockScreen.HandleReleased -= this.OnHandleReleased;
                Destroy(this._alarmClockScreen);
            }
            if (this._screenObject != null)
                Destroy(this._screenObject);
            if (this._audioSourceObject != null)
                Destroy(this._audioSourceObject);
            base.OnDestroy();
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
            this._hiddenAlarm = false;
            this._alarmStopButton.gameObject.SetActive(false);
        }

        public void OnHandleReleased(object sender, FloatingScreenHandleEventArgs e)
        {
            PluginConfig.Instance.GameScreenPosX = e.Position.x;
            PluginConfig.Instance.GameScreenPosY = e.Position.y;
            PluginConfig.Instance.GameScreenPosZ = e.Position.z;
            var rot = e.Rotation.eulerAngles;
            PluginConfig.Instance.GameScreenRotX = rot.x;
            PluginConfig.Instance.GameScreenRotY = rot.y;
            PluginConfig.Instance.GameScreenRotZ = rot.z;
        }

        public void OnDidResumeEvent()
        {
            if (this._hiddenAlarm)
                this._screenObject.SetActive(false);
            this._alarmStopButton.gameObject.SetActive(false);
            if (PluginConfig.Instance.GameLockPosition || PluginConfig.Instance.GameScreenHidden)
                return;
            if (this._alarmClockScreen == null)
                return;
            foreach (var canvas in this._alarmClockScreen.GetComponentsInChildren<Canvas>())
                canvas.sortingOrder = this._sortinglayerOrder;
            this._alarmClockScreen.ShowHandle = false;
        }

        public void OnDidPauseEvent()
        {
            if (this._hiddenAlarm)
                this._screenObject.SetActive(true);
            if (PluginConfig.Instance.AlarmEnabled)
                this._alarmStopButton.gameObject.SetActive(true);
            if (PluginConfig.Instance.GameLockPosition || PluginConfig.Instance.GameScreenHidden)
                return;
            if (this._alarmClockScreen == null)
                return;
            foreach (var canvas in this._alarmClockScreen.GetComponentsInChildren<Canvas>())
                canvas.sortingOrder = PluginConfig.Instance.GameUiSortingOrder;
            this._alarmClockScreen.ShowHandle = true;
        }
        public void OnTimeUpdate(string time, string timer)
        {
            if (this._alarmActive && !PluginConfig.Instance.AlarmEnabled)
                this._alarmActive = false;
            if (!this._init || (PluginConfig.Instance.GameScreenHidden && !this._hiddenAlarm))
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
            if (PluginConfig.Instance.GameScreenHidden && !this._hiddenAlarm)
                this._hiddenAlarm = true;
            if (this._alarmSoundController._AlarmClip != null && !this._audioSource.isPlaying && !PluginConfig.Instance.AlarmSoundMenuOnly && PluginConfig.Instance.AlarmSoundEnabled)
                this._audioSource.PlayOneShot(this._alarmSoundController._AlarmClip);
            if (!this._alarmActive)
                this._alarmActive = true;
        }

        public IEnumerator CanvasConfigUpdate()
        {
            yield return new WaitWhile(() => this._alarmClockScreen == null || !this._alarmClockScreen);
            try
            {
                var coreGameHUDController = Resources.FindObjectsOfTypeAll<CoreGameHUDController>().FirstOrDefault();
                if (coreGameHUDController != null)
                {
                    var energyGo = coreGameHUDController.GetField<GameObject, CoreGameHUDController>("_energyPanelGO");
                    var energyCanvas = energyGo.GetComponent<Canvas>();
                    foreach (var canvas in this._alarmClockScreen.GetComponentsInChildren<Canvas>())
                    {
                        canvas.worldCamera = Camera.main;
                        canvas.overrideSorting = energyCanvas.overrideSorting;
                        canvas.sortingLayerID = energyCanvas.sortingLayerID;
                        canvas.sortingLayerName = energyCanvas.sortingLayerName;
                        this._sortinglayerOrder = energyCanvas.sortingOrder;
                        canvas.sortingOrder = this._sortinglayerOrder;
                    }
                    foreach (var transform in this._alarmClockScreen.GetComponentsInChildren<Transform>())
                    {
                        if (PluginConfig.Instance.GameHMDOnly)
                            transform.gameObject.SetLayer(PluginConfig.Instance.HMDOnlyLayer);
                        else
                            transform.gameObject.SetLayer(PluginConfig.Instance.DefaultLayer);
                    }
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
                }
            }
            catch (Exception e)
            {
                Plugin.Log.Error(e);
            }
            this._alarmClockScreen.ShowHandle = false;
            this._timeValue.color = Color.white;
            this._timeValue.overflowMode = TextOverflowModes.Overflow;
            this._timeValue.fontSize = PluginConfig.Instance.GameScreenSize * PluginConfig.Instance.TimeFontSize;
            this._timerValue.color = Color.white;
            this._timerValue.overflowMode = TextOverflowModes.Overflow;
            this._timerValue.fontSize = PluginConfig.Instance.GameScreenSize * PluginConfig.Instance.TimerFontSize;
            this._init = true;
            this._bsAlarmClockController.TimeUpdate();
            if (PluginConfig.Instance.GameScreenHidden)
            {
                this._screenObject.SetActive(false);
            }
        }
        private async Task AudioSourcesSetttingAsync(CancellationToken token)
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
    }
}
