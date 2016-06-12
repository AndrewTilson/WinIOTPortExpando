using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using WinIOTPortExpando;
using System.Diagnostics;
using System.Threading.Tasks;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace ExampleTMP102
{
    public sealed class StartupTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            try
            {
                TMP102 temp = new TMP102(A0AddressSelect.GND);

                Task init = temp.OpenAsync();
                init.Wait();

                while (true)
                {
                    float degree = temp.Temperature();
                    Debug.WriteLine(degree);
                    await Task.Delay(500);
                }
            }
            finally
            {
                deferral.Complete();
            }
        }
    }
}
