using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.ViewControllers;
using BSAlarmClock.Configuration;
using IPA.Utilities;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using VRUIControls;
using CameraUtils.Core;
using TMPro;

namespace BSAlarmClock.Views
{
    /// デンパ時計さんの、BeatmapInformationを流用・参考にしてます。
    /// 参考元ソース:https://github.com/denpadokei/BeatmapInformation/blob/master/BeatmapInformation/Views/BeatmapInformationViewController.cs
    /// ライセンス:https://github.com/denpadokei/BeatmapInformation/blob/master/LICENSE

    [HotReload]
    internal class PlayerViewController : BSMLAutomaticViewController, IInitializable
    {
        private PauseController _pauseController;

        public bool _init;
        public FloatingScreen _alarmClockScreen;
        public int _sortinglayerOrder;

        [UIComponent("timeValue")]
        public readonly TextMeshProUGUI _timeValue;
        [UIComponent("timerValue")]
        public readonly TextMeshProUGUI _timerValue;

        [Inject]
        private void Constractor(DiContainer container)
        {
            this._init = false;
            this._pauseController = container.TryResolve<PauseController>();
        }
        public void Initialize()
        {
            if (!PluginConfig.Instance.Enable)
                return;
            if (this._pauseController != null)
            {
                this._pauseController.didPauseEvent += this.OnDidPauseEvent;
                this._pauseController.didResumeEvent += this.OnDidResumeEvent;
            }
            var screenSize = new Vector2(PluginConfig.Instance.GameScreenSize * 3f, PluginConfig.Instance.GameScreenSize * 2f);
            var screenPosition = new Vector3(PluginConfig.Instance.GameScreenPosX, PluginConfig.Instance.GameScreenPosY, PluginConfig.Instance.GameScreenPosZ);
            this._alarmClockScreen = FloatingScreen.CreateFloatingScreen(screenSize, true, screenPosition, Quaternion.Euler(0f, 0f, 0f));
            this._alarmClockScreen.SetRootViewController(this, AnimationType.None);
            var canvas = this._alarmClockScreen.GetComponentsInChildren<Canvas>(true).FirstOrDefault();
            canvas.renderMode = RenderMode.WorldSpace;
            this._alarmClockScreen.transform.rotation = Quaternion.Euler(PluginConfig.Instance.GameScreenRotX, PluginConfig.Instance.GameScreenRotY, PluginConfig.Instance.GameScreenRotZ);
            this._alarmClockScreen.HandleReleased += this.OnHandleReleased;
            this._alarmClockScreen.HandleSide = FloatingScreen.Side.Top;
        }

        protected override void OnDestroy()
        {
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
            base.OnDestroy();
        }

        [UIAction("#post-parse")]
        public void PostParse()
        {
            if (!PluginConfig.Instance.Enable)
                return;
            StartCoroutine(this.CanvasConfigUpdate());
        }

        [UIAction("AlarmStop")]
        public void AlarmStop()
        {

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
            if (PluginConfig.Instance.LockPosition)
                return;
            if (this._alarmClockScreen == null)
                return;
            foreach (var canvas in this._alarmClockScreen.GetComponentsInChildren<Canvas>())
                canvas.sortingOrder = this._sortinglayerOrder;
            this._alarmClockScreen.ShowHandle = false;
        }

        public void OnDidPauseEvent()
        {
            if (PluginConfig.Instance.LockPosition)
                return;
            if (this._alarmClockScreen == null)
                return;
            foreach (var canvas in this._alarmClockScreen.GetComponentsInChildren<Canvas>())
                canvas.sortingOrder = PluginConfig.Instance.GameUiSortingOrder;
            this._alarmClockScreen.ShowHandle = true;
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
                        if (PluginConfig.Instance.HMDOnly)
                            canvas.gameObject.SetLayer(PluginConfig.Instance.HMDOnlyLayer);
                        else
                            canvas.gameObject.SetLayer(PluginConfig.Instance.DefaultLayer);
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
            this.CanvasSet();
        }
        public void CanvasSet()
        {
            this._alarmClockScreen.ShowHandle = false;
            this._timeValue.color = Color.white;
            this._timeValue.overflowMode = TextOverflowModes.Overflow;
            this._timeValue.fontSize = PluginConfig.Instance.GameScreenSize / 1.8f;
            this._timerValue.color = Color.white;
            this._timerValue.overflowMode = TextOverflowModes.Overflow;
            this._timerValue.fontSize = PluginConfig.Instance.GameScreenSize / 2.5f;
            this._init = true;
        }

        public void SetValue(string time, string timer)
        {
            if (!this._init)
                return;
            this._timeValue.text = time;
            this._timerValue.text = timer;
        }
    }
}
