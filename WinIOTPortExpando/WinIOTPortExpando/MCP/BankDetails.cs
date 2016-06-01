using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace WinIOTPortExpando.MCPBase
{
    internal class BankDetails
    {
        internal byte PORT_EXPANDER_IODIR_REGISTER_ADDRESS { get; set; }
        internal byte PORT_EXPANDER_GPIO_REGISTER_ADDRESS { get; set; }
        internal byte PORT_EXPANDER_OLAT_REGISTER_ADDRESS { get; set; }

        internal byte iodirRegister { get; set; } // copy of I2C Port Expander IODIR register
        internal byte gpioRegister { get; set; } // copy of I2C Port Expander GPIO register
        internal byte olatRegister { get; set; } // copy of I2C Port Expander OLAT register

        private byte[] i2CReadBuffer;

        internal void init(I2cDevice i2c)
        {
            try
            {
                //initialize local copies of the IODIR, GPIO, and OLAT registers
                i2CReadBuffer = new byte[1];

                //read each register
                i2c.WriteRead(new byte[] { PORT_EXPANDER_IODIR_REGISTER_ADDRESS }, i2CReadBuffer);
                iodirRegister = i2CReadBuffer[0];

                i2c.WriteRead(new byte[] { PORT_EXPANDER_GPIO_REGISTER_ADDRESS }, i2CReadBuffer);
                gpioRegister = i2CReadBuffer[0];

                i2c.WriteRead(new byte[] { PORT_EXPANDER_OLAT_REGISTER_ADDRESS }, i2CReadBuffer);
                olatRegister = i2CReadBuffer[0];
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception: {0}", e.Message);
                return;
            }
        }
    }
}
