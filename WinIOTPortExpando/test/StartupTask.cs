using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using WinIOTPortExpando;
using System.Threading.Tasks;
using static WinIOTPortExpando.MCP23017;
using System.Diagnostics;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace test
{
    public sealed class StartupTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            try
            {

                MCP23017 test = new MCP23017();
                Task mytest = test.Init();
                mytest.Wait();

                test.SetInterruptOnChange(MCP23017.Pin.GPA0, true);
                test.SetInterruptOnChange(MCP23017.Pin.GPA1, true);
                test.SetInterruptOnChange(MCP23017.Pin.GPA2, true);
                test.SetInterruptOnChange(MCP23017.Pin.GPA3, true);
                test.SetInterruptOnChange(MCP23017.Pin.GPA4, true);
                test.SetInterruptOnChange(MCP23017.Pin.GPA5, true);
                test.SetInterruptOnChange(MCP23017.Pin.GPA6, true);
                test.SetInterruptOnChange(MCP23017.Pin.GPA7, true);

                List<Pin> interupt = new List<Pin>();

                do
                {
                    test.Write(MCP23017.Pin.GPA0, MCP23017.PinValue.High);
                    interupt = test.GetChangedPins();
                    Debug.WriteLine(interupt.Count());
                    await Task.Delay(25);
                    test.Write(MCP23017.Pin.GPA0, MCP23017.PinValue.Low);
                    interupt = test.GetChangedPins();
                    Debug.WriteLine(interupt.Count());
                    await Task.Delay(25);
                    test.Write(MCP23017.Pin.GPA1, MCP23017.PinValue.High);
                    interupt = test.GetChangedPins();
                    Debug.WriteLine(interupt.Count());
                    await Task.Delay(25);
                    test.Write(MCP23017.Pin.GPA1, MCP23017.PinValue.Low);
                    interupt = test.GetChangedPins();
                    Debug.WriteLine(interupt.Count());
                    await Task.Delay(25);
                    test.Write(MCP23017.Pin.GPA2, MCP23017.PinValue.High);
                    interupt = test.GetChangedPins();
                    Debug.WriteLine(interupt.Count());
                    await Task.Delay(25);
                    test.Write(MCP23017.Pin.GPA2, MCP23017.PinValue.Low);
                    interupt = test.GetChangedPins();
                    Debug.WriteLine(interupt.Count());
                    await Task.Delay(25);
                    test.Write(MCP23017.Pin.GPA3, MCP23017.PinValue.High);
                    interupt = test.GetChangedPins();
                    Debug.WriteLine(interupt.Count());
                    await Task.Delay(25);
                    test.Write(MCP23017.Pin.GPA3, MCP23017.PinValue.Low);
                    interupt = test.GetChangedPins();
                    Debug.WriteLine(interupt.Count());
                    await Task.Delay(25);
                    test.Write(MCP23017.Pin.GPA4, MCP23017.PinValue.High);
                    interupt = test.GetChangedPins();
                    Debug.WriteLine(interupt.Count());
                    await Task.Delay(25);
                    test.Write(MCP23017.Pin.GPA4, MCP23017.PinValue.Low);
                    interupt = test.GetChangedPins();
                    Debug.WriteLine(interupt.Count());
                    await Task.Delay(25);
                    test.Write(MCP23017.Pin.GPA5, MCP23017.PinValue.High);
                    interupt = test.GetChangedPins();
                    Debug.WriteLine(interupt.Count());
                    await Task.Delay(25);
                    test.Write(MCP23017.Pin.GPA5, MCP23017.PinValue.Low);
                    interupt = test.GetChangedPins();
                    Debug.WriteLine(interupt.Count());
                    await Task.Delay(25);
                    test.Write(MCP23017.Pin.GPA6, MCP23017.PinValue.High);
                    interupt = test.GetChangedPins();
                    Debug.WriteLine(interupt.Count());
                    await Task.Delay(25);
                    test.Write(MCP23017.Pin.GPA6, MCP23017.PinValue.Low);
                    interupt = test.GetChangedPins();
                    Debug.WriteLine(interupt.Count());
                    await Task.Delay(25);
                    test.Write(MCP23017.Pin.GPA7, MCP23017.PinValue.High);
                    interupt = test.GetChangedPins();
                    Debug.WriteLine(interupt.Count());
                    await Task.Delay(25);
                    test.Write(MCP23017.Pin.GPA7, MCP23017.PinValue.Low);
                    interupt = test.GetChangedPins();
                    Debug.WriteLine(interupt.Count());
                    await Task.Delay(25);
                } while (true);
            }
            finally
            {
                deferral.Complete();
            }
        }
    }
}
