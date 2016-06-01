using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinIOTPortExpando.MCPBase
{
    public class MCP23017 : MCPBase
    {
        public MCP23017(byte I2CAddress = 0x20, int InteruptPin = 0)
        {
            PORT_EXPANDER_I2C_ADDRESS = I2CAddress;
            if (InteruptPin != 0)
            {
                interupt = new GPIOAccess.PinIn(InteruptPin);
            }
            else
            {
                interupt = null;
            }
        }

        public void init()
        {
            //add the 2 banks for a MCP23017
            banks.Add(new BankDetails { PORT_EXPANDER_IODIR_REGISTER_ADDRESS = 0x00, PORT_EXPANDER_GPIO_REGISTER_ADDRESS = 0x09, PORT_EXPANDER_OLAT_REGISTER_ADDRESS = 0x0A });
            banks.Add(new BankDetails { PORT_EXPANDER_IODIR_REGISTER_ADDRESS = 0x10, PORT_EXPANDER_GPIO_REGISTER_ADDRESS = 0x19, PORT_EXPANDER_OLAT_REGISTER_ADDRESS = 0x1A });

            //call initialization of base inherited class to initialize the i2c device
            var task = MCPinit();
            task.Wait();

            //set the IO for each pin that was created
            foreach (Pin pin in MCPpins)
            {
                //create temp bank and set it = to the bank the pin references.
                BankDetails tempbank = new BankDetails();

                if (pin.bank == PinOpt.bank.A) { tempbank = banks[0]; }
                else if (pin.bank == PinOpt.bank.B) { tempbank = banks[1]; }

                //determine if pin is input or output and set the bit as needed.
                if (pin.IO == PinOpt.IO.output)
                {
                    bitMask = (byte)(tempbank.iodirRegister ^ (byte)pin.pin);
                    tempbank.iodirRegister &= bitMask;
                }
                else if (pin.IO == PinOpt.IO.input)
                {
                    bitMask = (byte)(tempbank.iodirRegister |= (byte)pin.pin);
                    tempbank.iodirRegister &= bitMask;
                }

                //write out desired bits
                i2CWriteBuffer = new byte[] { tempbank.PORT_EXPANDER_IODIR_REGISTER_ADDRESS, tempbank.iodirRegister };
                i2cPortExpander.Write(i2CWriteBuffer);
            }
        }
    }

    public class MCP23008 : MCPBase
    {
        public MCP23008(byte I2CAddress = 0x20, int InteruptPin = 0)
        {
            PORT_EXPANDER_I2C_ADDRESS = I2CAddress;

            if (InteruptPin != 0)
            {
                interupt = new GPIOAccess.PinIn(InteruptPin);
            }
            else
            {
                interupt = null;
            }
        }

        public void init()
        {
            //call initialization of base inherited class to initialize the i2c device
            var task = MCPinit();
            task.Wait();

            //add the 2 banks for a MCP23017
            banks.Add(new BankDetails { PORT_EXPANDER_IODIR_REGISTER_ADDRESS = 0x00, PORT_EXPANDER_GPIO_REGISTER_ADDRESS = 0x09, PORT_EXPANDER_OLAT_REGISTER_ADDRESS = 0x0A });

            //set the IO for each pin that was created
            foreach (Pin pin in MCPpins)
            {
                //determine if pin is input or output and set the bit as needed.
                if (pin.IO == PinOpt.IO.output)
                {
                    bitMask = (byte)(banks[0].iodirRegister ^ (byte)pin.pin);
                    banks[0].iodirRegister &= bitMask;
                }
                else if (pin.IO == PinOpt.IO.input)
                {
                    bitMask = (byte)(banks[0].iodirRegister |= (byte)pin.pin);
                    banks[0].iodirRegister &= bitMask;
                }

                //write out desired bits
                i2CWriteBuffer = new byte[] { banks[0].PORT_EXPANDER_IODIR_REGISTER_ADDRESS, banks[0].iodirRegister };
                i2cPortExpander.Write(i2CWriteBuffer);
            }
        }
    }
}
