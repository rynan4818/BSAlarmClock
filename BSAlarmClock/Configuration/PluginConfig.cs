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
        public const float DefaultGameScreenPosX = 0f;
        public const float DefaultGameScreenPosY = 2.7f;
        public const float DefaultGameScreenPosZ = 1.0f;
        public const float DefaultMenuScreenPosX = 0f;
        public const float DefaultMenuScreenPosY = 2.7f;
        public const float DefaultMenuScreenPosZ = 1.0f;

        public static PluginConfig Instance { get; set; }

        public virtual bool Enable { get; set; } = true;
        public virtual bool LockPosition { get; set; } = false;
        public virtual bool HMDOnly { get; set; } = false;
        public virtual float GameScreenPosX { get; set; } = DefaultGameScreenPosX;
        public virtual float GameScreenPosY { get; set; } = DefaultGameScreenPosY;
        public virtual float GameScreenPosZ { get; set; } = DefaultGameScreenPosZ;
        public virtual float GameScreenRotX { get; set; } = 0;
        public virtual float GameScreenRotY { get; set; } = 0;
        public virtual float GameScreenRotZ { get; set; } = 0;
        public virtual float GameScreenSize { get; set; } = 40f;
        public virtual float MenuScreenPosX { get; set; } = DefaultMenuScreenPosX;
        public virtual float MenuScreenPosY { get; set; } = DefaultMenuScreenPosY;
        public virtual float MenuScreenPosZ { get; set; } = DefaultMenuScreenPosZ;
        public virtual float MenuScreenRotX { get; set; } = 0;
        public virtual float MenuScreenRotY { get; set; } = 0;
        public virtual float MenuScreenRotZ { get; set; } = 0;
        public virtual float MenuScreenSize { get; set; } = 40f;
        public virtual int GameUiSortingOrder {  get; set; } = 31;
        public virtual int MenuUiSortingOrder { get; set; } = 3;
        [UseConverter(typeof(EnumConverter<VisibilityLayer>))]
        public virtual VisibilityLayer DefaultLayer { get; set; } = VisibilityLayer.UI;
        [UseConverter(typeof(EnumConverter<VisibilityLayer>))]
        public virtual VisibilityLayer HMDOnlyLayer { get; set; } = VisibilityLayer.HmdOnlyAndReflected;

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
}
