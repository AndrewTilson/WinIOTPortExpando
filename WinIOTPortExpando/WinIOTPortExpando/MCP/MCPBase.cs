using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.I2c;
using Windows.Foundation;

namespace WinIOTPortExpando.MCPBase
{
    public class pinchange
    {
        public Dictionary<int, bool> pinstate;
    }

    public class MCPBase
    {
        internal const string I2C_CONTROLLER_NAME = "I2C1"; //specific to RPi2 or RPi3
        internal byte PORT_EXPANDER_I2C_ADDRESS; //7-bit I2C address of the port expander
        internal byte PORT_EXPANDER_IOCON_REGISTER_ADDRESS = 0x0A; // I/O Expander Configruation Register
        internal GPIOAccess.PinIn interuptA; //input pin to recive interupt signal
        internal GPIOAccess.PinIn interuptB; //input pin to recive interupt signal
        internal List<Pin> MCPpins = new List<Pin>(); //list of all pins on the device
        internal List<Register> registers = new List<Register>(); //list of registers on the device
        internal I2cDevice i2cPortExpander; //device object used to communicate

        internal byte[] i2CReadBuffer = new byte[0xff];
        internal byte bitMask;
        private byte ioconRegister; //local copy of the I2C Port Expander IOCON register

        internal async Task MCPinit()
        {
            if (MCPpins.Count == 0)
            {
                throw new ArgumentException("Must add pin objects via addpins before pins on the register can be used");
            }

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
                throw new TypeInitializationException("Unable to initialize communication to the device", e);
            }

            //configure device for open - drain output on the interrupt pin
            i2cPortExpander.Write(new byte[] { PORT_EXPANDER_IOCON_REGISTER_ADDRESS, 0x00 });

            i2cPortExpander.WriteRead(new byte[] { PORT_EXPANDER_IOCON_REGISTER_ADDRESS }, i2CReadBuffer);
            ioconRegister = i2CReadBuffer[0];

            //set IO, pullup, and interupt status for pins
            foreach (Register register in registers)
            {
                //initialize register
                register.init(i2cPortExpander);

                foreach (Pin pin in MCPpins)
                {
                    if (pin.register == register.register)
                    {
                        if (pin.IO == PinOpt.IO.input)
                        {
                            bitMask = (byte)(register.iodirRegister |= (byte)pin.pin);
                            register.iodirRegister &= bitMask;

                            bitMask = (byte)(0x00 ^ (byte)pin.pin);
                            register.intconRegister |= bitMask;

                            bitMask = (byte)(0x00 ^ (byte)pin.pin);
                            register.gpintRegister |= bitMask;
                        }
                        else if (pin.IO == PinOpt.IO.output)
                        {
                            bitMask = (byte)(register.iodirRegister ^ (byte)pin.pin);
                            register.iodirRegister &= bitMask;

                            bitMask = (byte)(0xFF ^ (byte)pin.pin);
                            register.intconRegister &= bitMask;

                            bitMask = (byte)(0xFF ^ (byte)pin.pin);
                            register.gpintRegister &= bitMask;
                        }
                        bitMask = (byte)(0xFF ^ (byte)pin.pin);
                        register.gppuRegister &= bitMask; //always leaving pullup resistor disabled as testing shows this to be unstable.
                    }
                }

                i2cPortExpander.Write(new byte[] { register.PORT_EXPANDER_GPINT_REGISTER_ADDRESS, register.gpintRegister });
                i2cPortExpander.Write(new byte[] { register.PORT_EXPANDER_INTCON_REGISTER_ADDRESS, register.intconRegister });
                i2cPortExpander.Write(new byte[] { register.PORT_EXPANDER_GPPU_REGISTER_ADDRESS, register.gppuRegister }); 
                i2cPortExpander.Write(new byte[] { register.PORT_EXPANDER_IODIR_REGISTER_ADDRESS, register.iodirRegister });
            }

            //create event subscription for interupt pin if it was provided.
            if (interuptA != null) { interuptA.GpioPin.ValueChanged += InteruptChange; }
            if (interuptB != null) { interuptB.GpioPin.ValueChanged += InteruptChange; }
        }

        //add pins to object so that methods like putalloutputpinsenabled function.
        //if pin is not added to object then onchange will not work for that pin either.
        public void addpins(List<Pin> pins)
        {
            foreach (Pin pin in pins)
            {
                MCPpins.Add(pin);
            }
        }

