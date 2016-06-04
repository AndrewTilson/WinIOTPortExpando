using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace WinIOTPortExpando
{
    public class GPIOAccess
    {
        //Create new pin used for output.
        public class PinOut
        {
            //used to store the individual pin #
            public int Device_PIN { get; internal set; }
            public GpioPin GpioPin { get; internal set; }

            //initialization that takes the pin being created
            public PinOut(int pinNum)
            {
                var gpio = GpioController.GetDefault();

                if (gpio == null)
                {
                    throw new PlatformNotSupportedException("No Gpio Controller found");
                }

                try
                {
                    this.Device_PIN = pinNum;
                    this.GpioPin = gpio.OpenPin(this.Device_PIN);
                    
                    //Default pin to off
                    this.putEnabled(false);
                    this.GpioPin.SetDriveMode(GpioPinDriveMode.Output);
                }
                catch (Exception)
                {
                    throw new Exception("Error Initializing GPIO Pins");
                }
            }

            //Return the last value pin was set to.
            public bool getPinState()
            {
                switch (this.GpioPin.Read())
                {
                    case GpioPinValue.Low: { return true; }
                    case GpioPinValue.High: { return false; }
                    default: { return false; }
                }
            }

            //Change the pin state.
            public void putEnabled(bool state)
            {
                switch (state)
                {
                    case true:
                        this.GpioPin.Write(GpioPinValue.Low);
                        break;
                    case false:
                        this.GpioPin.Write(GpioPinValue.High);
                        break;
                }
            }
        }

        //Create new pin used for input.
        public class PinIn
        {
            //used to store the individual pin #
            public int Device_PIN { get; internal set; }
            public GpioPin GpioPin { get; internal set; }

            //initialization that takes the pin being created
            public PinIn(int pinNum, bool debouncetimeout = true)
            {
                var gpio = GpioController.GetDefault();

                if (gpio == null)
                {
                    throw new PlatformNotSupportedException("No Gpio Controller found");
                }

                try
                {
                    this.Device_PIN = pinNum;
                    this.GpioPin = gpio.OpenPin(this.Device_PIN);
                    this.GpioPin.SetDriveMode(GpioPinDriveMode.Input);
                    //Set a tolarance of 50milliseconds so any electical noise is not caught.
                    if (debouncetimeout == true)
                    {
                        this.GpioPin.DebounceTimeout = TimeSpan.FromMilliseconds(50);
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Error Initializing GPIO Pins");
                }
            }

            //Return the last value pin was set to.
            public bool getPinState()
            {
                switch (this.GpioPin.Read())
                {
                    case GpioPinValue.Low: { return true; }
                    case GpioPinValue.High: { return false; }
                    default: { return false; }
                }
            }
        }
    }
}
