using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;

public class Part3_06_AsyncLambdaExpressions
{
    private Clock _clock;

    public void Subscribe()
    {
        _clock.SecondChanged += async (sender, e) =>
        {
            var x = 1;
            //var deferral = e.Suspendingoperation.GetDeferral();
            //var result = await xxxAsync();
            //deferral.Complete();  //Now, tell Windows we're done
        };
        _clock.Run();
    }

    async void TimeChanged(object sender, TimeInfoEventArgs e)
    {
    }

}