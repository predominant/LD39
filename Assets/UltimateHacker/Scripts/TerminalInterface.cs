using System.Collections.Generic;
using UnityEngine;

namespace UltimateHacker
{
    public class TerminalInterface : MonoBehaviour
    {
        public ICommandTarget CommandTarget;

        public bool CanStatusCheck;
        public bool CanPoweroff;
        public bool CanRestart;
        public bool CanSetPollingTime;
        public bool CanRecord;
        public bool CanPlayback;
        public bool CanGetRecording;
        public bool CanPutRecording;

        public bool HasMonitor;

        public string Description;

        public List<string> CommandFeatures()
        {
            var output = new List<string>();

            output.Add(string.Format("<color=\"orange\">{0}</color> feature summary:"));
            output.Add(this.Description);

            if (this.HasMonitor)
                output.Add("NOTE: This device has been linked to a configuration monitor");

            return output;
        } 

        public List<string> CommandFunctions()
        {
            var output = new List<string>();
            output.Add(string.Format("<color=\"orange\">{0}</color> Function list:", this.gameObject.name));

            if (this.CanRestart)
                output.Add("  restart            - Restart the device");
            if (this.CanPoweroff)
                output.Add("  poweroff           - Shutdown the device");
            if (this.CanSetPollingTime)
                output.Add("  polltime <seconds> - Set the device polling time");
            if (this.CanRecord)
                output.Add("  record             - Record sensor data from the device (10s)");
            if (this.CanPlayback)
                output.Add("  playback           - Playback sensor data to the device");
            if (this.CanGetRecording)
                output.Add("  getrecording       - Download sensor data from device");
            if (this.CanPutRecording)
                output.Add("  putrecording       - Upload sensor data recording to device");
                output.Add("                       Must have a recording resident in drone");
            return output;
        }
    }
}
