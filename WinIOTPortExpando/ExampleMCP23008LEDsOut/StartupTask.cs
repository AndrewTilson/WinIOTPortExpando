﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using WinIOTPortExpando.MCPBase;
using System.Threading.Tasks;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace MCP23008LEDsOut
{
    public sealed class StartupTask : IBackgroundTask
    {
        MCP23008 register = new MCP23008(0x20, 26);
        List<Pin> allpins = new List<Pin>();

        Pin pin0 = new Pin { register = PinOpt.register.A, pin = PinOpt.pin.GP0, IO = PinOpt.IO.output };
        Pin pin1 = new Pin { register = PinOpt.register.A, pin = PinOpt.pin.GP1, IO = PinOpt.IO.output };
        Pin pin2 = new Pin { register = PinOpt.register.A, pin = PinOpt.pin.GP2, IO = PinOpt.IO.output };
        Pin pin3 = new Pin { register = PinOpt.register.A, pin = PinOpt.pin.GP3, IO = PinOpt.IO.output };
        Pin pin4 = new Pin { register = PinOpt.register.A, pin = PinOpt.pin.GP4, IO = PinOpt.IO.output };
        Pin pin5 = new Pin { register = PinOpt.register.A, pin = PinOpt.pin.GP5, IO = PinOpt.IO.output };
        Pin pin6 = new Pin { register = PinOpt.register.A, pin = PinOpt.pin.GP6, IO = PinOpt.IO.output };
        Pin pin7 = new Pin { register = PinOpt.register.A, pin = PinOpt.pin.GP7, IO = PinOpt.IO.output };


        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            try
            {
                //add all pins to register object. This is required for methods in the object to function.
                allpins = new List<Pin> { pin0, pin1, pin2, pin3, pin4, pin5, pin6, pin7 };
                register.addpins(allpins);
                register.init();

                do
                {
                    Task rotate = moveleds();
                    rotate.Wait();
                } while (true);
            }
            finally
            {
                deferral.Complete();
            }
        }

        internal async Task moveleds()
        {
            int speed = 25;

            register.PutAllOutputPinsEnabled(false);


            foreach (Pin pin in allpins.Where(p => p.IO == PinOpt.IO.output))
            {
                register.PutOutputPinEnabled(pin, false);
                await Task.Delay(speed);
                register.PutOutputPinEnabled(pin, true);
                await Task.Delay(speed);
            }

            register.PutAllOutputPinsEnabled(true);
            await Task.Delay(speed);
        }
    }
}
