using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace WinIOTPortExpando.MCPBase
{
    internal class Register
    {
        internal PinOpt.register register { get; set; }

        internal byte PORT_EXPANDER_IODIR_REGISTER_ADDRESS { get; set; } // IODIR register controls the direction of the GPIO on the port expander
        internal byte PORT_EXPANDER_IPOL_REGISTER_ADDRESS { get; set; } // Input Polarity Register
        internal byte PORT_EXPANDER_GPINT_REGISTER_ADDRESS { get; set; } // Interrput on Change pin
        internal byte PORT_EXPANDER_DEFVAL_REGISTER_ADDRESS { get; set; } // Default Compare Register for Interrupt-on-change
        internal byte PORT_EXPANDER_INTCON_REGISTER_ADDRESS { get; set; } // Interrupt Control Register
        internal byte PORT_EXPANDER_GPPU_REGISTER_ADDRESS { get; set; } // GPIO Pull-up Resistor Register
        internal byte PORT_EXPANDER_INTF_REGISTER_ADDRESS { get; set; } //Interrupt Flag Register
        internal byte PORT_EXPANDER_INTCAP_REGISTER_ADDRESS { get; set; } //Interrupt Capture Register
        internal byte PORT_EXPANDER_GPIO_REGISTER_ADDRESS { get; set; } // GPIO register is used to read the pins input
        internal byte PORT_EXPANDER_OLAT_REGISTER_ADDRESS { get; set; } // Output Latch register is used to set the pins output high/low
        internal byte PORT_EXPANDER_IOCON_REGISTER_ADDRESS { get; set; } // I/O Expander Configruation Register

        internal byte iodirRegister { get; set; } // copy of I2C Port Expander IODIR register
        internal byte gpioRegister { get; set; } // copy of I2C Port Expander GPIO register
        internal byte olatRegister { get; set; } // copy of I2C Port Expander OLAT register
        internal byte gpintRegister { get; set; } // copy of I2C Port Expander GPINT register
        internal byte intconRegister { get; set; } // copy of I2C Port Expander INTCON register
        internal byte intfRegister { get; set; } // copy of I2C Port Expander INTF register
        internal byte gppuRegister { get; set; } // copy of I2C Port Expander INTF register
        internal byte ioconRegister { get; set; } //local copy of the I2C Port Expander IOCON register

        private byte[] i2CWriteBuffer;
        private byte[] i2CReadBuffer;

        internal void init(I2cDevice i2c)
        {
            try
            {
                GetRegisterValues(i2c);

                //Always leaving pullup resistor disabled as testing shows this to be unstable.
                i2CWriteBuffer = new byte[] { PORT_EXPANDER_GPPU_REGISTER_ADDRESS, 0x00 };
                i2c.Write(i2CWriteBuffer);

                //Set all to outputs as defaults
                i2CWriteBuffer = new byte[] { PORT_EXPANDER_IODIR_REGISTER_ADDRESS, 0x00 };
                i2c.Write(i2CWriteBuffer);

                //reset input polarity
                i2CWriteBuffer = new byte[] { PORT_EXPANDER_IPOL_REGISTER_ADDRESS, 0x00 };
                i2c.Write(i2CWriteBuffer);

                //configure device for open - drain output on the interrupt pin
                i2c.Write(new byte[] { PORT_EXPANDER_IOCON_REGISTER_ADDRESS, 0x00 });

                i2c.WriteRead(new byte[] { PORT_EXPANDER_IOCON_REGISTER_ADDRESS }, i2CReadBuffer);
                ioconRegister = i2CReadBuffer[0];
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to initialize register: " + ex.Message);
            }
            return;
        }

        internal void GetRegisterValues(I2cDevice i2c)
        {
            // initialize local copies of the registers
            i2CReadBuffer = new byte[1];

            i2c.WriteRead(new byte[] { PORT_EXPANDER_IODIR_REGISTER_ADDRESS }, i2CReadBuffer);
            iodirRegister = i2CReadBuffer[0];

            i2c.WriteRead(new byte[] { PORT_EXPANDER_GPIO_REGISTER_ADDRESS }, i2CReadBuffer);
            gpioRegister = i2CReadBuffer[0];

            i2c.WriteRead(new byte[] { PORT_EXPANDER_OLAT_REGISTER_ADDRESS }, i2CReadBuffer);
            olatRegister = i2CReadBuffer[0];

            i2c.WriteRead(new byte[] { PORT_EXPANDER_GPINT_REGISTER_ADDRESS }, i2CReadBuffer);
            gpintRegister = i2CReadBuffer[0];

            i2c.WriteRead(new byte[] { PORT_EXPANDER_INTCON_REGISTER_ADDRESS }, i2CReadBuffer);
            intconRegister = i2CReadBuffer[0];

            i2c.WriteRead(new byte[] { PORT_EXPANDER_INTF_REGISTER_ADDRESS }, i2CReadBuffer);
            intfRegister = i2CReadBuffer[0];
        }
    }
}
