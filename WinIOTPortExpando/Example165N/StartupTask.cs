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

namespace Example165N
{
    public sealed class StartupTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            try
            {
                Shift165N icin = new Shift165N(6, 13, 19, 26, 1);

                icin.OnChange += triggered;

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
            
        }
    }
}
