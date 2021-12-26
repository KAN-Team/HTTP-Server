using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        public RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        public HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        { 
            //TODO: parse the receivedRequest using the \r\n delimeter
            string[] stringSeparators = new string[] { "\r\n" };
            requestLines = requestString.Split(stringSeparators, StringSplitOptions.None);
            
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length < 3) return false;

            return (ParseRequestLine() && LoadHeaderLines() && ValidateBlankLine() && ValidateContent());
        }

        private bool ParseRequestLine()
        {
            string[] ReqLineDetails = requestLines[0].Split(' ');

            bool MethodValid = true;
            switch (ReqLineDetails[0])
            {
                case "GET":
                    method = RequestMethod.GET;
                    break;
                case "POST":
                    method = RequestMethod.POST;
                    break;
                case "HEAD":
                    method = RequestMethod.HEAD;
                    break;
                default:
                    MethodValid = false;
                    break;
            }
            
            bool uri = ValidateIsURI(ReqLineDetails[1]);
            relativeURI = ReqLineDetails[1];

            switch (ReqLineDetails[2]) {
                case "HTTP/1.0":
                    httpVersion = HTTPVersion.HTTP10;
                    break;
                case "HTTP/1.1":
                    httpVersion = HTTPVersion.HTTP11;
                    break;
                default:
                    httpVersion = HTTPVersion.HTTP09;
                    break;

            }
            return (MethodValid && uri );
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            headerLines = new Dictionary<string, string>();
            string[] stringSeparators = new string[] { "\r\n" };
            string [] headers = requestLines[1].Split(stringSeparators, StringSplitOptions.None);

            foreach(string line in headers )
            {
                 stringSeparators = new string[] { ": " };
                string[] Head = line.Split(stringSeparators, StringSplitOptions.None);
                headerLines.Add(Head[0],Head[1]);
            }

            if (HeaderLines.Count != headers.Length) return false;

            //HTTP 1.1 requires a Host: header
            if (httpVersion.Equals(HTTPVersion.HTTP11)) {
                if (!headerLines.ContainsKey("Host")) return false;
            }
            return true;
        }

        private bool ValidateBlankLine()
        {
            return string.IsNullOrEmpty(requestLines[requestLines.Length-1]);
        }
        //bonus
        private bool ValidateContent() {

            string[] stringSeparators = new string[] { "\r\n" };
            contentLines = requestLines[3].Split(stringSeparators, StringSplitOptions.None);

            switch (method) {

                //Content is empty when the method is Get or Head
                case RequestMethod.HEAD:
                case RequestMethod.GET:
                    if (string.IsNullOrEmpty(requestLines[3])) return true;
                    break;

                case RequestMethod.POST:
                    //A POST request must include a Content-Length and Content-Type line in the headers
                    //Content only takes values with Post method
                    if (!string.IsNullOrEmpty(requestLines[3]) && headerLines.ContainsKey("Content-length")&& 
                        (headerLines.ContainsKey("Content-Type")|| headerLines.ContainsKey("Transfer-Encoding"))) 
                        return true;
                    break;
            }          
            return true;
        }
    }
}
