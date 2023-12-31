﻿using BSAlarmClock.Models;
using Zenject;

namespace BSAlarmClock.Installers
{
    public class BSAlarmClockAppInstaller : Installer
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<BSAlarmClockController>().AsSingle().NonLazy();
            this.Container.BindInterfacesAndSelfTo<AlarmSoundController>().AsSingle().NonLazy();
        }
    }
}
