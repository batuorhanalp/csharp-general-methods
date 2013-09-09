using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Facebook
{
    internal enum HttpVerb
    {
        GET,
        POST,
        DELETE
    }

    /// <summary>
    /// Wrapper around the Facebook Graph API. 
    /// </summary>
    public class FacebookAPI
    {
        /// <summary>
        /// Create a new instance of the API, with public access only.
        /// </summary>
        public FacebookAPI()
            : this(null)
        {
        }

        /// <summary>
        /// Create a new instance of the API, using the given token to
        /// authenticate.
        /// </summary>
        /// <param name="token">The access token used for
        /// authentication</param>
        public FacebookAPI(string token)
        {
            AccessToken = token;
        }

        /// <summary>
        /// The access token used to authenticate API calls.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Makes a Facebook Graph API GET request.
        /// </summary>
        /// <param name="relativePath">The path for the call,
        /// e.g. /username</param>
        public JSONObject Get(string relativePath)
        {
            return Call(relativePath, HttpVerb.GET, null);
        }

        /// <summary>
        /// Makes a Facebook Graph API GET request.
        /// </summary>
        /// <param name="relativePath">The path for the call,
        /// e.g. /username</param>
        /// <param name="args">A dictionary of key/value pairs that
        /// will get passed as query arguments.</param>
        public JSONObject Get(string relativePath,
                              Dictionary<string, string> args)
        {
            return Call(relativePath, HttpVerb.GET, args);
        }

        /// <summary>
        /// Makes a Facebook Graph API DELETE request.
        /// </summary>
        /// <param name="relativePath">The path for the call,
        /// e.g. /username</param>
        public JSONObject Delete(string relativePath)
        {
            return Call(relativePath, HttpVerb.DELETE, null);
        }

        /// <summary>
        /// Makes a Facebook Graph API POST request.
        /// </summary>
        /// <param name="relativePath">The path for the call,
        /// e.g. /username</param>
        /// <param name="args">A dictionary of key/value pairs that
        /// will get passed as query arguments. These determine
        /// what will get set in the graph API.</param>
        public JSONObject Post(string relativePath,
                               Dictionary<string, string> args)
        {
            return Call(relativePath, HttpVerb.POST, args);
        }

        /// <summary>
        /// Makes a Facebook Graph API Call.
        /// </summary>
        /// <param name="relativePath">The path for the call, 
        /// e.g. /username</param>
        /// <param name="httpVerb">The HTTP verb to use, e.g.
        /// GET, POST, DELETE</param>
        /// <param name="args">A dictionary of key/value pairs that
        /// will get passed as query arguments.</param>
        private JSONObject Call(string relativePath,
                                HttpVerb httpVerb,
                                Dictionary<string, string> args)
        {
            var baseURL = new Uri("https://graph.facebook.com");
            var url = new Uri(baseURL, relativePath);
            if (args == null)
            {
                args = new Dictionary<string, string>();
            }
            if (!string.IsNullOrEmpty(AccessToken))
            {
                args["access_token"] = AccessToken;
            }
            var obj = JSONObject.CreateFromString(MakeRequest(url,
                                                                     httpVerb,
                                                                     args));
            if (obj.IsDictionary && obj.Dictionary.ContainsKey("error"))
            {
                //throw new FacebookAPIException(obj.Dictionary["error"]
                //                                  .Dictionary["type"]
                //                                  .String,
                //                               obj.Dictionary["error"]
                //                                  .Dictionary["message"]
                //                                  .String);
            }
            return obj;
        }

        /// <summary>
        /// Make an HTTP request, with the given query args
        /// </summary>
        /// <param name="url">The URL of the request</param>
        /// <param name="verb">The HTTP verb to use</param>
        /// <param name="args">Dictionary of key/value pairs that represents
        /// the key/value pairs for the request</param>
        private string MakeRequest(Uri url, HttpVerb httpVerb,
                                   Dictionary<string, string> args)
        {
            if (args != null && args.Keys.Count > 0 && httpVerb == HttpVerb.GET)
            {
                url = new Uri(url + EncodeDictionary(args, true));
            }

            var request = WebRequest.Create(url) as HttpWebRequest;

            request.Method = httpVerb.ToString();

            if (httpVerb == HttpVerb.POST)
            {
                string postData = EncodeDictionary(args, false);

                var encoding = new ASCIIEncoding();
                byte[] postDataBytes = encoding.GetBytes(postData);

                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postDataBytes.Length;

                var requestStream = request.GetRequestStream();
                requestStream.Write(postDataBytes, 0, postDataBytes.Length);
                requestStream.Close();
            }

            try
            {
                using (var response
                    = request.GetResponse() as HttpWebResponse)
                {
                    if (response != null)
                    {
                        var reader
                            = new StreamReader(response.GetResponseStream());

                        return reader.ReadToEnd();
                    }
                }
            }
            catch (WebException e)
            {
                return (e.Message);
                //throw new FacebookAPIException("Server Error", e.Message);
            }
            return null;
        }

        /// <summary>
        /// Encode a dictionary of key/value pairs as an HTTP query string.
        /// </summary>
        /// <param name="dict">The dictionary to encode</param>
        /// <param name="questionMark">Whether or not to start it
        /// with a question mark (for GET requests)</param>
        private string EncodeDictionary(Dictionary<string, string> dict,
                                        bool questionMark)
        {
            var sb = new StringBuilder();
            if (questionMark)
            {
                sb.Append("?");
            }
            foreach (var kvp in dict)
            {
                sb.Append(HttpUtility.UrlEncode(kvp.Key));
                sb.Append("=");
                sb.Append(HttpUtility.UrlEncode(kvp.Value));
                sb.Append("&");
            }
            sb.Remove(sb.Length - 1, 1); // Remove trailing &
            return sb.ToString();
        }
    }
}