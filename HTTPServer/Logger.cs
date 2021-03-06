using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        // TODO: Create log file named log.txt to log exception details in it
        static StreamWriter sr = new StreamWriter("log.txt");
        static readonly object _object = new object();

        // for each exception write its details associated with datetime 
        public static void LogException(Exception ex)
        {
            lock (_object)
            {
                DateTime dateTime = DateTime.Now;
                sr.WriteLine("Datetime: " + dateTime.ToString());
                sr.WriteLine("message: " + ex.Message);
            }
            
        }
    }
}
