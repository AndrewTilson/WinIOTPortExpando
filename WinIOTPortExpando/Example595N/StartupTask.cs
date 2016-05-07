using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using WinIOTPortExpando.ShiftRegister;
using System.Threading.Tasks;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Example595N
{
    public sealed class StartupTask : IBackgroundTask
    {
        //Use this to specify additional registers chained together.
        static int registerqt = 2;

        //Pass in the pins that you have the register hooked up with.
        Shift595N shift = new Shift595N(22, 27, 18, registerqt);

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            int pins = registerqt * 8;
            int speed = 25;
            for (int loop = 0; loop < 2; loop++)
            {
                for (int pin = 1; pin <= pins; pin++)
                {
                    shift.putEnabled(pin, true);
                    Task.Delay(speed).Wait();
                }
                for (int pin = pins; pin >= 1; pin--)
                {
                    shift.putEnabled(pin, false);
                    Task.Delay(speed).Wait();
                }
            }

            for (int loop = 0; loop < 2; loop++)
            {
                for (int pin = 1; pin <= pins; pin++)
                {
                    shift.putEnabled(pin, true);
                    Task.Delay(speed).Wait();
                }
                for (int pin = 1; pin <= pins; pin++)
                {
                    shift.putEnabled(pin, false);
                    Task.Delay(speed).Wait();
                }
            }

            for (int loop = 0; loop < 4; loop++)
            {
                shift.allEnabled(true);
                Task.Delay(speed).Wait();
                shift.allEnabled(false);
                Task.Delay(speed).Wait();
            }
        }
    }
}
