using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using GPIOAccess;
using System.Threading.Tasks;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace MCP23017
{
    public sealed class StartupTask : IBackgroundTask
    {
        I2CPortExpander i2c = new I2CPortExpander(0x20);
        //I2CPortExpander i2c2 = new I2CPortExpander(0x27);

        I2CPortExpander.Pins pin0 = I2CPortExpander.Pins.GP0;
        I2CPortExpander.Pins pin1 = I2CPortExpander.Pins.GP1;
        I2CPortExpander.Pins pin2 = I2CPortExpander.Pins.GP2;
        I2CPortExpander.Pins pin3 = I2CPortExpander.Pins.GP3;
        I2CPortExpander.Pins pin4 = I2CPortExpander.Pins.GP4;
        I2CPortExpander.Pins pin5 = I2CPortExpander.Pins.GP5;
        I2CPortExpander.Pins pin6 = I2CPortExpander.Pins.GP6;
        I2CPortExpander.Pins pin7 = I2CPortExpander.Pins.GP7;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            try
            {
                i2c.SetOutputPins(pin0, pin1, pin2, pin3, pin4, pin5, pin6, pin7);
                //i2c2.SetOutputPins(pin0, pin1, pin2, pin3, pin4, pin5, pin6, pin7);

                //Force process to not end until app closure.
                do
                {
                    moveleds();
                    await Task.Delay(10000);
                } while (true);
            }
            finally
            {
                deferral.Complete();
            }
        }

        public async void moveleds()
        {
            List<I2CPortExpander.Pins> pins = new List<I2CPortExpander.Pins> { pin0, pin1, pin2, pin3, pin4, pin5, pin6, pin7 };

            int speed = 25;

            i2c.PutAllOutputPinsEnabled(true);

            for (int loop = 0; loop < 10; loop++)
            {
                foreach (I2CPortExpander.Pins pin in pins)
                {
                    i2c.PutOutputPinEnabled(true, pin);
                    await Task.Delay(speed);
                }
                foreach (I2CPortExpander.Pins pin in pins)
                {
                    i2c.PutOutputPinEnabled(false, pin);
                    await Task.Delay(speed);
                }
            }
        }
    }
}
