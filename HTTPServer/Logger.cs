﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        static StreamWriter sr = new StreamWriter("log.txt");
        public static void LogException(Exception ex)
        {
            DateTime dateTime = new DateTime();
            sr.WriteLine("Datetime: " + dateTime );
            sr.WriteLine("message: " + ex.Message);
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime 
        }
    }
}
