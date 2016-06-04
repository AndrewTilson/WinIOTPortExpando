using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using WinIOTPortExpando.MCPBase;
using System.Threading.Tasks;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace MCP23017InAndOut
{
    public sealed class StartupTask : IBackgroundTask
    {
        MCP23017 register1 = new MCP23017(0x20, 4);
        List<Pin> allpins = new List<Pin>();

        Pin led0 = new Pin { register = PinOpt.register.A, pin = PinOpt.pin.GP0, IO = PinOpt.IO.output };
        Pin led1 = new Pin { register = PinOpt.register.A, pin = PinOpt.pin.GP1, IO = PinOpt.IO.output };
        Pin led2 = new Pin { register = PinOpt.register.A, pin = PinOpt.pin.GP2, IO = PinOpt.IO.output };
        Pin led3 = new Pin { register = PinOpt.register.A, pin = PinOpt.pin.GP3, IO = PinOpt.IO.output };
        Pin led4 = new Pin { register = PinOpt.register.A, pin = PinOpt.pin.GP4, IO = PinOpt.IO.output };
        Pin led5 = new Pin { register = PinOpt.register.A, pin = PinOpt.pin.GP5, IO = PinOpt.IO.output };
        Pin led6 = new Pin { register = PinOpt.register.A, pin = PinOpt.pin.GP6, IO = PinOpt.IO.output };
        Pin led7 = new Pin { register = PinOpt.register.A, pin = PinOpt.pin.GP7, IO = PinOpt.IO.output };
        Pin sw0 = new Pin { register = PinOpt.register.B, pin = PinOpt.pin.GP0, IO = PinOpt.IO.input };
        Pin sw1 = new Pin { register = PinOpt.register.B, pin = PinOpt.pin.GP1, IO = PinOpt.IO.input };
        Pin sw2 = new Pin { register = PinOpt.register.B, pin = PinOpt.pin.GP2, IO = PinOpt.IO.input };
        Pin sw3 = new Pin { register = PinOpt.register.B, pin = PinOpt.pin.GP3, IO = PinOpt.IO.input };
        Pin sw4 = new Pin { register = PinOpt.register.B, pin = PinOpt.pin.GP4, IO = PinOpt.IO.input };
        Pin sw5 = new Pin { register = PinOpt.register.B, pin = PinOpt.pin.GP5, IO = PinOpt.IO.input };
        Pin sw6 = new Pin { register = PinOpt.register.B, pin = PinOpt.pin.GP6, IO = PinOpt.IO.input };
        Pin sw7 = new Pin { register = PinOpt.register.B, pin = PinOpt.pin.GP7, IO = PinOpt.IO.input };

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            try
            {
                sw0.OnChange += sw0change;
                sw1.OnChange += sw1change;
                sw2.OnChange += sw2change;
                sw3.OnChange += sw3change;
                sw4.OnChange += sw4change;
                sw5.OnChange += sw5change;
                sw6.OnChange += sw6change;
                sw7.OnChange += sw7change;

                allpins = new List<Pin> { led0, led1, led2, led3, led4, led5, led6, led7, sw0, sw1, sw2, sw3, sw4, sw5, sw6, sw7 };
                register1.addpins(allpins);
                register1.init();

                do
                {
                    await Task.Delay(15000);
                } while (true);
            }
            finally
            {
                deferral.Complete();
            }
        }

        private void sw7change(Pin m)
        {
            register1.PutOutputPinEnabled(led7, register1.getEnabled(m));
        }

        private void sw6change(Pin m)
        {
            register1.PutOutputPinEnabled(led6, register1.getEnabled(m));
        }

        private void sw5change(Pin m)
        {
            register1.PutOutputPinEnabled(led5, register1.getEnabled(m));
        }

        private void sw4change(Pin m)
        {
            register1.PutOutputPinEnabled(led4, register1.getEnabled(m));
        }

        private void sw3change(Pin m)
        {
            register1.PutOutputPinEnabled(led3, register1.getEnabled(m));
        }

        private void sw2change(Pin m)
        {
            register1.PutOutputPinEnabled(led2, register1.getEnabled(m));
        }

        private void sw1change(Pin m)
        {
            register1.PutOutputPinEnabled(led1, register1.getEnabled(m));
        }

        private void sw0change(Pin m)
        {
            register1.PutOutputPinEnabled(led0, register1.getEnabled(m));
        }
    }
}
