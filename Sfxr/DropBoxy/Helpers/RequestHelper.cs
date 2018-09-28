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
using DropBoxy.Models;
using OAuth;

namespace DropBoxy.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class RequestHelper
    {
        /// <summary>
        /// Gets the delete request.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static RestRequest GetDeleteRequest(string path)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = Constants.CONST_DROPBOX_DELETEPATTERN;
            request.AddParameter(Constants.CONST_REQUESTVALUES_VERSION, 
                Constants.CONST_VERSION, ParameterType.UrlSegment);
            request.AddParameter(Constants.CONST_REQUESTVALUES_PATH, path);
            request.AddParameter(Constants.CONST_REQUESTVALUES_ROOT, Constants.CONST_DROPBOX);

            return request;
        }

        /// <summary>
        /// Gets the download request.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static RestRequest GetDownloadRequest(string path)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = Constants.CONST_DROPBOX_FILESPATTERN;
            request.AddParameter(Constants.CONST_REQUESTVALUES_VERSION, 
                Constants.CONST_VERSION, ParameterType.UrlSegment);
            request.AddParameter(Constants.CONST_REQUESTVALUES_PATH, path, ParameterType.UrlSegment);

            return request;
        }

        /// <summary>
        /// Gets the account info request.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <param name="apiSecret">The API secret.</param>
        /// <param name="userInfo">The user info.</param>
        /// <returns></returns>
        public static RestRequest GetAccountInfoRequest(string apiKey, string apiSecret, UserInfo userInfo )
        {
            OAuthBase oAuth = new OAuthBase();

            string nonce = oAuth.GenerateNonce();
            string timeStamp = oAuth.GenerateTimeStamp();
            string normalizedUrl;
            string normalizedRequestParameters;

            string sig = oAuth.GenerateSignature(new Uri
                          (string.Format(Constants.CONST_OAUTH_SIGNATUREPATTERN, Constants.CONST_DROPBOX_BASEURI  , 
                                Constants.CONST_VERSION)),apiKey, apiSecret, userInfo.Token, userInfo.Secret,
                                            Constants.CONST_GET, timeStamp, nonce, out normalizedUrl, 
                                            out normalizedRequestParameters);

            sig = HttpUtility.UrlEncode(sig);

            var request = new RestRequest(Method.GET);
            request.Resource = string.Format(Constants.CONST_DROPBOX_ACCOUNTINFOPATTERN, 
                                                                        Constants.CONST_VERSION);

            request.AddParameter(Constants.CONST_OAUTH_CONSUMERKEY, apiKey);
            request.AddParameter(Constants.CONST_OAUTH_TOKEN, userInfo.Token);
            request.AddParameter(Constants.CONST_OAUTH_NONCE, nonce);
            request.AddParameter(Constants.CONST_OAUTH_TIMESTAMP, timeStamp);
            request.AddParameter(Constants.CONST_OAUTH_SIGNATUREMETHOD, Constants.CONST_OAUTH_ENCRYPTIONMETHOD);
            request.AddParameter(Constants.CONST_OAUTH_VERSION, Constants.CONST_OAUTH_OUATHVERSION);
            request.AddParameter(Constants.CONST_OAUTH_SIGNATURE, sig);

            return request;
        }

        /// <summary>
        /// Gets the login request.
        /// </summary>
        /// <returns></returns>
        public static RestRequest GetLoginRequest( string apiKey, string login, string password)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = Constants.CONST_DROPBOX_LOGINPATTERN;
            request.AddParameter(Constants.CONST_REQUESTVALUES_VERSION,
                                            Constants.CONST_VERSION, ParameterType.UrlSegment);
            request.AddParameter(Constants.CONST_OAUTH_CONSUMERKEY, apiKey);
            request.AddParameter(Constants.CONST_REQUESTVALUES_EMAIL, login);
            request.AddParameter(Constants.CONST_REQUESTVALUES_PASSWORD, password);

            return request;
        }

        /// <summary>
        /// Gets the upload file request.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static RestRequest GetUploadFileRequest( string path, string fileName)
        {
            var request = new RestRequest(Method.POST);
            request.Resource = Constants.CONST_DROPBOX_FILESPATTERN;
            request.AddParameter(Constants.CONST_REQUESTVALUES_VERSION, 
                Constants.CONST_VERSION, ParameterType.UrlSegment);
            request.AddParameter(Constants.CONST_REQUESTVALUES_PATH, path, ParameterType.UrlSegment);
            request.AddParameter(Constants.CONST_REQUESTVALUES_FILE, fileName);

            return request;
        }

        /// <summary>
        /// Gets the copy request.
        /// </summary>
        /// <param name="fromPath">From path.</param>
        /// <param name="toPath">To path.</param>
        /// <returns></returns>
        public static RestRequest GetCopyRequest( string fromPath, string toPath)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = Constants.CONST_DROPBOX_COPY;
            request.AddParameter(Constants.CONST_REQUESTVALUES_VERSION,
                Constants.CONST_VERSION, ParameterType.UrlSegment);
            request.AddParameter(Constants.CONST_REQUESTVALUES_FROMPATH, fromPath);
            request.AddParameter(Constants.CONST_REQUESTVALUES_TOPATH, toPath);
            request.AddParameter(Constants.CONST_REQUESTVALUES_ROOT, Constants.CONST_DROPBOX);

            return request;
        }


        /// <summary>
        /// Gets the move request.
        /// </summary>
        /// <param name="fromPath">From path.</param>
        /// <param name="toPath">To path.</param>
        /// <returns></returns>
        public static RestRequest GetCreateFolderRequest(string path)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = Constants.CONST_DROPBOX_CREATEFOLDER;
            request.AddParameter(Constants.CONST_REQUESTVALUES_VERSION,
                Constants.CONST_VERSION, ParameterType.UrlSegment);
            request.AddParameter(Constants.CONST_REQUESTVALUES_PATH, path);
            request.AddParameter(Constants.CONST_REQUESTVALUES_ROOT, Constants.CONST_DROPBOX);

            return request;
        }
        
        /// <summary>
        /// Gets the move request.
        /// </summary>
        /// <param name="fromPath">From path.</param>
        /// <param name="toPath">To path.</param>
        /// <returns></returns>
        public static RestRequest GetMoveRequest(string fromPath, string toPath)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = Constants.CONST_DROPBOX_MOVE;
            request.AddParameter(Constants.CONST_REQUESTVALUES_VERSION,
                Constants.CONST_VERSION, ParameterType.UrlSegment);
            request.AddParameter(Constants.CONST_REQUESTVALUES_FROMPATH, fromPath);
            request.AddParameter(Constants.CONST_REQUESTVALUES_TOPATH, toPath);
            request.AddParameter(Constants.CONST_REQUESTVALUES_ROOT, Constants.CONST_DROPBOX);

            return request;
        }

        /// <summary>
        /// Gets the metedata request.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static RestRequest GetMetedataRequest(string path)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = Constants.CONST_DROPBOX_METADATA;
            request.AddParameter(Constants.CONST_REQUESTVALUES_VERSION,
                Constants.CONST_VERSION, ParameterType.UrlSegment);
            
            request.AddParameter(Constants.CONST_REQUESTVALUES_PATH,
                path, ParameterType.UrlSegment);

            return request;
        }
    }
}
