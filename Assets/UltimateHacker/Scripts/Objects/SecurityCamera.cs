using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimateHacker.Objects
{
    public class SecurityCamera : IMonitorableObject
    {
        public bool Online
        { get; private set; }

        public SecurityCameraSource Source;

        private ObjectMonitor _monitor;

        public void Restart()
        {
            this.Online = true;
        }

        public bool IsOnline()
        {
            return this.Online;
        }
    }
}