        //enable or disable all output pins
        public void PutAllOutputPinsEnabled(bool enable)
        {
            PutOutputPinEnabled(MCPpins.ToArray(), enable);
        }

        //enable or disable all pin
        public void PutOutputPinEnabled(Pin pin, bool enable)
        {
            //Check that the pin is an output pin.
            if (pin.IO == PinOpt.IO.output)
            {
                //create temp register and set it = to the register the pin references.
                Register tempregister = new Register();

                if (pin.register == PinOpt.register.A) { tempregister = registers[0]; }
                else if (pin.register == PinOpt.register.B) { tempregister = registers[1]; }

                //Get state of pins and then adjust the pin requested.
                i2cPortExpander.WriteRead(new byte[] { tempregister.PORT_EXPANDER_GPIO_REGISTER_ADDRESS }, i2CReadBuffer);
                switch (enable)
                {
                    case false:
                        {
                            bitMask = (byte)(i2CReadBuffer[0] ^ (byte)pin.pin);
                            tempregister.olatRegister &= bitMask;
                            break;
                        }
                    case true:
                        {
                            bitMask = (byte)(i2CReadBuffer[0] ^ (byte)pin.pin);
                            tempregister.olatRegister |= bitMask;
                            break;
                        }
                }

                //Only if the pin state is different than what was read from the i2cdevice then do a write.
                if (i2CReadBuffer[0] != tempregister.olatRegister)
                {
                    i2cPortExpander.Write(new byte[] { tempregister.PORT_EXPANDER_OLAT_REGISTER_ADDRESS, tempregister.olatRegister });
                }
            }
        }

        //enable or disable all passed pins
        public void PutOutputPinEnabled(Pin[] pins, bool enable)
        {
            //Loop through pins and call single pin PutOutputPinEnabled.
            foreach (Pin pin in pins)
            {
                if (pin.IO != PinOpt.IO.input)
                {
                    PutOutputPinEnabled(pin, enable);
                }
            }
        }

        //Take pin and live from register determine if pin is enabled or not.
        public bool getEnabled(Pin pin)
        {
            if (pin.register == PinOpt.register.A)
            {
                registers[0].gpioRegister = getpinvals(registers[0]);
            }
            else
            {
                registers[1].gpioRegister = getpinvals(registers[1]);
            }
            return (i2CReadBuffer[0] & (byte)pin.pin) != (byte)pin.pin;
        }

        //read 8 bit value from specified register and update the registers local value
        internal byte getpinvals(Register register)
        {
            i2cPortExpander.WriteRead(new byte[] { register.PORT_EXPANDER_GPIO_REGISTER_ADDRESS }, i2CReadBuffer);
            register.gpioRegister = i2CReadBuffer[0];
            return register.gpioRegister;
        }

        private void InteruptChange(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            Register tempregister = new Register();
            PinOpt.register thisregister = new PinOpt.register();
            byte previousestate = new byte();

            //get register need for the interrupt pin that triggered the interrupt
            if (sender.PinNumber == interuptA.Device_PIN)
            {
                tempregister = registers[0];
                thisregister = PinOpt.register.A;
            }
            else if (sender.PinNumber == interuptB.Device_PIN)
            {
                tempregister = registers[1];
                thisregister = PinOpt.register.B;
            }

            //get state of register before interrupt
            i2cPortExpander.WriteRead(new byte[] { tempregister.PORT_EXPANDER_INTCAP_REGISTER_ADDRESS }, i2CReadBuffer);
            previousestate = i2CReadBuffer[0];

            //get the pin that triggered the interrupt
            i2cPortExpander.WriteRead(new byte[] { tempregister.PORT_EXPANDER_INTF_REGISTER_ADDRESS }, i2CReadBuffer);
            if (i2CReadBuffer[0] != tempregister.intfRegister)
            {
                tempregister.intfRegister = i2CReadBuffer[0];

                //Debug.WriteLine("Register" + tempregister.register + " " + tempregister.intfRegister.ToString());

                //check each pin that is in the register that was interrupt and if it was the cause of the interrupt trigger its onchange action
                foreach (Pin pin in MCPpins.Where(p => p.register == thisregister))
                {
                    if ((previousestate & (byte)pin.pin) != 0)
                    {
                        pin.TriggerChange();
                    }
                }
            }
        }
    }
}
