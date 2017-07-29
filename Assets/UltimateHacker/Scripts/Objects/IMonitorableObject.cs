using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimateHacker.Objects
{
    public interface IMonitorableObject
    {
        void Restart();
        bool IsOnline();
    }
}
