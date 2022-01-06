# HTTP-Server (Network Application)

### Requirements
- Implementing part of the HTTP protocol. </br>
&nbsp; &nbsp; &nbsp; - Threaded (multiple clients). </br>
&nbsp; &nbsp; &nbsp; - GET / POST / HEAD Methods. </br>
&nbsp; &nbsp; &nbsp; - Error Handling (Page Not found - Bad Request - Redirection - Internal Server Error). </br>

### Starting the Server
- Accepting multiple clients by starting a thread for each accepted connection.
- Keep on accepting requests from the remote client until the client closes the socket (sends a zero length message).
- For each received request, the server must reply with a response.

### Receiving Request
- The received request must be a valid HTTP request, else return 400 Bad Request. </br>
&nbsp; &nbsp; &nbsp; - Checking single space separating the request line parameters (Method URI HTTPVersion). </br>
&nbsp; &nbsp; &nbsp; - Checking blank line separating the header lines and the content, even if the content is empty. </br>
&nbsp; &nbsp; &nbsp; - Checking valid URI. </br>
&nbsp; &nbsp; &nbsp; - Checking at least request line and host header and blank lines exist.

### Response Headers
- The response should include the following headers: </br>
&nbsp; &nbsp; &nbsp; - Content-Type (We will use only text/html). </br>
&nbsp; &nbsp; &nbsp; - Content-Length (The length of the content). </br>
&nbsp; &nbsp; &nbsp; - Date (Current DateTime of the server). </br>
&nbsp; &nbsp; &nbsp; - Location (Only if there is redirection). </br>

### Handling Request
- Using Configuration.RootPath, map the URI to the physical path (See below line for an Example). </br>
```
configuration.RootPath= “c:\intepub\wwwroot\fcis1”  and URI = “/aboutus.html” then physical path= “c:\intepub\wwwroot\fcis1\aboutus.html”
```

#### Redirection
- If the URI exists in the configuration.RedirectionRules, then return 301 Redirection Error and add location header with the new redirected URI.
- The content should be the content of the static page “redirect.html”.

#### Not Found
- If the physical file is not found return 404 Not Found error.
- The content should be the content of the static page “Notfound.html”

#### Bad Request
- If there is any parsing error in the request, return 400 Bad Request Error.
- The content should be loaded with the content of the static page “BadRequest.html”.

#### Internal Server Error
- If there is any unknown exception, return 500 Internal Server Error.
- The content should be the content of the static page “InternalError.html”.

***

## Getting Started
1) Put **inetpub** folder in **`C Directory`**
2) In **Server** class line **193** change the `rootfolder` to your downloads folder path.
3) In **server** class line **195** change the `xmlDocumentpath` to your xml file path.

## Server Demo
**To Run and Test the HTTP Servver, here are some links to use:** 
1) http://localhost:1000/aboutus.html (redirection)
2) http://localhost:1000/main.html (main)
3) http://localhost:1000/blabla.html  (404 page)
4) http://localhost:1000/formpage.html  (post method try to submit the form and see the changes of the xml file)

### Copyrights
- Nada Mohamed - Nada Anies
- KAN Org.
- University of Ain Shams, Egypt
