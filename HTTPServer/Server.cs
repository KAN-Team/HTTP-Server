using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Xml;

namespace HTTPServer
{
    class Server
    {
        string redirectionPath = string.Empty;
        Socket serverSocket;
        string page = null;
        int countformpge = 0;
        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint hostEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket.Bind(hostEndPoint);

        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(100);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = this.serverSocket.Accept();
                Console.WriteLine("---------------------------------------");            
                Console.WriteLine("New client accepted: {0}", clientSocket.RemoteEndPoint);
                Thread newthread = new Thread(new ParameterizedThreadStart(HandleConnection));
                //Start the thread
                newthread.Start(clientSocket);

            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            Socket clientSock = (Socket)obj;
            clientSock.ReceiveTimeout = 0;

            // TODO: receive requests in while true until remote client closes the socket.
            int receivedLength;
            byte[] data;

            while (true)
            {
                try
                {
                    // TODO: Receive request
                    data = new byte[1024];
                    receivedLength = clientSock.Receive(data);

                    // TODO: break the while loop if receivedLen==0
                    //If the message length is ZERO, means client has Closed the connection
                    //Then Close the connection with this client
                    //Else, display the message on the console window

                    if (receivedLength == 0)
                    {
                        Console.WriteLine("---------------------------------------");
                        Console.WriteLine("Client: {0} ended the connection", clientSock.RemoteEndPoint);
                        clientSock.Close();
                        break;
                    }

                    string RequestString = Encoding.ASCII.GetString(data , 0 ,receivedLength);
                    Console.WriteLine("---------------------------------------");
                    Console.WriteLine("Client Request :\r\n"+RequestString);
                    // TODO: Create a Request object using received request string
                    Request request = new Request(RequestString);

                    // TODO: Call HandleRequest Method that returns the response
                    Response response = HandleRequest(request);
                    Console.WriteLine("---------------------------------------");
                    Console.WriteLine("Response : \r\n" + response.ResponseString);

                    if (!string.IsNullOrEmpty(redirectionPath))
                    { 
                        string NewRequestString = RequestString.Replace(request.relativeURI,"/" + redirectionPath);

                        Request request2 = new Request(NewRequestString);
                        redirectionPath = string.Empty;
                        Console.WriteLine("---------------------------------------");
                        Console.WriteLine("Redirected Request :\r\n" + NewRequestString);
                        response = HandleRequest(request2);
                        Console.WriteLine("---------------------------------------");
                        Console.WriteLine("Redirected Response : \r\n" + response.ResponseString);
                    }

                    // TODO: Send Response back to client
                    data = Encoding.ASCII.GetBytes(response.ResponseString);
                    clientSock.Send(data);
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);                    
                }
            }
            // TODO: close client socket
            clientSock.Close();
        }

       public Response HandleRequest(Request request)
        {
            string content;
            string PageName;
            StatusCode code;
            Response response;
            try
            {

                //throw new Exception();
                //TODO: check for bad request 
                if (!request.ParseRequest())
                {
                    code = StatusCode.BadRequest;
                    PageName = Configuration.BadRequestDefaultPageName;
                    content = LoadDefaultPage(PageName);
                    goto returnresponse;
                }

                //TODO: check for redirect
                //TODO: map the relativeURI in request to get the physical path of the resource.
                PageName = request.relativeURI;
                string[] pageName = PageName.Split('/');
                PageName = pageName[1];

                if (Configuration.RedirectionRules.ContainsKey(PageName))
                {
                    code = StatusCode.Redirect;
                    redirectionPath = GetRedirectionPagePathIFExist(PageName);
                    PageName = Configuration.RedirectionDefaultPageName;
                    content = LoadDefaultPage(PageName);
                    goto returnresponse;
                }
                //TODO: check file exists
                //TODO: read the physical file
                // Create OK response
                content = LoadDefaultPage(PageName);
                 if (string.IsNullOrEmpty(content))
                {
                    code = StatusCode.NotFound;
                    PageName = Configuration.NotFoundDefaultPageName;
                    content = LoadDefaultPage(PageName);
                }
                else
                {
                    code = StatusCode.OK;                
                }                            
            }
            catch (Exception ex)
            {
                code = StatusCode.InternalServerError;
                Exception e2 = (Exception)Activator.CreateInstance(ex.GetType(), "Internal Server Error", ex);
                Logger.LogException(e2);
                PageName = Configuration.InternalErrorDefaultPageName;
                content = LoadDefaultPage(PageName);               
            }
        returnresponse:
            //handle post method (bonus)
            if (string.Equals(PageName, "formpage.html"))
            {
                //Thread newthread = new Thread(new ThreadStart(savetoxml));
                //Start the thread
                Process.Start(PageName);
                savetoxml();
                //newthread.Start();
                response = new Response(code, "text/html", "Thank you", redirectionPath, request.httpVersion, request.method);
            }
             else  response = new Response(code, "text/html", content, redirectionPath, request.httpVersion, request.method);
            return response;
        }
        //bonus
        private void savetoxml() {
            Console.WriteLine("&&&&&& threed start");

            string rootFolder = @"C:\Users\Rama2\Downloads";
            string authorsFile = "Student.txt";
            string xmlDocumentpath = "E:/last year/network/Network_project/HTTPServer/Students.xml";
            string filepath = Path.Combine(rootFolder, authorsFile);
            while (true) {
                if (File.Exists(filepath))
                {
                    Console.WriteLine("&&&&&& submit");
                    StreamReader sr = new StreamReader(filepath);
                    string line;
                    string[] student = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        student = line.Split(' ');
                        break;
                    }
                    sr.Close();

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(xmlDocumentpath);
                    XmlNode rootnode = xmlDoc.GetElementsByTagName("StudentsInformation")[0];
                    XmlNode studentnode = xmlDoc.CreateElement("Student");

                    XmlNode studentnamenode = xmlDoc.CreateElement("name");
                    studentnamenode.InnerText = student[0];
                    studentnode.AppendChild(studentnamenode);

                    XmlNode studentidnode = xmlDoc.CreateElement("ID");
                    studentidnode.InnerText = student[1];
                    studentnode.AppendChild(studentidnode);

                    XmlNode studentsecnode = xmlDoc.CreateElement("Section");
                    studentsecnode.InnerText = student[2];
                    studentnode.AppendChild(studentsecnode);

                    rootnode.AppendChild(studentnode);
                    xmlDoc.Save(xmlDocumentpath);
                    File.Delete(filepath);
                    break;
                }
            }
            
        }
     
        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            string RedirectionPage;
            RedirectionPage = Configuration.RedirectionRules[relativePath];
            return RedirectionPage;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string contentOfPage;
            string filePath = Configuration.RootPath +"/" +defaultPageName;
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            // else read file and return its content
            if (File.Exists(filePath))
            {
                // read page and get content
                StreamReader sr = new StreamReader(filePath);
                contentOfPage = sr.ReadToEnd();
            }
            else
            {
                contentOfPage = string.Empty;
            }
            return contentOfPage;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                // then fill Configuration.RedirectionRules dictionary 
                StreamReader sr = new StreamReader(filePath);
                string line;
                string[] rules;
                Configuration.RedirectionRules = new Dictionary<string, string>();
                while ((line = sr.ReadLine()) != null)
                {
                    rules = line.Split(',');
                    Configuration.RedirectionRules.Add(rules[0], rules[1]);
                }
                sr.Close();
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Exception e2 = (Exception)Activator.CreateInstance(ex.GetType(), "redicriction file is not exist", ex);
                Logger.LogException(e2);
                Environment.Exit(1);
            }
        }
    }
}
