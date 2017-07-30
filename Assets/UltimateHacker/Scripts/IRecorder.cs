using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimateHacker
{
    public interface IRecorder
    {
        List<string> CommandRecord(float time);
        List<string> CommandGetRecording();
        List<string> CommandPutRecording(float time);
        List<string> CommandDeleteRecording();
    }
}
