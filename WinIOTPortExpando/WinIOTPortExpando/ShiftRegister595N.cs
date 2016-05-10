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
using System.Collections;

namespace WinIOTPortExpando
{
    namespace ShiftRegister
    {
        public class Shift595N
        {
            private PinOut PinSDI, PinRCLK, PinSRCLK;     // Serial Data Input 22 14, Shift Register Clock 27 11, Storage Register Clock 18 12
            private BitArray currentstate; //store binary value of whats commited to the shift register
            private int PinQT;

            public Shift595N(int SDIPin, int SRCLKPin, int CLKPin, int NumOfRegisters)
            {
                this.PinSDI = new PinOut(SDIPin);
                this.PinRCLK = new PinOut(CLKPin);
                this.PinSRCLK = new PinOut(SRCLKPin);

                //calculate the number of bits that need to be set
                PinQT = NumOfRegisters * 8 - 1;

                currentstate = new BitArray(PinQT + 1);

                allEnabled(false);
            }

            public void allEnabled(bool state)
            {
                switch (state)
                {
                    case true:
                        this.currentstate.SetAll(false);
                        sendToRegister();
                        break;
                    case false:
                        this.currentstate.SetAll(false);
                        sendToRegister();
                        break;
                }
            }

            public virtual bool getPinState(int pin)
            {
                checkpins(pin);
                switch (this.currentstate[pin])
                {
                    case true: { return false; }
                    default: { return true; }
                }
            }

            public void putEnabled(int pin, bool state)
            {
                checkpins(pin);
                switch (state)
                {
                    case true: { this.currentstate[pin] = false; break; }
                    default: { this.currentstate[pin] = true; break; }
                }
                sendToRegister();
            }

            private void sendToRegister()
            {
                //do this in the reverse order since each bit clocked in is shifted.
                for (int i = PinQT; i >= 0; i--)
                {
                    switch (this.currentstate[i])
                    {
                        case true: { PinSDI.putEnabled(true); break; }
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

            //Used to ensure that the pin passed is a valid pin to change.
            internal void checkpins(int pin)
            {
                if (pin > PinQT || pin < 0)
                {
                    throw new ArgumentOutOfRangeException("Invalid pin number. Ensure that you instantiated with enough registers defined.");
                }
            }
        }
    }
}