using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.ViewControllers;
using BSAlarmClock.Configuration;
using BSAlarmClock.Models;
using System;
using System.Collections.Generic;
using TMPro;
using Zenject;

namespace BSAlarmClock.Views
{
    public class SettingTabViewController : BSMLAutomaticViewController, IInitializable
    {
        private AlarmSoundController _alarmSoundController;
        private MenuViewController _menuViewController;
        private BSAlarmClockController _bsAlarmClockController;
        public static readonly string TabName = "BS Alarm Clock";
        public string ResourceName => string.Join(".", this.GetType().Namespace, this.GetType().Name);
        public int _timerHour = 0;
        public int _timerMin = 0;

        [UIValue("AlarmSoundChoices")]
        public List<object> AlarmSoundChoices { get; set; } = new List<object>();
        [UIComponent("AlarmStatus")]
        private readonly TextMeshProUGUI _alarmStatus;

        [Inject]
        private void Constractor(AlarmSoundController alarmSoundController, MenuViewController menuViewController, BSAlarmClockController bSAlarmClockController)
        {
            this._alarmSoundController = alarmSoundController;
            this._menuViewController = menuViewController;
            this._bsAlarmClockController = bSAlarmClockController;
        }

        public void Initialize()
        {
            GameplaySetup.instance.AddTab(TabName, this.ResourceName, this, MenuType.All);
            foreach (var file in this._alarmSoundController.GetAlarmFiles())
                AlarmSoundChoices.Add(file);
        }

        protected override void OnDestroy()
        {
            GameplaySetup.instance?.RemoveTab(TabName);
            base.OnDestroy();
        }

        public void AlarmStatusSet()
        {
            if (PluginConfig.Instance.AlarmEnabled)
                this._alarmStatus.text = "Alarm Status [ON]";
            else
                this._alarmStatus.text = "Alarm Status [OFF]";
        }

        [UIAction("#post-parse")]
        internal void PostParse()
        {
            this.AlarmStatusSet();
        }

        [UIValue("AlarmHour")]
        public int AlarmHour
        {
            get => PluginConfig.Instance.AlarmHour;
            set
            {
                if (PluginConfig.Instance.AlarmHour.Equals(value))
                    return;
                PluginConfig.Instance.AlarmHour = value;
                this._bsAlarmClockController.AlarmSet();
                this._menuViewController._alarmActive = false;
                NotifyPropertyChanged();
            }
        }
        [UIValue("AlarmMin")]
        public int AlarmMin
        {
            get => PluginConfig.Instance.AlarmMin;
            set
            {
                if (PluginConfig.Instance.AlarmMin.Equals(value))
                    return;
                PluginConfig.Instance.AlarmMin = value;
                this._bsAlarmClockController.AlarmSet();
                this._menuViewController._alarmActive = false;
                NotifyPropertyChanged();
            }
        }
        [UIValue("TimerHour")]
        public int TimerHour
        {
            get => this._timerHour;
            set => this._timerHour = value;
        }
        [UIValue("TimerMin")]
        public int TimerMin
        {
            get => this._timerMin;
            set => this._timerMin = value;
        }

