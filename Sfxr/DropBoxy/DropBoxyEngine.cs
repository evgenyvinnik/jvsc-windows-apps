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
using System.IO.IsolatedStorage;
using OAuth;
using RestSharp;
using RestSharp.Deserializers;
using DropBoxy.Models;
using DropBoxy.Security;


namespace DropBoxy
{
    /// <summary>
    /// 
    /// </summary>
    public class DropBoxyEngine
    {
        #region Members
        private string apiKey;
        private string apiSecret;
        private UserInfo userInfo;
        private AccountInfo accInfo;
        private PublishFileInfo fileToPublish;
        #endregion

        #region Properties
        /// <summary>
        /// Sets the API secret.
        /// </summary>
        /// <value>The API secret.</value>
        public string ApiSecret
        {
            set { this.apiSecret = value; }
        }

        /// <summary>
        /// Sets the API key.
        /// </summary>
        /// <value>The API key.</value>
        public string ApiKey
        {
            set { this.apiKey = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Logins this instance.
        /// </summary>
        public void Login( string login, string password, Action<UserInfo> callback)
        {
            var restClient = new RestClient(Constants.CONST_DROPBOX_BASEURI);
            var request = Helpers.RequestHelper.GetLoginRequest(this.apiKey, login, password);
            
            restClient.ExecuteAsync<UserInfo>(request,(response) => {
                if (string.IsNullOrEmpty(response.ErrorMessage))
                {
                    userInfo = response.Data;
                    callback(response.Data);
                }
                else
                {
                    callback(null);
                }
            });
        }

        /// <summary>
        /// Retrieves the user info.
        /// </summary>
        public void RetrieveUserInfo(Action<AccountInfo> callback)
        {
            var restClient = new RestClient(Constants.CONST_DROPBOX_BASEURI);
            var request = Helpers.RequestHelper.GetAccountInfoRequest(this.apiKey, this.apiSecret, userInfo) ;

            restClient.ExecuteAsync<AccountInfo>(request, (response) => {
                if (string.IsNullOrEmpty(response.ErrorMessage))
                {
                    accInfo = response.Data;
                    if (callback != null)
                        callback(accInfo);
                }
                else
                    throw new Exception(response.ErrorMessage);
            });
        }

        /// <summary>
        /// Downloads the file.
        /// </summary>
        public void Delete(string path, Action<bool> callback)
        {
            path = CheckPath(path);
            var restClient = new RestClient(Constants.CONST_DROPBOX_BASEURI);

            restClient.Authenticator = new OAuthenticator(restClient.BaseUrl, this.apiKey, this.apiSecret, 
                                                                        userInfo.Token, userInfo.Secret);

            var request = Helpers.RequestHelper.GetDeleteRequest(path);
            restClient.ExecuteAsync(request, (response) =>
                {
                    if (string.IsNullOrEmpty(response.ErrorMessage))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            callback(true);
                        }
                        else
                        {
                            throw new Exception(response.Content);
                        }
                    }
                    else
                    {
                        throw new Exception(response.ErrorMessage);
                    }
                });
        }

