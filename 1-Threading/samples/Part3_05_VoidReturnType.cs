using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;

public class Part3_VoidReturnType
{
    private Clock _clock;

    public void Subscribe()
    {
        _clock.SecondChanged += new SecondChangeHandler(TimeChanged);
        _clock.Run();
    }

    async void TimeChanged(object sender, TimeInfoEventArgs e)
    {
        //// A dewferral tells Windows you're returning but not done
        //var deferral = e.Suspendingoperation.GetDeferral();
        //// TODO: perform async operation(s) here ...
        //var result = await xxxAsync();
        //deferral.Complete();  //Now, tell Windows we're done
    }

}

public class TimeInfoEventArgs : EventArgs { }
public delegate void SecondChangeHandler(object clock, TimeInfoEventArgs timeInfo);

public class Clock
{
    public SecondChangeHandler SecondChanged;

    public void Run()
    {
        SecondChanged(this, new TimeInfoEventArgs());
    }
}

public class DisplayClock
{
    private Clock _clock;

    public DisplayClock()
    {
        _clock.SecondChanged += new SecondChangeHandler(TimeChanged);
        _clock.Run();
    }

    public void TimeChanged(object sender, TimeInfoEventArgs e)
    {
        Console.Write("Event fires to a regular event handler");
    } 
}