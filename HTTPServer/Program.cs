using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static string path = @"C:\Users\LENOVO\OneDrive\Documents\githup\Network_project\HTTPServer\bin\Debug\Redirection.txt";

        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            CreateRedirectionRulesFile();
            //Start server
            // 1) Make server object on port 1000
            Server server = new Server(1000, path);
            // 2) Start Server
            Console.WriteLine("Server is active now  ^_^ ");
            server.StartServer();
            
        }

        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            Stream stream = new FileStream(path,FileMode.Create);
            TextWriter tw = new StreamWriter(stream , Encoding.UTF8);
            tw.WriteLine("aboutus.html,aboutus2.html");
            tw.Close();
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2
        }
         
    }
}
