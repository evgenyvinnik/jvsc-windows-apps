using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using RestSharp;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Linq;

namespace DropBoxy.Security
{
    public class OAuthenticator : OAuth.OAuthBase,IAuthenticator
    {
        private readonly string _baseUrl;
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly string _token;
        private readonly string _tokenSecret;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthenticator"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL.</param>
        /// <param name="consumerKey">The consumer key.</param>
        /// <param name="consumerSecret">The consumer secret.</param>
        public OAuthenticator(string baseUrl, string consumerKey, string consumerSecret)
            : this(baseUrl, consumerKey, consumerSecret, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthenticator"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL.</param>
        /// <param name="consumerKey">The consumer key.</param>
        /// <param name="consumerSecret">The consumer secret.</param>
        /// <param name="token">The token.</param>
        /// <param name="tokenSecret">The token secret.</param>
        public OAuthenticator(string baseUrl, string consumerKey, string consumerSecret, string token, string tokenSecret)
        {
            this._baseUrl = baseUrl;
            this._consumerKey = consumerKey;
            this._consumerSecret = consumerSecret;
            this._token = token;
            this._tokenSecret = tokenSecret;
        }

        /// <summary>
        /// Authenticates the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void Authenticate(RestRequest request)
        {
            request.AddParameter("oauth_version", "1.0");
            request.AddParameter("oauth_nonce", this.GenerateNonce());
            request.AddParameter("oauth_timestamp", this.GenerateTimeStamp());
            request.AddParameter("oauth_signature_method", "HMAC-SHA1");
            request.AddParameter("oauth_consumer_key", this._consumerKey);
            if (!string.IsNullOrEmpty(this._token))
            {
                request.AddParameter("oauth_token", this._token);
            }
            foreach (Parameter parameter in request.Parameters)
            {
                if ((parameter.Type == ParameterType.GetOrPost) && (parameter.Value is string))
                {
                    //parameter.Value = this.UrlEncode(parameter.Value.ToString());
                }
            }
            request.Parameters.Sort(new QueryParameterComparer());
            request.AddParameter("oauth_signature", this.GenerateSignature(request));
        }

        /// <summary>
        /// Normalizes the request parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        private string NormalizeRequestParameters(IList<Parameter> parameters)
        {
            StringBuilder builder = new StringBuilder();
            List<Parameter> list = parameters.Where<Parameter>(delegate(Parameter param)
            {
                return (param.Type == ParameterType.GetOrPost);
            }).ToList<Parameter>();


            Parameter parameter = null;
            for (int i = 0; i < list.Count; i++)
            {
                parameter = parameters[i];
                builder.AppendFormat("{0}={1}", parameter.Name, UrlEncode(parameter.Value.ToString()));
                if (i < (list.Count - 1))
                {
                    builder.Append("&");
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Generates the signature.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private string GenerateSignature(RestRequest request)
        {
            Uri uri = this.BuildUri(request);
            string str = string.Format("{0}://{1}", uri.Scheme, uri.Host);
            if (((uri.Scheme != "http") || (uri.Port != 80)) && ((uri.Scheme != "https") || (uri.Port != 0x1bb)))
            {
                str = str + ":" + uri.Port;
            }
            str = str + uri.AbsolutePath;
            string str2 = NormalizeRequestParameters(request.Parameters);
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0}&", request.Method.ToString().ToUpper());
            builder.AppendFormat("{0}&", UrlEncode(str));
            builder.AppendFormat("{0}", UrlEncode(str2));
            string data = builder.ToString();
            HMACSHA1 hashAlgorithm = new HMACSHA1();
            hashAlgorithm.Key = Encoding.UTF8.GetBytes(string.Format("{0}&{1}", UrlEncode(this._consumerSecret), string.IsNullOrEmpty(this._tokenSecret) ? string.Empty : UrlEncode(this._tokenSecret)));
            return ComputeHash(hashAlgorithm, data);
        }

        /// <summary>
        /// Builds the URI.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private Uri BuildUri(RestRequest request)
        {
            string resource = request.Resource;
            resource = request.Parameters.Where<Parameter>(delegate(Parameter p)
            {
                return (p.Type == ParameterType.UrlSegment);
            }).Aggregate<Parameter, string>(resource, delegate(string current, Parameter p)
            {
                return current.Replace("{" + p.Name + "}", p.Value.ToString());
            });
            return new Uri(string.Format("{0}/{1}", this._baseUrl, resource));
        }

        /// <summary>
        /// Helper function to compute a hash value
        /// </summary>
        /// <param name="hashAlgorithm">The hashing algoirhtm used. If that algorithm needs some initialization, like HMAC and its derivatives, they should be initialized prior to passing it to this function</param>
        /// <param name="data">The data to hash</param>
        /// <returns>a Base64 string of the hash value</returns>
        private static string ComputeHash(HashAlgorithm hashAlgorithm, string data)
        {
            if (hashAlgorithm == null)
            {
                throw new ArgumentNullException("hashAlgorithm");
            }
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException("data");
            }
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(hashAlgorithm.ComputeHash(bytes));
        }

        // Nested Types
        private class QueryParameterComparer : IComparer<Parameter>
        {
            // Methods
            /// <summary>
            /// Compares the specified x.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <returns></returns>
            public int Compare(Parameter x, Parameter y)
            {
                return ((x.Name == y.Name) ? string.Compare(x.Value.ToString(), y.Value.ToString()) : string.Compare(x.Name, y.Name));
            }
        }
    }

   
}
