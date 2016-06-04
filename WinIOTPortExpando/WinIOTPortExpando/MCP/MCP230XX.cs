using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinIOTPortExpando.MCPBase
{
    public class MCP23017 : MCPBase
    {
        public MCP23017(byte I2CAddress = 0x20, int InteruptPinA = 0, int InteruptPinB = 0)
        {
            PORT_EXPANDER_I2C_ADDRESS = I2CAddress;
            if (InteruptPinA != 0)
            {
                interuptA = new GPIOAccess.PinIn(InteruptPinA, false);
            }
            else
            {
                interuptA = null;
            }
            if (InteruptPinB != 0)
            {
                interuptB = new GPIOAccess.PinIn(InteruptPinB, false);
            }
            else
            {
                interuptB = null;
            }
        }

        public void init()
        {
            //add the 2 banks for a MCP23017
            banks.Add(new BankDetails {
                PORT_EXPANDER_IODIR_REGISTER_ADDRESS = 0x00,
                PORT_EXPANDER_IPOL_REGISTER_ADDRESS = 0x02,
                PORT_EXPANDER_GPINT_REGISTER_ADDRESS = 0x04,
                PORT_EXPANDER_DEFVAL_REGISTER_ADDRESS = 0x06,
                PORT_EXPANDER_INTCON_REGISTER_ADDRESS = 0x08,
                PORT_EXPANDER_GPPU_REGISTER_ADDRESS = 0x0C,
                PORT_EXPANDER_INTF_REGISTER_ADDRESS = 0x0E,
                PORT_EXPANDER_INTCAP_REGISTER_ADDRESS = 0x10,
                PORT_EXPANDER_GPIO_REGISTER_ADDRESS = 0x12,
                PORT_EXPANDER_OLAT_REGISTER_ADDRESS = 0x14,
                bank = PinOpt.bank.A
            });
            banks.Add(new BankDetails {
                PORT_EXPANDER_IODIR_REGISTER_ADDRESS = 0x01,
                PORT_EXPANDER_IPOL_REGISTER_ADDRESS = 0x03,
                PORT_EXPANDER_GPINT_REGISTER_ADDRESS = 0x05,
                PORT_EXPANDER_DEFVAL_REGISTER_ADDRESS = 0x07,
                PORT_EXPANDER_INTCON_REGISTER_ADDRESS = 0x09,
                PORT_EXPANDER_GPPU_REGISTER_ADDRESS = 0x0D,
                PORT_EXPANDER_INTF_REGISTER_ADDRESS = 0x0F,
                PORT_EXPANDER_INTCAP_REGISTER_ADDRESS = 0x11,
                PORT_EXPANDER_GPIO_REGISTER_ADDRESS = 0x13,
                PORT_EXPANDER_OLAT_REGISTER_ADDRESS = 0x15,
                bank = PinOpt.bank.B
            });

            //call initialization of base inherited class to initialize the i2c device
            var task = MCPinit();
            task.Wait();

            foreach (BankDetails bank in banks)
            {
                foreach (Pin pin in MCPpins)
                {
                    if (pin.bank == bank.bank)
                    {
                        if (pin.IO == PinOpt.IO.output)
                        {
                            bitMask = (byte)(bank.iodirRegister ^ (byte)pin.pin);
                            bank.iodirRegister &= bitMask;

                            bitMask = (byte)(0xFF ^ (byte)pin.pin);
                            bank.gpintRegister &= bitMask;
                        }
                        else if (pin.IO == PinOpt.IO.input)
                        {
                            bitMask = (byte)(bank.iodirRegister |= (byte)pin.pin);
                            bank.iodirRegister &= bitMask;

                            bitMask = (byte)(0x00 ^ (byte)pin.pin);
                            bank.gpintRegister |= bitMask;
                        }
                    }

                }
                i2cPortExpander.Write(new byte[] { bank.PORT_EXPANDER_IODIR_REGISTER_ADDRESS, bank.iodirRegister });
                i2cPortExpander.Write(new byte[] { bank.PORT_EXPANDER_GPINT_REGISTER_ADDRESS, bank.gpintRegister });
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
                interuptA = new GPIOAccess.PinIn(InteruptPin, false);
            }
            else
            {
                interuptA = null;
            }
        }

        public void init()
        {
            //call initialization of base inherited class to initialize the i2c device
            var task = MCPinit();
            task.Wait();

            //add the bank for a MCP23008
            banks.Add(new BankDetails
            {
                PORT_EXPANDER_IODIR_REGISTER_ADDRESS = 0x00,
                PORT_EXPANDER_IPOL_REGISTER_ADDRESS = 0x02,
                PORT_EXPANDER_GPINT_REGISTER_ADDRESS = 0x04,
                PORT_EXPANDER_DEFVAL_REGISTER_ADDRESS = 0x06,
                PORT_EXPANDER_INTCON_REGISTER_ADDRESS = 0x08,
                PORT_EXPANDER_GPPU_REGISTER_ADDRESS = 0x0C,
                PORT_EXPANDER_INTF_REGISTER_ADDRESS = 0x0E,
                PORT_EXPANDER_INTCAP_REGISTER_ADDRESS = 0x10,
                PORT_EXPANDER_GPIO_REGISTER_ADDRESS = 0x12,
                PORT_EXPANDER_OLAT_REGISTER_ADDRESS = 0x14,
                bank = PinOpt.bank.A
            });

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
                i2cPortExpander.Write(new byte[] { banks[0].PORT_EXPANDER_IODIR_REGISTER_ADDRESS, banks[0].iodirRegister });
            }
        }
    }
}
