using System;
using System.Collections.Generic;
using System.Linq;
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
            private string currentstate; //store binary value of whats commited to the shift register
            private int NumberOfPins;
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
                NumberOfPins = NumOfRegisters * 8;

                //build variable that state is stored in.
                this.currentstate = new string('1', NumberOfPins);

                read();
                this.timer = ThreadPoolTimer.CreatePeriodicTimer(Timer_tick, TimeSpan.FromMilliseconds(50));
            }

            private void Timer_tick(ThreadPoolTimer timer)
            {
                lock (thisLock)
                {
                    //string temp = (this.currentstate.Reverse()).ToString();
                    string temp = this.currentstate;
                    if (temp != read())
                    {
                        temp = new string(temp.ToCharArray().Reverse().ToArray());
                        //string tempcurrent = (this.currentstate.Reverse()).ToString();
                        string tempcurrent = new string(this.currentstate.ToCharArray().Reverse().ToArray());
                        Dictionary<int, bool> addstate = new Dictionary<int, bool>();
                        for (int i = 0; i <= NumberOfPins - 1; i++)
                        {
                            if (temp[i] != tempcurrent[i])
                            {
                                pinchange statechange = new pinchange();
                                if (tempcurrent[i].ToString() == "1")
                                {
                                    addstate.Add(i, true);
                                }
                                else
                                {
                                    addstate.Add(i, false);
                                }
                            }
                        }
                        statechange.pinstate = addstate;
                        OnChange(statechange);
                    }
                }
            }

            public string read()
            {
                GpioPinValue tempval;

                PinPL.putEnabled(true);
                PinPL.putEnabled(false);
                PinCE.putEnabled(true);

                //int retval = 0;
                for (int i = 0; i <= NumberOfPins-1; i++)
                {
                    //retval = retval << 1;
                    tempval = PinQ7.GpioPin.Read();
                    if (tempval == GpioPinValue.High)
                    {
                        this.currentstate = this.currentstate.Remove(i, 1).Insert(i, "1");
                    }
                    else if (tempval == GpioPinValue.Low)
                    {
                    this.currentstate = this.currentstate.Remove(i, 1).Insert(i, "0");
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
        }
    }
}
