using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

public delegate void myDelegate();

public class AsyncDelegateExample1
{
    public static void Main()
    {
        var primero = new myDelegate(Primero);
        var segundo = new myDelegate(Segundo);

        IAsyncResult counterResult = primero.BeginInvoke(null, null);
        IAsyncResult parserResult = segundo.BeginInvoke(null, null);

        for (int i = 0; i < 50; i++)
            Console.WriteLine("main " + i);
    }

    public static void Primero()
    {
        for (int i = 0; i < 50; i++)
            Console.WriteLine("primero " + i);
    }

    public static void Segundo()
    {
        for (int i = 0; i < 50; i++)
            Console.WriteLine("segundo " + i);
    }
}