using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.I2c;
using Windows.Foundation;

namespace WinIOTPortExpando.MCPBase
{
    public class MCPBase
    {
        internal const string I2C_CONTROLLER_NAME = "I2C1"; //specific to RPi2 or RPi3
        internal byte PORT_EXPANDER_I2C_ADDRESS; //7-bit I2C address of the port expander
        internal GPIOAccess.PinIn interupt;
        internal List<Pin> MCPpins = new List<Pin>();
        internal List<BankDetails> banks = new List<BankDetails>();

        internal I2cDevice i2cPortExpander;

        internal byte[] i2CWriteBuffer;
        internal byte[] i2CReadBuffer = new byte[0xff];
        internal byte bitMask;

        internal async Task MCPinit()
        {
            //initialize I2C communications
            try
            {
                var i2cSettings = new I2cConnectionSettings(PORT_EXPANDER_I2C_ADDRESS);
                i2cSettings.BusSpeed = I2cBusSpeed.FastMode;
                i2cSettings.SharingMode = I2cSharingMode.Shared;
                string deviceSelector = I2cDevice.GetDeviceSelector(I2C_CONTROLLER_NAME);
                var i2cDeviceControllers = await DeviceInformation.FindAllAsync(deviceSelector);
                i2cPortExpander = await I2cDevice.FromIdAsync(i2cDeviceControllers[0].Id, i2cSettings);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception: {0}", e.Message);
                return;
            }

            //initialize banks
            foreach (BankDetails bank in banks)
            {
                bank.init(i2cPortExpander);
            }

            //create event subscription for interupt pin if it was provided.
            if (interupt != null)
            {
                interupt.GpioPin.ValueChanged += InteruptChange();
            }
        }

        public void addpins(List<Pin> pins)
        {
            foreach (Pin pin in pins)
            {
                MCPpins.Add(pin);
            }
        }

        public void PutAllOutputPinsEnabled(bool enable)
        {
            //On all output pins enable or disable.
            foreach (Pin pin in MCPpins)
            {
                if (pin.IO == PinOpt.IO.output)
                {
                    PutOutputPinEnabled(pin, enable);
                }
            }
        }

        public void PutOutputPinEnabled(Pin pin, bool enable)
        {
            //Check that the pin is an output pin.
            if (pin.IO == PinOpt.IO.output)
            {
                //create temp bank and set it = to the bank the pin references.
                BankDetails tempbank = new BankDetails();

                if (pin.bank == PinOpt.bank.A) { tempbank = banks[0]; }
                else if (pin.bank == PinOpt.bank.B) { tempbank = banks[1]; }

                //Get state of pins and then adjust the pin requested.
                i2cPortExpander.WriteRead(new byte[] { tempbank.PORT_EXPANDER_GPIO_REGISTER_ADDRESS }, i2CReadBuffer);
                switch (enable)
                {
                    case false:
                        {
                            bitMask = (byte)(i2CReadBuffer[0] ^ (byte)pin.pin);
                            tempbank.olatRegister &= bitMask;
                            break;
                        }
                    case true:
                        {
                            bitMask = (byte)(i2CReadBuffer[0] ^ (byte)pin.pin);
                            tempbank.olatRegister |= bitMask;
                            break;
                        }
                }

                //Only if the pin state is different than what was read from the i2cdevice then do a write.
                if (i2CReadBuffer[0] != tempbank.olatRegister)
                {
                    i2cPortExpander.Write(new byte[] { tempbank.PORT_EXPANDER_OLAT_REGISTER_ADDRESS, tempbank.olatRegister });
                }
            }
            else
            {
                throw new ArgumentException("One of the pins specified is not an output pin.");
            }
        }

        public void PutOutputPinEnabled(Pin[] pins, bool enable)
        {
            //Loop through pins and call single pin PutOutputPinEnabled.
            foreach (Pin pin in pins)
            {
                PutOutputPinEnabled(pin, enable);
            }
        }

        public bool getEnabled(Pin pin)
        {
            if (pin.bank == PinOpt.bank.A)
            {
                i2cPortExpander.WriteRead(new byte[] { banks[0].PORT_EXPANDER_GPIO_REGISTER_ADDRESS }, i2CReadBuffer);
            }
            else
            {
                i2cPortExpander.WriteRead(new byte[] { banks[1].PORT_EXPANDER_GPIO_REGISTER_ADDRESS }, i2CReadBuffer);
            }
            return (i2CReadBuffer[0] & (byte)pin.pin) != (byte)pin.pin;
        }

        private TypedEventHandler<GpioPin, GpioPinValueChangedEventArgs> InteruptChange()
        {
            //Loop through input pins and check if they changed from what is in memory.
            throw new NotImplementedException();
            //return;
        }
    }
}
