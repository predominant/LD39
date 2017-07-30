using System.Collections.Generic;

namespace UltimateHacker
{
    public interface ICommandTarget
    {
        List<string> CommandStatusCheck();
        List<string> ProcessCommand(string command);
        List<string> CommandNotFound(string command);

        List<string> CommandPoweroff();
        List<string> CommandRestart();
        List<string> CommandSetPollingTime(float time);
    }
}
