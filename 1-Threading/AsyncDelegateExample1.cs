using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

public delegate int SampleDelegate(string data);

public class AsyncDelegateExample1
{
    public static void Main()
    {
        SampleDelegate counter = new SampleDelegate(CountCharacters);
        SampleDelegate parser = new SampleDelegate(Parse);

        IAsyncResult counterResult = counter.BeginInvoke("hello", null, null);
        IAsyncResult parserResult = parser.BeginInvoke("10", null, null);
        Console.WriteLine("Main thread continuing");

        Console.WriteLine("Counter returned {0}", counter.EndInvoke(counterResult));
        Console.WriteLine("Parser returned {0}", parser.EndInvoke(parserResult));

        Console.WriteLine("Done");
    }

    public static int CountCharacters(string text)
    {
        Thread.Sleep(2000);
        Console.WriteLine("Counting characters in {0}", text);
        return text.Length;
    }

    public static int Parse(string text)
    {
        Thread.Sleep(100);
        Console.WriteLine("Parsing text {0}", text);
        return int.Parse(text);
    }
}