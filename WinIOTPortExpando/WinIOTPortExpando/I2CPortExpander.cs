using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace GPIOAccess
{
    public class I2CPortExpander
    {
        //pins variable to simplify consumption.
        //User doesnt need to pass in the byte value to turn on a pin this way.
        public enum Pins
        {
            GP0 = 0x1,
            GP1 = 0x2,
            GP2 = 0x4,
            GP3 = 0x8,
            GP4 = 0x10,
            GP5 = 0x20,
            GP6 = 0x40,
            GP7 = 0x80
        }

        // use these constants for controlling how the I2C bus is setup
        private const string I2C_CONTROLLER_NAME = "I2C1"; //specific to RPi2 or RPi3
        private const byte PORT_EXPANDER_I2C_ADDRESS = 0x20; // 7-bit I2C address of the port expander
        private const byte PORT_EXPANDER_IODIR_REGISTER_ADDRESS = 0x00; // IODIR register controls the direction of the GPIO on the port expander
        private const byte PORT_EXPANDER_GPIO_REGISTER_ADDRESS = 0x09; // GPIO register is used to read the pins input
        private const byte PORT_EXPANDER_OLAT_REGISTER_ADDRESS = 0x0A; // Output Latch register is used to set the pins output high/low

        private byte iodirRegister; // local copy of I2C Port Expander IODIR register
        private byte gpioRegister; // local copy of I2C Port Expander GPIO register
        private byte olatRegister; // local copy of I2C Port Expander OLAT register

        private I2cDevice i2cPortExpander;

        byte[] i2CWriteBuffer;
        byte[] i2CReadBuffer;
        byte bitMask;

        //initialization class. Can take a different i2c address if needed to add additional i2c port expanders
        public I2CPortExpander(byte I2CAddress = PORT_EXPANDER_I2C_ADDRESS)
        {
            InitializeSystem(I2CAddress);
        }

        private async void InitializeSystem(byte I2CAddress)
        {
            // initialize I2C communications
            try
            {
                var i2cSettings = new I2cConnectionSettings(I2CAddress);
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

            // initialize I2C Port Expander registers
            try
            {
                // initialize local copies of the IODIR, GPIO, and OLAT registers
                i2CReadBuffer = new byte[1];

                // read in each register value on register at a time (could do this all at once but
                // for example clarity purposes we do it this way)
                i2cPortExpander.WriteRead(new byte[] { PORT_EXPANDER_IODIR_REGISTER_ADDRESS }, i2CReadBuffer);
                iodirRegister = i2CReadBuffer[0];

                i2cPortExpander.WriteRead(new byte[] { PORT_EXPANDER_GPIO_REGISTER_ADDRESS }, i2CReadBuffer);
                gpioRegister = i2CReadBuffer[0];

                i2cPortExpander.WriteRead(new byte[] { PORT_EXPANDER_OLAT_REGISTER_ADDRESS }, i2CReadBuffer);
                olatRegister = i2CReadBuffer[0];
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception: {0}", e.Message);
                return;
            }
        }

        public void SetOutputPins(params Pins[] pins)
        {
            //loop through each pin provided and generate bit set for iodirRegister on the passed pins
            foreach (Pins pin in pins)
            {
                bitMask = (byte)(iodirRegister ^ (byte)pin);
                iodirRegister &= bitMask;
            }

            //write out desired bits
            i2CWriteBuffer = new byte[] { PORT_EXPANDER_IODIR_REGISTER_ADDRESS, iodirRegister };
            i2cPortExpander.Write(i2CWriteBuffer);
        }

        public void SetInputPins(params Pins[] pins)
        {
            //loop through each pin provided and generate bit set for iodirRegister on the passed pins
            foreach (Pins pin in pins)
            {
                bitMask = (byte)(iodirRegister |= (byte)pin);
                iodirRegister &= bitMask;
            }

            //write out desired bits
            i2CWriteBuffer = new byte[] { PORT_EXPANDER_IODIR_REGISTER_ADDRESS, iodirRegister };
            i2cPortExpander.Write(i2CWriteBuffer);
        }

        public void PutOutputPinEnabled(bool state, Pins pin)
        {
            //check that the passed pin is a output pin
            i2cPortExpander.WriteRead(new byte[] { PORT_EXPANDER_IODIR_REGISTER_ADDRESS }, i2CReadBuffer);
            if (i2CReadBuffer[0] != 1)
            {
                //need to get state of pins and then adjust the pin requested.
                i2cPortExpander.WriteRead(new byte[] { PORT_EXPANDER_GPIO_REGISTER_ADDRESS }, i2CReadBuffer);
                switch (state)
                    {
                        case true:
                            {
                                bitMask = (byte)(i2CReadBuffer[0] ^ (byte)pin);
                                olatRegister &= bitMask;
                                break;
                            }
                        case false:
                            {
                                bitMask = (byte)(i2CReadBuffer[0] ^ (byte)pin);
                                olatRegister |= bitMask;
                                break;
                            }
                    }
                //only if the pin state is different than what was read from the i2cdevice then do a write.
                if (i2CReadBuffer[0] != olatRegister)
                {
                    i2cPortExpander.Write(new byte[] { PORT_EXPANDER_OLAT_REGISTER_ADDRESS, olatRegister });
                }
            }
        }

        public void PutOutputPinEnabled(bool state, params Pins[] pins)
        {
            foreach (Pins pin in pins)
            {
                PutOutputPinEnabled(state, pin);
            }
        }

        public bool getEnabled(Pins pin)
        {
            i2cPortExpander.WriteRead(new byte[] { PORT_EXPANDER_GPIO_REGISTER_ADDRESS }, i2CReadBuffer);
            return (i2CReadBuffer[0] & (byte)pin) != (byte)pin;
        }

        public void PutAllOutputPinsEnabled(bool state)
        {
            //needs changed to pull from iodir and only change those bits that are set to output
            switch (state)
            {
                case true: 
                    {
                        bitMask = (byte)(0xFF);
                        break;
                    }
                case false:
                    {
                        bitMask = (byte)(0x00);
                        break;
                    }
            }
            i2cPortExpander.Write(new byte[] { PORT_EXPANDER_OLAT_REGISTER_ADDRESS, bitMask });
        }
    }
}
