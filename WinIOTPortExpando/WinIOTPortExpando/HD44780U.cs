using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinIOTPortExpando.MCPBase;
using static WinIOTPortExpando.MCPBase.PinOpt;


namespace WinIOTPortExpando
{
    //This class has not been fully tested yet. Turns out the lcd screen i have on hand was bad so until i have one to test with i cannot verify this is working.
    public class HD44780U
    {
        private const byte ENABLE_BIT = 0x20;
        private const byte ENABLE_BIT_INVERSE = 0xDF;
        private const byte READ_BIT = 0x40;
        private const byte DATA_BIT = 0x80;
        private const byte WRITE_INSTRUCTION = 0x0;
        private const byte WRITE_DATA = DATA_BIT;
        internal PinOpt.register register { get; set; }

        public enum PortExpander
        {
            MCP23017,
            MCP23008
        }

        internal WinIOTPortExpando.MCPBase.MCPBase expander;

        public HD44780U (byte address, PortExpander ExpanderIC, register register)
        {
            switch (ExpanderIC)
            {
                case PortExpander.MCP23017:
                    {
                        MCP23017 temp = new MCP23017(address);
                        temp.init();
                        expander = temp;
                        break;
                    }
                case PortExpander.MCP23008:
                    {
                        MCP23008 temp = new MCP23008(address);
                        temp.init();
                        expander = temp;
                        break;
                    }
            }
        }

        public static void Delay(int ms)
        {
            int time = Environment.TickCount;
            while (true)
                if (Environment.TickCount - time >= ms)
                    return;
        }


        public void LcdReset()
        {
            //interpreted by HD44780U controller as 8 bits until switched to 4 bits below
            Send4BitInstruction(0x03); //0011
            Delay(4);
            Send4BitInstruction(0x03); //0011
            Send4BitInstruction(0x03); //0011

            // Function set (Set interface to be 4 bits long.) Interface is 8 bits in length.
            Send4BitInstruction(0x02); //0010

            // Set number of lines and font
            Send4BitInstruction(0x02); //0010
            Send4BitInstruction(0x08); //1000

            // Turn on display.  No cursor
            Send4BitInstruction(0x00); //0000
            Send4BitInstruction(0x0C); //1100

            // Entry mode set - increment addr by 1, shift cursor by right.
            Send4BitInstruction(0x00); //0000
            Send4BitInstruction(0x06); //0110

            LcdClear();
        }
        public void LcdClear()
        {
            Send4BitInstruction(0x00); //0000
            Send4BitInstruction(0x01); //0001
            Delay(1);
            // return to zero
            Send4BitInstruction(0x00); //0000
            Send4BitInstruction(0x02); //0010
            Delay(1);
        }
        public void MoveToLine(byte line)
        {
            switch (line)
            {
                case 0:
                    SetDisplayAddress(0);
                    break;
                case 1:
                    SetDisplayAddress(64);
                    break;
            }
        }
        public void PrintLine(string data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                Send8BitCharacter((byte)data.ToCharArray()[i]);
                Delay(1);
            }
        }
        private void Send4BitInstruction(byte c)
        {
            Send4Bits(WRITE_INSTRUCTION, c);
        }

        private void Send8BitCharacter(byte c)
        {
            byte temp = c;
            temp >>= 4;
            Send4Bits(WRITE_DATA, temp);
            temp = c;
            temp &= 0x0F;
            Send4Bits(WRITE_DATA, temp);
        }

        private void Send4Bits(byte control, byte d)
        {
            byte portB = control;
            portB |= FlipAndShift(d);
            portB |= ENABLE_BIT;

            if (register == register.A)
                expander.writegpio(PinOpt.register.A, portB);
            else
                expander.writegpio(PinOpt.register.B, portB);

            portB &= 0xDF;
            if (register == register.A)
                expander.writegpio(PinOpt.register.A, portB);
            else
                expander.writegpio(PinOpt.register.B, portB);
        }

        private void SetDisplayAddress(byte addr)
        {
            byte temp = addr;
            temp >>= 4;
            temp |= 0x08;
            Send4BitInstruction(temp);
            temp = addr;
            temp &= 0x0F;
            Send4BitInstruction(temp);
        }

        private byte FlipAndShift(byte src)
        {
            byte dest = 0;
            if ((src & 0x1) != 0)
            {
                dest |= 0x10;
            }
            if ((src & 0x2) != 0)
            {
                dest |= 0x08;
            }
            if ((src & 0x4) != 0)
            {
                dest |= 0x04;
            }
            if ((src & 0x8) != 0)
            {
                dest |= 0x02;
            }
            return dest;
        }

        public string GetWiringInfo()
        {
            string output = string.Empty;
            output += "HD44780U Compatible LCD Driver" + Environment.NewLine;
            output += "Wiring and pin example for LMB162ABC LCD panel" + Environment.NewLine;
            output += "Pins: " + Environment.NewLine;
            output += "   1 - Vss (ground)" + Environment.NewLine;
            output += "   2 - Vdd (+ power)" + Environment.NewLine;
            output += "   3 - V0 (LCD Contrast reference supply)" + Environment.NewLine;
            output += "   4 - RS (register select)" + Environment.NewLine;
            output += "   5 - R/W (read / write control bus)" + Environment.NewLine;
            output += "   6 - E (data enable)" + Environment.NewLine;
            output += "   7 - DB0" + Environment.NewLine;
            output += "   8 - DB1" + Environment.NewLine;
            output += "   9 - DB2" + Environment.NewLine;
            output += "   10 - DB3" + Environment.NewLine;
            output += "   11 - DB4" + Environment.NewLine;
            output += "   12 - DB5" + Environment.NewLine;
            output += "   13 - DB6" + Environment.NewLine;
            output += "   14 - DB7" + Environment.NewLine;
            output += "   15 - BLA (backlight positive supply)" + Environment.NewLine;
            output += "   16 - BLK (backlight negative supply)" + Environment.NewLine + Environment.NewLine;
            output += "Wiring to MCP23017 I/O Expander" + Environment.NewLine;
            output += "   4 - GPB7" + Environment.NewLine;
            output += "   5 - GPB6" + Environment.NewLine;
            output += "   6 - GPB5" + Environment.NewLine;
            output += "   11 - GPB4" + Environment.NewLine;
            output += "   12 - GPB3" + Environment.NewLine;
            output += "   13 - GPB2" + Environment.NewLine;
            output += "   14 - GPB1" + Environment.NewLine + Environment.NewLine;
            output += "Wiring can connect to ports B or ports A on the MCP23017." + Environment.NewLine;
            output += "The port designation is passed into the class upon instantiation.";

            return output;
        }
    }
}