        /// <summary>
        /// Downloads the file.
        /// </summary>
        public void DownloadFile( string path, Action<byte[]> callback)
        {
            path = CheckPath(path);
            var restClient = new RestClient("http://api-content.dropbox.com");

            restClient.Authenticator = new OAuthenticator(restClient.BaseUrl, this.apiKey, this.apiSecret, userInfo.Token, userInfo.Secret);

            var request = Helpers.RequestHelper.GetDownloadRequest(path);
            restClient.ExecuteAsync(request, (response) =>
                {
                    if (string.IsNullOrEmpty(response.ErrorMessage))
                        callback(response.RawBytes);
                    else
                        throw new Exception(response.ErrorMessage);
                });
        }

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="pathToFile">The path to file.</param>
        /// <param name="callback">The callback.</param>
        public void UploadFile( string path, string fileName, byte[] content, Action<bool> callback)
        {
            if ( !string.IsNullOrEmpty(path))
                path = CheckPath(path);
            var restClient = new RestClient(Constants.CONST_DROPBOX_STORAGEURI);
            restClient.ClearHandlers();
            restClient.AddHandler("*", new JsonDeserializer());
            restClient.Authenticator = new OAuthenticator(restClient.BaseUrl, this.apiKey, this.apiSecret,
                                                                userInfo.Token, userInfo.Secret);
            
            var request = Helpers.RequestHelper.GetUploadFileRequest(path, fileName);

            request.AddFile(content, fileName, null);
            restClient.ExecuteAsync(request, (response) =>
                {
                    if (string.IsNullOrEmpty(response.ErrorMessage))
                    {
                        if (response.Content.Contains("winner"))
                        {
                            callback(true);
                        }
                        else
                        {
                            throw new Exception(response.Content);
                        }
                    }
                    else
                        throw new Exception(response.ErrorMessage);
                });
        }

        /// <summary>
        /// Copies the specified from.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="callback">The callback.</param>
        public void Copy(string from, string to, Action<MetaData> callback)
        {
            from = CheckPath(from);
            to = CheckPath(to);

            var restClient = new RestClient(Constants.CONST_DROPBOX_BASEURI);
            restClient.ClearHandlers();
            restClient.AddHandler("*", new JsonDeserializer());
            restClient.Authenticator = new OAuthenticator(restClient.BaseUrl, this.apiKey, this.apiSecret,
                                                                userInfo.Token, userInfo.Secret);

            var request = Helpers.RequestHelper.GetCopyRequest(from, to);

            ExecuteMetadataRequest(restClient, request, callback);
        }

        /// <summary>
        /// Moves the specified from.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        public void Move(string from, string to, Action<MetaData> callback)
        {
            from = CheckPath(from);
            to = CheckPath(to);

            var restClient = new RestClient(Constants.CONST_DROPBOX_BASEURI);
            restClient.ClearHandlers();
            restClient.AddHandler("*", new JsonDeserializer());
            restClient.Authenticator = new OAuthenticator(restClient.BaseUrl, this.apiKey, this.apiSecret,
                                                                userInfo.Token, userInfo.Secret);

            var request = Helpers.RequestHelper.GetMoveRequest(from, to);
            ExecuteMetadataRequest(restClient, request, callback);
        }


        /// <summary>
        /// Creates the fodler.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="callback">The callback.</param>
        public void CreateFolder( string path, Action<MetaData> callback)
        {
            path = CheckPath(path);

            var restClient = new RestClient(Constants.CONST_DROPBOX_BASEURI);
            restClient.ClearHandlers();
            restClient.AddHandler("*", new JsonDeserializer());
            restClient.Authenticator = new OAuthenticator(restClient.BaseUrl, this.apiKey, this.apiSecret,
                                                                userInfo.Token, userInfo.Secret);

            var request = Helpers.RequestHelper.GetCreateFolderRequest(path);

            ExecuteMetadataRequest(restClient, request, callback);

        }

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="callback">The callback.</param>
        public void GetMetadata(string path, Action<MetaData> callback)
        {
            if (!string.IsNullOrEmpty(path))
                 path = CheckPath(path);

            var restClient = new RestClient(Constants.CONST_DROPBOX_BASEURI);
            restClient.ClearHandlers();
            restClient.AddHandler("*", new JsonDeserializer());
            restClient.Authenticator = new OAuthenticator(restClient.BaseUrl, this.apiKey, this.apiSecret,
                                                                userInfo.Token, userInfo.Secret);

            var request = Helpers.RequestHelper.GetMetedataRequest(path);

            ExecuteMetadataRequest(restClient, request, callback);
        }


