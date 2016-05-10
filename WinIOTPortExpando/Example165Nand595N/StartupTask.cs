using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using WinIOTPortExpando.ShiftRegister;
using System.Threading.Tasks;
using WinIOTPortExpando;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Example165Nand595N
{
    public sealed class StartupTask : IBackgroundTask
    {
        Shift165N Shift165N = new Shift165N(6, 13, 19, 26, 1);
        Shift595N Shift595N = new Shift595N(22, 27, 18, 1);

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            try
            {
                Shift165N.OnChange += triggered;
                Shift595N.allEnabled(false);

                //Force process to not end untill app closure.
                do
                {
                    await Task.Delay(10000);
                } while (true);
            }
            finally
            {
                deferral.Complete();
            }
        }

        private void triggered(pinchange m)
        {
            foreach (KeyValuePair<int,bool> pin in m.pinstate)
            {
                Shift595N.putEnabled(pin.Key, pin.Value);
            }
        }
    }
}
