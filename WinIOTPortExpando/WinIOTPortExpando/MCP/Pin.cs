﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WinIOTPortExpando.MCPBase.PinOpt;

namespace WinIOTPortExpando.MCPBase
{
    public class PinOpt
    {
        //pins variable to simplify consumption.
        //User doesnt need to pass in the byte value to turn on a pin this way.
        public enum pin
        {
            GP0 = 0x1,
            GP1 = 0x2,
            GP2 = 0x4,
            GP3 = 0x8,
            GP4 = 0x10,
            GP5 = 0x20,
            GP6 = 0x40,
            GP7 = 0x80,
        }

        public enum IO
        {
            input,
            output
        }

        public enum register
        {
            A,
            B
        }
    }

    public class Pin
    {
        public PinOpt.pin pin { get; set; }
        public PinOpt.IO IO { get; set; }
        public register register { get; set; }
        public bool interupt { get; set; }
        public bool state { get; set; }

        public delegate void MyEventHandler(Pin m);
        public event MyEventHandler OnChange;

        //method for MCPBase to trigger the event.
        internal void TriggerChange()
        {
            if(OnChange != null)
            {
                OnChange(this);
            }
        }
    }
}
