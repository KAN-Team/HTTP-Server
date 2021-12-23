using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

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
            byte[] data = new byte[1024];
            string welcomeMsg = "Welcome to my server ^_^ ..";
            data = Encoding.ASCII.GetBytes(welcomeMsg);
            clientSock.Send(data);

            while (true)
            {
                try
                {
                    // TODO: Receive request
                    data = new byte[1024];
                    receivedLength = clientSock.Receive(data);
                    // TODO: break the while loop if receivedLen==0
                    if (receivedLength == 0)
                    {
                       // Console.WriteLine("Client: {0} ended the connection", clientSock.RemoteEndPoint);
                        break;
                    }
                    // TODO: Create a Request object using received request string
                    string RequestString = Encoding.ASCII.GetString(data , 0 ,receivedLength);
                    Console.WriteLine("Client Request :\r\n"+RequestString);

                    Request request = new Request(RequestString);
                    // TODO: Call HandleRequest Method that returns the response
                    Response response = HandleRequest(request);
                    Console.WriteLine("Response : \r\n" + response.ResponseString);
                    // TODO: Send Response back to client
                    data = Encoding.ASCII.GetBytes(response.ResponseString);
                    clientSock.Send(data);
                    //If the message length is ZERO, means client has Closed the connection
                    //Then Close the connection with this client
                    if (receivedLength == 0)
                    {
                        Console.WriteLine("Client: {0} ended the connection", clientSock.RemoteEndPoint);
                        clientSock.Close();
                    }
                    //Else, display the message on the console window

                }

                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    // TODO: log exception using Logger class
                }
            }

            // TODO: close client socket
            clientSock.Close();
        }
        Response HandleRequest(Request request)
        {
            string content;
            string physicalPath, 
                redirectionPath = string.Empty;
            StatusCode code;
            Response response;
            try
            {
                //TODO: check for bad request 
                bool ValidRequest = request.ParseRequest();
                if (!ValidRequest)
                {
                    code = StatusCode.BadRequest;
                    physicalPath = Configuration.BadRequestDefaultPageName;
                    content = LoadDefaultPage(physicalPath);
                    loadPage(physicalPath, content);
                    response = new Response(code, "text/html", content, redirectionPath, request.httpVersion);
                    return response;
                }
                //TODO: map the relativeURI in request to get the physical path of the resource.
                physicalPath = request.relativeURI;
                string[] pageName = physicalPath.Split('/');
                physicalPath = pageName[1];
                //TODO: check for redirect
                if (Configuration.RedirectionRules.ContainsKey(physicalPath))
                {
                    code = StatusCode.Redirect;
                    redirectionPath = GetRedirectionPagePathIFExist(physicalPath);
                    physicalPath = Configuration.RedirectionDefaultPageName;
                    content = LoadDefaultPage(physicalPath);
                    loadPage(physicalPath, content);
                    response = new Response(code, "text/html", content, redirectionPath, request.httpVersion);
                    return response;
                }
                //TODO: check file exists
                //TODO: read the physical file
                content = LoadDefaultPage(physicalPath);
                if (string.IsNullOrEmpty(content))
                {
                    code = StatusCode.NotFound;
                    physicalPath = Configuration.NotFoundDefaultPageName;
                    content = LoadDefaultPage(physicalPath);
                }
                else
                {
                    code = StatusCode.OK;
                }
                loadPage(physicalPath, content);
                response = new Response(code, "text/html", content, redirectionPath, request.httpVersion);
                return response;
                // Create OK response
            }
            catch (Exception ex)
            {
                code = StatusCode.InternalServerError;
                Exception e2 = (Exception)Activator.CreateInstance(ex.GetType(), "Internal Server Error", ex);
                Logger.LogException(e2);
                physicalPath = Configuration.InternalErrorDefaultPageName;
                content = LoadDefaultPage(physicalPath);
                loadPage(physicalPath, content);
                response = new Response(code, "text/html", content, redirectionPath, request.httpVersion);
                return response;
            }
        }

     
        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            string RedirectionPage;
            RedirectionPage = Configuration.RedirectionRules[relativePath];
            return RedirectionPage;
        }
        private void loadPage(string path, string content)
        {
           
            StreamWriter writer = new StreamWriter(path); 
            writer.WriteLine(content);
            writer.Close();
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
