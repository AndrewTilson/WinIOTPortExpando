using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using WinIOTPortExpando;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace ExampleHD44780U
{
    public sealed class StartupTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            //This example has not been fully tested yet. Turns out the lcd screen i have on hand was bad so until i have one to test with i cannot verify this is working.
            var deferral = taskInstance.GetDeferral();
            try
            {
                HD44780U display = new HD44780U(0x20, HD44780U.PortExpander.MCP23008, WinIOTPortExpando.MCPBase.PinOpt.register.A);
                display.PrintLine("HELLO!");
            }
            finally
            {
                deferral.Complete();
            }
        }
    }
}
