using BSAlarmClock.Views;
using Zenject;

namespace BSAlarmClock.Installers
{
    public class BSAlarmClockPlayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<PlayerViewController>().FromNewComponentAsViewController().AsCached().NonLazy();
        }
    }
}
