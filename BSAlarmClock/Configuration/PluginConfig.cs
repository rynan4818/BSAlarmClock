using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CameraUtils.Core;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace BSAlarmClock.Configuration
{
    internal class PluginConfig
    {
        public const float DefaultGameScreenPosX = -1.5f;
        public const float DefaultGameScreenPosY = 4.0f;
        public const float DefaultGameScreenPosZ = 6.7f;
        public const float DefaultMenuScreenPosX = -1.3f;
        public const float DefaultMenuScreenPosY = 3.3f;
        public const float DefaultMenuScreenPosZ = 4.2f;
        public const string ShortTimeString = "ShortTimeString";
        public const string LongTimeString = "LongTimeString";

        public static PluginConfig Instance { get; set; }

        public virtual bool MenuScreenHidden { get; set; } = false;
        public virtual bool GameScreenHidden { get; set; } = false;
        public virtual bool MenuLockPosition { get; set; } = false;
        public virtual bool GameLockPosition { get; set; } = false;
        public virtual bool MenuHMDOnly { get; set; } = false;
        public virtual bool GameHMDOnly { get; set; } = false;
        public virtual float GameScreenPosX { get; set; } = DefaultGameScreenPosX;
        public virtual float GameScreenPosY { get; set; } = DefaultGameScreenPosY;
        public virtual float GameScreenPosZ { get; set; } = DefaultGameScreenPosZ;
        public virtual float GameScreenRotX { get; set; } = 0;
        public virtual float GameScreenRotY { get; set; } = 0;
        public virtual float GameScreenRotZ { get; set; } = 0;
        public virtual float GameScreenSize { get; set; } = 15f;
        public virtual float MenuScreenPosX { get; set; } = DefaultMenuScreenPosX;
        public virtual float MenuScreenPosY { get; set; } = DefaultMenuScreenPosY;
        public virtual float MenuScreenPosZ { get; set; } = DefaultMenuScreenPosZ;
        public virtual float MenuScreenRotX { get; set; } = 0;
        public virtual float MenuScreenRotY { get; set; } = 0;
        public virtual float MenuScreenRotZ { get; set; } = 0;
        public virtual float MenuScreenSize { get; set; } = 11f;
        public virtual int GameUiSortingOrder { get; set; } = 31;
        public virtual int MenuUiSortingOrder { get; set; } = 3;
        public virtual float CycleLength { get; set; } = 1f;
        public virtual int AlarmHour { get; set; } = 0;
        public virtual int AlarmMin { get; set; } = 0;
        public virtual bool AlarmEnabled { get; set; } = false;
        public virtual bool AlarmSoundMenuOnly { get; set; } = false;
        public virtual bool AlarmSoundEnabled { get; set; } = true;
        public virtual string AlarmSound { get; set; } = "Alarm1.wav";
        public virtual float ScreenSizeX { get; set; } = 2.5f;
        public virtual float ScreenSizeY { get; set; } = 2.8f;
        public virtual float TimeFontSize { get; set; } = 1.2f;
        public virtual float TimerFontSize { get; set; } = 0.6f;
        public virtual float AlarmStopButtonSize { get; set; } = 0.45f;
        public virtual float AlarmVolume { get; set; } = 100f;
        [UseConverter(typeof(EnumConverter<VisibilityLayer>))]
        public virtual VisibilityLayer DefaultLayer { get; set; } = VisibilityLayer.UI;
        [UseConverter(typeof(EnumConverter<VisibilityLayer>))]
        public virtual VisibilityLayer HMDOnlyLayer { get; set; } = VisibilityLayer.HmdOnlyAndReflected;
        [UseConverter(typeof(ListConverter<TimeFormatSetting>))]
        public virtual List<TimeFormatSetting> TimeFormat { get; set; } = new List<TimeFormatSetting>()
        {
            new TimeFormatSetting()
            {
                Format = ShortTimeString,
                ScreenSizeX = 2.5f
            },
            new TimeFormatSetting()
            {
                Format = LongTimeString,
                ScreenSizeX = 4.0f
            },
            new TimeFormatSetting()
            {
                Format = "H:mm",
                ScreenSizeX = 2.5f
            },
            new TimeFormatSetting()
            {
                Format = "H:mm:ss",
                ScreenSizeX = 4.0f
            },
            new TimeFormatSetting()
            {
                Format = "h:mm tt",
                ScreenSizeX = 5.5f
            },
            new TimeFormatSetting()
            {
                Format = "h:mm:ss tt",
                ScreenSizeX = 6.5f
            },
            new TimeFormatSetting()
            {
                Format = "HH:mm",
                ScreenSizeX = 2.5f
            },
            new TimeFormatSetting()
            {
                Format = "HH:mm:ss",
                ScreenSizeX = 4.0f
            }
        };
        public virtual string TimeFormatSelect { get; set; } = ShortTimeString;
        /// <summary>
        /// これは、BSIPAが設定ファイルを読み込むたびに（ファイルの変更が検出されたときを含めて）呼び出されます
        /// </summary>
        public virtual void OnReload()
        {
            // 設定ファイルを読み込んだ後の処理を行う
        }

        /// <summary>
        /// これを呼び出すと、BSIPAに設定ファイルの更新を強制します。 これは、ファイルが変更されたことをBSIPAが検出した場合にも呼び出されます。
        /// </summary>
        public virtual void Changed()
        {
            // 設定が変更されたときに何かをします
        }

        /// <summary>
        /// これを呼び出して、BSIPAに値を<paramref name ="other"/>からこの構成にコピーさせます。
        /// </summary>
        public virtual void CopyFrom(PluginConfig other)
        {
            // このインスタンスのメンバーは他から移入されました
        }
    }
    public class TimeFormatSetting
    {
        [NonNullable]
        public virtual string Format { get; set; }
        [NonNullable]
        public virtual float ScreenSizeX { get; set; }
    }
}
