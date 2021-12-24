using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

   
    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        List<string> headerLines = new List<string>();
        
        public Response(StatusCode code, string contentType, string content, string redirectoinPath , HTTPVersion hTTPVersion)
        {
            // create status line
            string statusLine = GetStatusLine(code , hTTPVersion);
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            DateTime dateTime =  DateTime.Now;
            headerLines.Add(contentType);
            headerLines.Add(content.Length.ToString());
            headerLines.Add(dateTime.ToString());
            string Headers = "";
            Headers = "Content-Type: " + headerLines[0] + "\r\n"
                + "Content-Length: " + headerLines[1] + "\r\n"
                + "Date: " + headerLines[2];

            if (!string.IsNullOrEmpty(redirectoinPath))
            {
                headerLines.Add(redirectoinPath);
                Headers = Headers +"\r\n"+ "location: " + headerLines[3];
            }
            // TODO: Create the request string
            string BlankLine = "\r\n";

            responseString = statusLine + "\r\n" + Headers + "\r\n" + BlankLine + content;     
        }

        private string GetStatusLine(StatusCode code , HTTPVersion hTTPVersion)
        {
            // TODO: Create the response status line and return it
            string version;
            switch (hTTPVersion)
            {
                case HTTPVersion.HTTP10:
                    version = "HTTP/1.0";
                    break;
                case HTTPVersion.HTTP11:
                    version = "HTTP/1.1";
                    break;
                default:
                    version = "HTTP/0.9";
                    break;
            }
            string CodeInString = code.GetHashCode().ToString();

            string Message = Enum.GetName(typeof(StatusCode), code);
            
            string statusLine = version + " " + CodeInString + " " + Message ;
            return statusLine;
        }
       
        }
}
