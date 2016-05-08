using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.System.Threading;
using static WinIOTPortExpando.GPIOAccess;

namespace WinIOTPortExpando
{
    public class pinchange
    {
        public Dictionary<int, bool> pinstate;
    }

    namespace ShiftRegister
    {
        public class Shift165N
        {
            private PinOut PinPL, PinCP, PinCE;
            private PinIn PinQ7;
            private BitArray currentstate; //store binary value of whats commited to the shift register
            private int PinQT;
            private ThreadPoolTimer timer;

            public delegate void MyEventHandler(pinchange m);
            public event MyEventHandler OnChange;

            private Object thisLock = new Object();
            private pinchange statechange = new pinchange();

            public Shift165N(int PLPin, int CPPin, int CEPin, int Q7Pin, int NumOfRegisters)
            {
                this.PinPL = new PinOut(PLPin);
                this.PinCP = new PinOut(CPPin);
                this.PinCE = new PinOut(CEPin);
                this.PinQ7 = new PinIn(Q7Pin);

                //calculate the number of bits that need to be set
                PinQT = NumOfRegisters*8-1;

                this.currentstate = new BitArray(PinQT + 1);

                read();
                this.timer = ThreadPoolTimer.CreatePeriodicTimer(Timer_tick, TimeSpan.FromMilliseconds(125));
            }

            private void Timer_tick(ThreadPoolTimer timer)
            {
                lock (thisLock)
                {
                    //get the previous state
                    BitArray temp = new BitArray(this.currentstate);
                    
                    read();
                    //check if previous state = current state
                    if (!comparebitarray(temp, this.currentstate, PinQT))
                    {
                        Dictionary<int, bool> addstate = new Dictionary<int, bool>();

                            for (int pinbit = 0; pinbit <= PinQT; pinbit++)
                            {
                                if (temp[pinbit] != this.currentstate[pinbit])
                                {
                                    pinchange statechange = new pinchange();
                                    if (this.currentstate[pinbit] == true)
                                    {
                                        addstate.Add(pinbit, true);
                                    }
                                    else
                                    {
                                        addstate.Add(pinbit, false);
                                    }
                                }
                            }
                        statechange.pinstate = addstate;
                        OnChange(statechange);
                    }
                }
            }

            public BitArray read()
            {
                GpioPinValue tempval;

                PinPL.putEnabled(true);
                PinPL.putEnabled(false);
                PinCE.putEnabled(true);

                for (int i = PinQT; i >= 0; i--)
                {
                    tempval = PinQ7.GpioPin.Read();

                    if (tempval == GpioPinValue.High)
                    {
                        this.currentstate[i] = true;
                    }
                        else
                    {
                        this.currentstate[i] = false;
                    }
                    clock();
                }

                PinCE.putEnabled(false);
                return this.currentstate;
            }

            private void clock()
            {
                PinCP.putEnabled(true);
                PinCP.putEnabled(false);
            }

            private bool comparebitarray(BitArray a, BitArray b, int length)
            {
                for (int i = 0; i <= length; i++)
                {
                    if (a[i] != b[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
