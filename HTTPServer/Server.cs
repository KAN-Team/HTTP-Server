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
            byte[] data;
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
                        Console.WriteLine("Client: {0} ended the connection", clientSock.RemoteEndPoint);
                        break;
                    }
                    // TODO: Create a Request object using received request string
                    string RequestString = BitConverter.ToString(data);
                    Request request = new Request(RequestString);
                    // TODO: Call HandleRequest Method that returns the response
                    Response response = HandleRequest(request);
                    // TODO: Send Response back to client

                    //If the message length is ZERO, means client has Closed the connection
                    //Then Close the connection with this client

                    //Else, display the message on the console window
                }



                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                }
            }

            // TODO: close client socket
            clientSock.Close();
        }
        StatusCode code;
        Response HandleRequest(Request request)
        { 
            bool ValidRequest;
            Response response;
            string responsecontent ; 
            string PhysicalPath , RedirectionPage = string.Empty;
            try
            {
                //TODO: check for bad request 
                ///
                ValidRequest = request.ParseRequest();
                if (!ValidRequest) {
                    code = StatusCode.BadRequest;
                    StreamReader sr = new StreamReader(Configuration.BadRequestDefaultPageName);
                    responsecontent = sr.ReadToEnd();
                    response = new Response(code, "text/html", responsecontent,RedirectionPage);
                    return response;
                }
                //////////////////////
                //TODO: map the relativeURI in request to get the physical path of the resource.
                PhysicalPath = request.relativeURI;
                //TODO: check for redirect
                if (Configuration.RedirectionRules.ContainsKey(PhysicalPath))
                {
                   RedirectionPage= GetRedirectionPagePathIFExist(PhysicalPath);
                   code = StatusCode.Redirect;
                   StreamReader sr = new StreamReader(Configuration.RedirectionDefaultPageName);
                   responsecontent = sr.ReadToEnd();
                   response = new Response(code, "text/html", responsecontent, RedirectionPage);
                   return response;
                }
                //TODO: check file exist
                //TODO: read the physical file
                // Create OK response
                responsecontent = LoadDefaultPage(PhysicalPath);              
                if(string.IsNullOrEmpty(responsecontent))
                {
                    StreamReader sr = new StreamReader(Configuration.NotFoundDefaultPageName);
                    responsecontent = sr.ReadToEnd();
                    code = StatusCode.NotFound;
                    response = new Response(code, "text/html", responsecontent, RedirectionPage);
                    return response;
                }
                else
                {
                    code = StatusCode.OK;
                    response = new Response(code, "text/html", responsecontent, RedirectionPage);
                    return response;
                }
                
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                // TODO: in case of exception, return Internal Server Error. 
                code = StatusCode.InternalServerError;
                Exception e2 = (Exception)Activator.CreateInstance(ex.GetType(), "Internal Server Error", ex);
                Logger.LogException(e2);
                StreamReader sr = new StreamReader(Configuration.InternalErrorDefaultPageName);
                responsecontent = sr.ReadToEnd();
                response = new Response(code, "text/html", responsecontent, RedirectionPage);
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

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            try
            {
                StreamReader sr = new StreamReader(filePath);
                string content = sr.ReadToEnd();               
                return content;
            }
            catch (Exception ex)
            {
                
                Exception e2 = (Exception)Activator.CreateInstance(ex.GetType(), "page not found", ex);          
                Logger.LogException(e2);
               
                return string.Empty;
            }
          
            // else read file and return its content
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
