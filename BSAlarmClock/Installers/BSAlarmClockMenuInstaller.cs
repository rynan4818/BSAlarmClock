using BSAlarmClock.Views;
using Zenject;

namespace BSAlarmClock.Installers
{
    public class BSAlarmClockMenuInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<SettingTabViewController>().FromNewComponentAsViewController().AsSingle().NonLazy();
            this.Container.BindInterfacesAndSelfTo<MenuViewController>().FromNewComponentAsViewController().AsSingle().NonLazy();
        }
    }
}
