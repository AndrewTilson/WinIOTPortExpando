using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using WinIOTPortExpando;
using WinIOTPortExpando.MCPBase;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace ExampleHD44780U
{
    public sealed class StartupTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            //This example has not been fully tested yet. Turns out the lcd screen i have on hand was bad so until i have one to test with i cannot verify this is working.
            var deferral = taskInstance.GetDeferral();
            try
            {
                //create port expander IC and its proper pins for the screen
                MCP23008 register = new MCP23008(0x20, 26);
                List<Pin> allpins = new List<Pin>();

                Pin rs = new Pin { register = PinOpt.register.A, pin = PinOpt.pin.GP0, IO = PinOpt.IO.output };
                Pin e = new Pin { register = PinOpt.register.A, pin = PinOpt.pin.GP1, IO = PinOpt.IO.output };
                Pin d4 = new Pin { register = PinOpt.register.A, pin = PinOpt.pin.GP2, IO = PinOpt.IO.output };
                Pin d5 = new Pin { register = PinOpt.register.A, pin = PinOpt.pin.GP3, IO = PinOpt.IO.output };
                Pin d6 = new Pin { register = PinOpt.register.A, pin = PinOpt.pin.GP4, IO = PinOpt.IO.output };
                Pin d7 = new Pin { register = PinOpt.register.A, pin = PinOpt.pin.GP5, IO = PinOpt.IO.output };

                allpins = new List<Pin> { rs, e, d4, d5, d6, d7 };
                register.addpins(allpins);
                register.init();

                //create the lcd screen object and provide it the expander and the pins
                HD44780U HD44780U;
                HD44780U = new HD44780U(register, 20, 2);
                await HD44780U.InitAsync(rs, e, d4, d5, d6, d7);
                await HD44780U.clearAsync();

                //write out to the lcd
                HD44780U.setCursor(0, 0);
                HD44780U.write("Windows 10 IoT");
                while (true)
                {
                    HD44780U.setCursor(0, 1);
                    HD44780U.write(DateTime.Now.ToString("hh:mm:ss:fff tt", System.Globalization.DateTimeFormatInfo.InvariantInfo));
                }
            }
            finally
            {
                deferral.Complete();
            }
        }
    }
}
