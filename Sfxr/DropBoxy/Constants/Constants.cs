namespace DropBoxy
{
    public static class Constants
    {
        #region VALUES
        internal const string CONST_VERSION = "0";

        internal const string CONST_DROPBOX = "dropbox";

        internal const string CONST_SANDBOX = "sandbox";

        internal const string CONST_GET = "GET";
        #endregion

        #region REQUESTPATTERNS
        internal const string CONST_DROPBOX_DELETEPATTERN = "{version}/fileops/delete";

        internal const string CONST_DROPBOX_FILESPATTERN = "{version}/files/dropbox{path}";

        internal const string CONST_DROPBOX_ACCOUNTINFOPATTERN = "{0}/account/info";

        internal const string CONST_DROPBOX_LOGINPATTERN = "{version}/token";

        internal const string CONST_DROPBOX_COPY = "{version}/fileops/copy";

        internal const string CONST_DROPBOX_MOVE = "{version}/fileops/move";

        internal const string CONST_DROPBOX_CREATEFOLDER = "{version}/fileops/create_folder";

        internal const string CONST_DROPBOX_METADATA = "{version}/metadata/dropbox{path}";
        #endregion

        #region DROPBOXURI
        internal const string CONST_DROPBOX_BASEURI = "http://api.dropbox.com";

        internal const string CONST_DROPBOX_STORAGEURI = "http://api-content.dropbox.com";
        #endregion

        #region REQUESTVALUES
        internal const string CONST_REQUESTVALUES_VERSION = "version";
        
        internal const string CONST_REQUESTVALUES_PATH = "path";

        internal const string CONST_REQUESTVALUES_FILE = "file";

        internal const string CONST_REQUESTVALUES_EMAIL = "email";
        
        internal const string CONST_REQUESTVALUES_PASSWORD = "password";

        internal const string CONST_REQUESTVALUES_ROOT = "root";

        internal const string CONST_REQUESTVALUES_FROMPATH = "from_path";

        internal const string CONST_REQUESTVALUES_TOPATH = "to_path";
        #endregion

        #region OAUTHCONSTANTS
        internal const string CONST_OAUTH_CONSUMERKEY = "oauth_consumer_key";

        internal const string CONST_OAUTH_TOKEN = "oauth_token";

        internal const string CONST_OAUTH_NONCE = "oauth_nonce";

        internal const string CONST_OAUTH_TIMESTAMP = "oauth_timestamp";

        internal const string CONST_OAUTH_SIGNATUREMETHOD = "oauth_signature_method";

        internal const string CONST_OAUTH_VERSION = "oauth_version";

        internal const string CONST_OAUTH_SIGNATURE = "oauth_signature";

        internal const string CONST_OAUTH_OUATHVERSION = "1.0";

        internal const string CONST_OAUTH_ENCRYPTIONMETHOD = "HMAC-SHA1";

        internal const string CONST_OAUTH_SIGNATUREPATTERN = "{0}/{1}/account/info";
        #endregion
    }
}