        /// <summary>
        /// Publishes the file.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="callback">The callback.</param>
        /// <returns></returns>
        public void PublishFile( byte[] content, string filePath, Action<string> callback)
        {
            this.fileToPublish = new PublishFileInfo()
            {
                Callback = callback,
                Content = content,
                FilePath = CheckPath(filePath)
            };

            if (this.accInfo == null)
            {
                Action<AccountInfo> accUpdate = ContinuePublish;
                RetrieveUserInfo(accUpdate);
            }
            else
            { 
               ContinuePublish( this.accInfo );
            }
        }

        /// <summary>
        /// Gets the public link.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="callback">The callback.</param>
        public void GetPublicLink(string filePath, Action<string> callback)
        {
            this.fileToPublish = new PublishFileInfo()
            {
                Callback = callback,
                FilePath = CheckPath(filePath)
            };

            if (!fileToPublish.FilePath.StartsWith("/Public", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception("You can use only files from public folder!");
            }

            if (this.accInfo == null)
            {
                Action<AccountInfo> accUpdate = ContinueRetrievingLink;
                RetrieveUserInfo(accUpdate);
            }


        }

        /// <summary>
        /// Sets the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="secret">The secret.</param>
        public void SetToken( string token, string secret)
        {
            this.userInfo = new UserInfo()
            {
                Secret = secret,
                Token = token
            };
        }
        #endregion

        /// <summary>
        /// Executes the metadata request.
        /// </summary>
        /// <param name="restClient">The rest client.</param>
        /// <param name="request">The request.</param>
        /// <param name="callback">The callback.</param>
        private void ExecuteMetadataRequest( RestClient restClient, RestRequest request, Action<MetaData> callback)
        {
            restClient.ExecuteAsync<MetaData>(request, (response) =>
            {
                if (string.IsNullOrEmpty(response.ErrorMessage))
                {
                    if (response.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        callback(response.Data);
                    }
                    else
                    {
                        throw new Exception(response.Content);
                    }
                }
                else
                    throw new Exception(response.ErrorMessage);
            });
        }

        /// <summary>
        /// Checks the path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private string CheckPath(string path)
        {
            if (!path.StartsWith("/")) 
                return string.Format("{0}{1}", "/", path);
            return path;
        }
        
        /// <summary>
        /// Continues the publish.
        /// </summary>
        /// <param name="info">The info.</param>
        private void ContinuePublish(AccountInfo info)
        {
            Action<bool> status = PublishCompleted;

            string fileName = System.IO.Path.GetFileName(this.fileToPublish.FilePath);
            string folder = string.Format("Public{0}",
                System.IO.Path.GetDirectoryName(this.fileToPublish.FilePath).Replace('\\', '/'));

            UploadFile(folder, fileName, this.fileToPublish.Content, status);
        }

        /// <summary>
        /// Continues the retrieving link.
        /// </summary>
        /// <param name="info">The info.</param>
        private void ContinueRetrievingLink(AccountInfo info)
        {
            Action<MetaData> retrievingCompleted = RetrivingLinkCompleted;

            GetMetadata(this.fileToPublish.FilePath, retrievingCompleted);
        }

        /// <summary>
        /// Retrivings the link completed.
        /// </summary>
        /// <param name="data">The data.</param>
        private void RetrivingLinkCompleted(MetaData data)
        {
            if (data.Is_Dir)
            {
                throw new Exception("You can publish only files");
            }

            else
            {
                //removing "/public"
                string pathToFile = data.Path.Substring(8);

                string uri = string.Format("http://dl.dropbox.com/u/{0}/{1}", accInfo.Uid, pathToFile);
                this.fileToPublish.Callback(uri);
            }
        }
        /// <summary>
        /// Publishes the success.
        /// </summary>
        /// <param name="status">if set to <c>true</c> [status].</param>
        private void PublishCompleted(bool status)
        {
            if (status)
            {
                string uri = string.Format("http://dl.dropbox.com/u/{0}/{1}", accInfo.Uid, fileToPublish.FilePath);
                fileToPublish.Callback(uri);
            }
            else
            { 
                //todo: drop exception here
            }
        }


    }
}
