﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Gaming.Input;

namespace XboxWheelCompatibility.WheelTransformer
{
    public class WheelManager
    {
        private static readonly object WheelListLock = new();
        public static readonly List<RacingWheel> ActiveWheels = new();
        public static event EventHandler<RacingWheel?>? MainWheelChanged;
        public static RacingWheel? MainWheel = null;

        public static void UpdateWheels()
        {
            lock (WheelListLock)
            {

                foreach (var OldWheel in ActiveWheels.ToList())
                {
                    if (RacingWheel.RacingWheels.Contains(OldWheel)) continue;

                    ActiveWheels.Remove(OldWheel);
                }

                foreach (var NewWheel in RacingWheel.RacingWheels)
                {
                    if (ActiveWheels.Contains(NewWheel)) continue;

                    ActiveWheels.Add(NewWheel);
                }

                MainWheel = ActiveWheels.Count > 0 ? ActiveWheels.Last() : null;

                if (MainWheelChanged == null) return;
                MainWheelChanged.Invoke(null, MainWheel);
            }
        }
        public static bool IsFerrari458SpiderWheel(RacingWheel wheel)
{
    return wheel.HardwareProductId == 0xB671 &&
           wheel.HardwareVendorId == 0x044F &&
           wheel.HardwareInstanceId == "USB\\VID_044F&PID_B671\\00001382FE1E5F29";
}
        private static void ListenForWheelChanges()
        {
            LifecycleManager.Tick += (object? Sender, EventArgs Event) =>
            {
                if(RacingWheel.RacingWheels.Count != ActiveWheels.Count)
                {
                    UpdateWheels();
                }
            };
        }

        public static void Initialize()
        {
            ListenForWheelChanges();
        }
    }
}
