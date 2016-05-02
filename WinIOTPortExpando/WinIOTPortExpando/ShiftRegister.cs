using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using static WinIOTPortExpando.GPIOAccess;
using Windows.Foundation;

namespace WinIOTPortExpando
{
    namespace ShiftRegister
    {
        public class Shift595N
        {
            //private 
            private PinOut PinSDI;     // Serial Data Input 22 14
            private PinOut PinRCLK;    // Shift Register Clock 27 11
            private PinOut PinSRCLK;   // Storage Register Clock 18 12
            private string currentstate; //store binary value of whats commited to the shift register

            public Shift595N(int SDIPin, int SRCLKPin, int CLKPin, int NumOfRegisters)
            {
                this.PinSDI = new PinOut(SDIPin);
                this.PinRCLK = new PinOut(CLKPin);
                this.PinSRCLK = new PinOut(SRCLKPin);

                //calculate the number of bits that need to be set
                NumOfRegisters = NumOfRegisters * 8;

                //build variable that state is stored in.
                this.currentstate = new string('1', NumOfRegisters);
                allEnabled(false);
            }

            public void allEnabled(bool state)
            {
                switch (state)
                {
                    case true:
                        this.currentstate = new string('1', this.currentstate.Count());
                        sendToRegister();
                        break;
                    case false:
                        this.currentstate = new string('0', this.currentstate.Count());
                        sendToRegister();
                        break;
                }
            }

            public virtual bool getPinState(int pin)
            {
                checkpins(pin);
                pin = pin - 1;
                if (this.currentstate.Length >= pin)
                {
                    switch (this.currentstate[pin])
                    {
                        case '0': { return false; }
                        default: { return true; }
                    }
                }
                else
                {
                    return false;
                }
            }

            public void putEnabled(int pin, bool state)
            {
                checkpins(pin);
                pin = pin - 1;
                switch (state)
                {
                    case true: { this.currentstate = this.currentstate.Remove(pin, 1).Insert(pin, "1"); break; }
                    case false: { this.currentstate = this.currentstate.Remove(pin, 1).Insert(pin, "0"); break; }
                }
                sendToRegister();
            }

            private void sendToRegister()
            {
                foreach (char digit in this.currentstate.Reverse())
                {
                    switch (digit)
                    {
                        case '0': { PinSDI.putEnabled(true); break; }
                        default: { PinSDI.putEnabled(false); break; }
                    }
                    PulseSRCLK();
                }
                PulseRCLK();
            }

            // Pulse Serial Clock
            //commit data to shift register
            private void PulseSRCLK()
            {
                PinSRCLK.putEnabled(true);
                PinSRCLK.putEnabled(false);
            }

            // Pulse Register Clock
            //push data to pins
            private void PulseRCLK()
            {
                PinRCLK.putEnabled(true);
                PinRCLK.putEnabled(false);
            }

            internal void checkpins(int pin)
            {
                if (pin > this.currentstate.Count() || pin < 0)
                {
                    throw new ArgumentOutOfRangeException("Invalid pin number. Ensure that you instantiated with enough registers defined.");
                }
            }
        }

        public class Shift165N : Shift595N
        {
            public PinIn SerialReadPin;

            public Shift165N(int SDIPin, int CLKPin, int SRCLKPin, int SerialReadPin, int NumOfRegisters) : base(SDIPin, CLKPin, SRCLKPin, NumOfRegisters)
            {
                Shift165N Shift595N = new Shift165N(SDIPin, CLKPin, SRCLKPin, SerialReadPin, NumOfRegisters);

                PinIn SerialRead = new PinIn(SerialReadPin);
            }

            public override bool getPinState(int pin)
            {
                //To Be implimented. Need to attempt to do a serial read from the register
                return true;
            }
        }
    }
}