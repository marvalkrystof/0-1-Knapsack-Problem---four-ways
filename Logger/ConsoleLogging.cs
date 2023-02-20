using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    internal class ConsoleLogging : ILoggingMethod
    {
    
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    
    }
}
