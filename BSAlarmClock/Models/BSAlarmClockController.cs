using BSAlarmClock.Configuration;
using System;
using UnityEngine;
using Zenject;

namespace BSAlarmClock.Models
{
	public class BSAlarmClockController : IInitializable, ITickable
    {
        public float _currentCycleTime;
        public DateTime _alarmTime;
        public event Action<string, string> _timeUpdated;
        public event Action _alarmPing;
        public bool _gamePlayActive;
        public string _timeFormat;

        public void Initialize()
        {
            this._currentCycleTime = 0f;
            this.AlarmSet();
            this._timeFormat = PluginConfig.ShortTimeString;
            foreach (var foramt in PluginConfig.Instance.TimeFormat)
            {
                if (foramt.Format == PluginConfig.Instance.TimeFormatSelect)
                {
                    this._timeFormat = foramt.Format;
                    break;
                }
            }
        }
        public void Tick()
        {
            if (this._currentCycleTime >= PluginConfig.Instance.CycleLength)
            {
                this._currentCycleTime = 0f;
                this.TimeUpdate();
            }
            this._currentCycleTime += Time.deltaTime;
        }
        public void TimeUpdate()
        {
            var timer = "";
            if (PluginConfig.Instance.AlarmEnabled)
            {
                this.CheckAlarm();
                if (DateTime.Now <= this._alarmTime)
                    timer = (this._alarmTime - DateTime.Now).ToString(@"hh\:mm\:ss");
                else
                    timer = "00:00:00";
            }
            this._timeUpdated?.Invoke(this.ToTimeFormatString(DateTime.Now), timer);
        }
        public void CheckAlarm()
        {
            if(DateTime.Now > this._alarmTime)
            {
                this._alarmPing?.Invoke();
            }
        }
        public void AlarmSet()
        {
            var alarm = DateTime.Today.AddHours(PluginConfig.Instance.AlarmHour).AddMinutes(PluginConfig.Instance.AlarmMin);
            if (DateTime.Now > alarm)
                alarm = DateTime.Today.AddDays(1).AddHours(PluginConfig.Instance.AlarmHour).AddMinutes(PluginConfig.Instance.AlarmMin);
            this._alarmTime = alarm;
            this.TimeUpdate();
        }
        public string ToTimeFormatString(DateTime time)
        {
            switch (this._timeFormat)
            {
                case PluginConfig.ShortTimeString:
                    return time.ToShortTimeString();
                case PluginConfig.LongTimeString:
                    return time.ToLongTimeString();
                default:
                    try
                    {
                        return time.ToString(this._timeFormat);
                    }
                    catch (FormatException)
                    {
                        return time.ToShortDateString();
                    }
            }
        }
    }
}
