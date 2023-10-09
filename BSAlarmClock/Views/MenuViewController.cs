using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.ViewControllers;
using BSAlarmClock.Configuration;
using CameraUtils.Core;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRUIControls;
using Zenject;

namespace BSAlarmClock.Views
{
    /// デンパ時計さんの、BeatmapInformationを流用・参考にしてます。
    /// 参考元ソース:https://github.com/denpadokei/BeatmapInformation/blob/master/BeatmapInformation/Views/BeatmapInformationViewController.cs
    /// ライセンス:https://github.com/denpadokei/BeatmapInformation/blob/master/LICENSE

    [HotReload]
    public class MenuViewController : BSMLAutomaticViewController, IInitializable
    {
        public static readonly string SceneMenu = "MainMenu";
        public readonly GameObject _screenObject = new GameObject("BSAlarmClockScreen");

        public bool _init;
        public bool _active;
        public FloatingScreen _alarmClockScreen;
        public int _sortinglayerOrder;

        [UIComponent("timeValue")]
        public readonly TextMeshProUGUI _timeValue;
        [UIComponent("timerValue")]
        public readonly TextMeshProUGUI _timerValue;
        public void Initialize()
        {
            this._init = false;
            this._active = true;
            if (_alarmClockScreen != null)
                return;
            var screenSize = new Vector2(PluginConfig.Instance.MenuScreenSize * 3f, PluginConfig.Instance.MenuScreenSize * 2f);
            var screenPosition = new Vector3(PluginConfig.Instance.MenuScreenPosX, PluginConfig.Instance.MenuScreenPosY, PluginConfig.Instance.MenuScreenPosZ);
            this._alarmClockScreen = FloatingScreen.CreateFloatingScreen(screenSize, true, screenPosition, Quaternion.identity, 0f, true);
            this._alarmClockScreen.SetRootViewController(this, AnimationType.None);
            this._alarmClockScreen.transform.SetParent(this._screenObject.transform);
            DontDestroyOnLoad(this._screenObject);
            var canvas = this._alarmClockScreen.GetComponentsInChildren<Canvas>(true).FirstOrDefault();
            canvas.renderMode = RenderMode.WorldSpace;
            this._alarmClockScreen.transform.rotation = Quaternion.Euler(PluginConfig.Instance.MenuScreenRotX, PluginConfig.Instance.MenuScreenRotY, PluginConfig.Instance.MenuScreenRotZ);
            this._alarmClockScreen.HandleReleased += this.OnHandleReleased;
            this._alarmClockScreen.HandleSide = FloatingScreen.Side.Top;
            SceneManager.activeSceneChanged += this.ActiveSceneChanged;
        }
        protected override void OnDestroy()
        {
            SceneManager.activeSceneChanged -= this.ActiveSceneChanged;
            if (this._alarmClockScreen != null)
            {
                this._alarmClockScreen.HandleReleased -= this.OnHandleReleased;
                Destroy(this._alarmClockScreen);
            }
            if (this._screenObject != null)
                Destroy(this._screenObject);
            base.OnDestroy();
        }

        [UIAction("#post-parse")]
        public void PostParse()
        {
            StartCoroutine(this.CanvasConfigUpdate());
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

        public void ActiveSceneChanged(Scene arg0, Scene arg1)
        {
            if (arg1.name == SceneMenu && PluginConfig.Instance.Enable)
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

        public IEnumerator CanvasConfigUpdate()
        {
            yield return new WaitWhile(() => this._alarmClockScreen == null || !this._alarmClockScreen);
            foreach (var canvas in this._alarmClockScreen.GetComponentsInChildren<Canvas>())
            {
                canvas.worldCamera = Camera.main;
                canvas.sortingOrder = PluginConfig.Instance.MenuUiSortingOrder;
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
            this._alarmClockScreen.ShowHandle = false;
            this._timeValue.color = Color.white;
            this._timeValue.overflowMode = TextOverflowModes.Overflow;
            this._timeValue.fontSize = PluginConfig.Instance.GameScreenSize / 1.8f;
            this._timerValue.color = Color.white;
            this._timerValue.overflowMode = TextOverflowModes.Overflow;
            this._timerValue.fontSize = PluginConfig.Instance.GameScreenSize / 2.5f;
            this._init = true;
            if (!PluginConfig.Instance.Enable)
            {
                this._active = false;
                this._screenObject.SetActive(false);
            }
        }
        public void SetValue(string time, string timer)
        {
            if (!this._init || !this._active)
                return;
            this._timeValue.text = time;
            this._timerValue.text = timer;
        }

        public void ShowHandle()
        {
            if (this._alarmClockScreen == null || !this._active)
                return;
            this._alarmClockScreen.ShowHandle = false;
        }

        public void HideHandle()
        {
            if (this._alarmClockScreen == null || !this._active)
                return;
            this._alarmClockScreen.ShowHandle = true;
        }
    }
}