        [UIValue("AlarmSound")]
        public string AlarmSound
        {
            get => PluginConfig.Instance.AlarmSound;
            set
            {
                PluginConfig.Instance.AlarmSound = value;
                _= this._alarmSoundController.LoadAlarmClipAsync();
            }
        }
        [UIValue("AlarmSoundMenuOnly")]
        public bool AlarmSoundMenuOnly
        {
            get => PluginConfig.Instance.AlarmSoundMenuOnly;
            set => PluginConfig.Instance.AlarmSoundMenuOnly = value;
        }
        [UIValue("AlarmVolume")]
        public int AlarmVolume
        {
            get => (int)PluginConfig.Instance.AlarmVolume;
            set
            {
                PluginConfig.Instance.AlarmVolume = value;
                this._menuViewController.AudioSourceSetting();
            }
        }
        [UIValue("AlarmSoundEnabled")]
        public bool AlarmSoundEnabled
        {
            get => PluginConfig.Instance.AlarmSoundEnabled;
            set => PluginConfig.Instance.AlarmSoundEnabled = value;
        }
        [UIValue("MenuLockPosition")]
        public bool MenuLockPosition
        {
            get => PluginConfig.Instance.MenuLockPosition;
            set
            {
                PluginConfig.Instance.MenuLockPosition = value;
                this._menuViewController.HandleCheck();
            }
        }
        [UIValue("GameLockPosition")]
        public bool GameLockPosition
        {
            get => PluginConfig.Instance.GameLockPosition;
            set => PluginConfig.Instance.GameLockPosition = value;
        }
        [UIValue("MenuScreenHidden")]
        public bool MenuScreenHidden
        {
            get => PluginConfig.Instance.MenuScreenHidden;
            set
            {
                PluginConfig.Instance.MenuScreenHidden = value;
                this._menuViewController.ScreenActiveCheck(null);
            }
        }
        [UIValue("GameScreenHidden")]
        public bool GameScreenHidden
        {
            get => PluginConfig.Instance.GameScreenHidden;
            set => PluginConfig.Instance.GameScreenHidden = value;
        }
        [UIValue("MenuHMDOnly")]
        public bool MenuHMDOnly
        {
            get => PluginConfig.Instance.MenuHMDOnly;
            set
            {
                PluginConfig.Instance.MenuHMDOnly = value;
                this._menuViewController.CanvasLayerUpdate();
            }
        }
        [UIValue("GameHMDOnly")]
        public bool GameHMDOnly
        {
            get => PluginConfig.Instance.GameHMDOnly;
            set => PluginConfig.Instance.GameHMDOnly = value;
        }
        [UIValue("MenuScreenSize")]
        public int MenuScreenSize
        {
            get => (int)PluginConfig.Instance.MenuScreenSize;
            set
            {
                PluginConfig.Instance.MenuScreenSize = value;
                this._menuViewController.ScreenSizeChange();
            }
        }
        [UIValue("GameScreenSize")]
        public int GameScreenSize
        {
            get => (int)PluginConfig.Instance.GameScreenSize;
            set
            {
                PluginConfig.Instance.GameScreenSize = value;
                this._menuViewController.ScreenSizeChange();
            }
        }
        [UIValue("ScreenSizeX")]
        public float ScreenSizeX
        {
            get => PluginConfig.Instance.ScreenSizeX;
            set
            {
                PluginConfig.Instance.ScreenSizeX = value;
                this._menuViewController.ScreenSizeChange();
            }
        }
        [UIValue("ScreenSizeY")]
        public float ScreenSizeY
        {
            get => PluginConfig.Instance.ScreenSizeY;
            set
            {
                PluginConfig.Instance.ScreenSizeY = value;
                this._menuViewController.ScreenSizeChange();
            }
        }
        [UIValue("TimeFontSize")]
        public float TimeFontSize
        {
            get => PluginConfig.Instance.TimeFontSize;
            set
            {
                PluginConfig.Instance.TimeFontSize = value;
                this._menuViewController.ScreenSizeChange();
            }
        }
        [UIValue("TimerFontSize")]
        public float TimerFontSize
        {
            get => PluginConfig.Instance.TimerFontSize;
            set
            {
                PluginConfig.Instance.TimerFontSize = value;
                this._menuViewController.ScreenSizeChange();
            }
        }
        [UIValue("AlarmStopButtonSize")]
        public float AlarmStopButtonSize
        {
            get => PluginConfig.Instance.AlarmStopButtonSize;
            set
            {
                PluginConfig.Instance.AlarmStopButtonSize = value;
                this._menuViewController.ScreenSizeChange();
            }
        }
        [UIAction("AlarmON")]
        public void AlarmON()
        {
            PluginConfig.Instance.AlarmEnabled = true;
            this.AlarmStatusSet();
            this._bsAlarmClockController.AlarmSet();
        }
        [UIAction("AlarmOFF")]
        public void AlarmOFF()
        {
            PluginConfig.Instance.AlarmEnabled = false;
            this.AlarmStatusSet();
            this._menuViewController._alarmStopButton.gameObject.SetActive(false);
            this._bsAlarmClockController.AlarmSet();
        }
        [UIAction("AlarmTest")]
        public void AlarmTest()
        {
            this._menuViewController.AlarmSoundPlay();
        }
        [UIAction("TimerSet")]
        public void TimerSet()
        {
            var a = DateTime.Now.AddHours(this._timerHour).AddMinutes(this._timerMin);
            this.AlarmHour = a.Hour;
            this.AlarmMin = a.Minute;
        }
        [UIAction("ResetMenuPosition")]
        public void ResetMenuPosition()
        {
            PluginConfig.Instance.MenuScreenPosX = PluginConfig.DefaultMenuScreenPosX;
            PluginConfig.Instance.MenuScreenPosY = PluginConfig.DefaultMenuScreenPosY;
            PluginConfig.Instance.MenuScreenPosZ = PluginConfig.DefaultMenuScreenPosZ;
            PluginConfig.Instance.MenuScreenRotX = 0;
            PluginConfig.Instance.MenuScreenRotY = 0;
            PluginConfig.Instance.MenuScreenRotZ = 0;
            this._menuViewController.ScreenSizeChange();
        }
        [UIAction("ResetGamePosition")]
        public void ResetGamePosition()
        {
            PluginConfig.Instance.GameScreenPosX = PluginConfig.DefaultGameScreenPosX;
            PluginConfig.Instance.GameScreenPosY = PluginConfig.DefaultGameScreenPosY;
            PluginConfig.Instance.GameScreenPosZ = PluginConfig.DefaultGameScreenPosZ;
            PluginConfig.Instance.GameScreenRotX = 0;
            PluginConfig.Instance.GameScreenRotY = 0;
            PluginConfig.Instance.GameScreenRotY = 0;
        }
    }
}
