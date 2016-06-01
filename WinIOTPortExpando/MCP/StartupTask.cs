using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using WinIOTPortExpando.MCPBase;
using System.Threading.Tasks;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace MCP
{
    public sealed class StartupTask : IBackgroundTask
    {
        MCP23017 register1 = new MCP23017(0x20);
        List<Pin> allpins = new List<Pin>();

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            try
            {
                Pin pin0 = new Pin { bank = PinOpt.bank.A, pin = PinOpt.pin.GP0, IO = PinOpt.IO.output };
                Pin pin1 = new Pin { bank = PinOpt.bank.A, pin = PinOpt.pin.GP1, IO = PinOpt.IO.output };
                Pin pin2 = new Pin { bank = PinOpt.bank.A, pin = PinOpt.pin.GP2, IO = PinOpt.IO.output };
                Pin pin3 = new Pin { bank = PinOpt.bank.A, pin = PinOpt.pin.GP3, IO = PinOpt.IO.output };
                Pin pin4 = new Pin { bank = PinOpt.bank.A, pin = PinOpt.pin.GP4, IO = PinOpt.IO.output };
                Pin pin5 = new Pin { bank = PinOpt.bank.A, pin = PinOpt.pin.GP5, IO = PinOpt.IO.output };
                Pin pin6 = new Pin { bank = PinOpt.bank.A, pin = PinOpt.pin.GP6, IO = PinOpt.IO.output };
                Pin pin7 = new Pin { bank = PinOpt.bank.A, pin = PinOpt.pin.GP7, IO = PinOpt.IO.output };
                Pin pin8 = new Pin { bank = PinOpt.bank.B, pin = PinOpt.pin.GP0, IO = PinOpt.IO.output };
                Pin pin9 = new Pin { bank = PinOpt.bank.B, pin = PinOpt.pin.GP1, IO = PinOpt.IO.output };
                Pin pin10 = new Pin { bank = PinOpt.bank.B, pin = PinOpt.pin.GP2, IO = PinOpt.IO.output };
                Pin pin11 = new Pin { bank = PinOpt.bank.B, pin = PinOpt.pin.GP3, IO = PinOpt.IO.output };
                Pin pin12 = new Pin { bank = PinOpt.bank.B, pin = PinOpt.pin.GP4, IO = PinOpt.IO.output };
                Pin pin13 = new Pin { bank = PinOpt.bank.B, pin = PinOpt.pin.GP5, IO = PinOpt.IO.output };
                Pin pin14 = new Pin { bank = PinOpt.bank.B, pin = PinOpt.pin.GP6, IO = PinOpt.IO.output };
                Pin pin15 = new Pin { bank = PinOpt.bank.B, pin = PinOpt.pin.GP7, IO = PinOpt.IO.output };

                allpins = new List<Pin> { pin0, pin1, pin2, pin3, pin4, pin5, pin6, pin7, pin8, pin9, pin10, pin11, pin12, pin13, pin14, pin15 };
                register1.addpins(allpins);
                register1.init();

                do
                {
                    moveleds();
                    await Task.Delay(15000);
                } while (true);
            }
            finally
            {
                deferral.Complete();
            }
        }

        public async void moveleds()
        {
            int speed = 25;

            register1.PutAllOutputPinsEnabled(true);

            for (int loop = 0; loop < 10; loop++)
            {
                foreach (Pin pin in allpins)
                {
                    register1.PutOutputPinEnabled(pin, true);
                    await Task.Delay(speed);
                }
                foreach (Pin pin in allpins)
                {
                    register1.PutOutputPinEnabled(pin, false);
                    await Task.Delay(speed);
                }
            }
        }
    }
}
