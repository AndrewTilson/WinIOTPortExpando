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
            if (InteruptPinA != 0) { interuptA = new GPIOAccess.PinIn(InteruptPinA, false); }
            else { interuptA = null; }
            if (InteruptPinB != 0) { interuptB = new GPIOAccess.PinIn(InteruptPinB, false); }
            else { interuptB = null; }
        }

        public void init()
        {
            //add the 2 registers for a MCP23017
            registers.Add(new Register {
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
                register = PinOpt.register.A
            });
            registers.Add(new Register {
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
                register = PinOpt.register.B
            });

            //call initialization of base inherited class to initialize the i2c device
            var task = MCPinit();
            task.Wait();
        }
    }

    public class MCP23008 : MCPBase
    {
        public MCP23008(byte I2CAddress = 0x20, int InteruptPin = 0)
        {
            PORT_EXPANDER_I2C_ADDRESS = I2CAddress;
            if (InteruptPin != 0) { interuptA = new GPIOAccess.PinIn(InteruptPin, false); }
            else { interuptA = null; }
            //set interupt b to null since there is only one register.
            interuptB = null;
        }

        public void init()
        {
            //add the register for a MCP23008
            registers.Add(new Register
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
                register = PinOpt.register.A
            });

            //call initialization of base inherited class to initialize the i2c device
            var task = MCPinit();
            task.Wait();
        }
    }
}
