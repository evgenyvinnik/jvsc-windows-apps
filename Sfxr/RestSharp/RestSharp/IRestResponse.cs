#region License
//   Copyright 2010 John Sheehan
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
#endregion

using System;
using System.Collections.Generic;
using System.Net;

namespace RestSharp
{
    /// <summary>
    /// Container for data sent back from API
    /// </summary>
    public interface IRestResponse
    {
        /// <summary>
        /// The RestRequest that was made to get this RestResponse
        /// </summary>
        /// <remarks>
        /// Mainly for debugging if ResponseStatus is not OK
        /// </remarks> 
        RestRequest Request { get; set; }

        /// <summary>
        /// MIME content type of response
        /// </summary>
        string ContentType { get; set; }

        /// <summary>
        /// Length in bytes of the response content
        /// </summary>
        long ContentLength { get; set; }

        /// <summary>
        /// Encoding of the response content
        /// </summary>
        string ContentEncoding { get; set; }

        /// <summary>
        /// String representation of response content
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// HTTP response status code
        /// </summary>
        HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Description of HTTP status returned
        /// </summary>
        string StatusDescription { get; set; }

        /// <summary>
        /// Response content
        /// </summary>
        byte[] RawBytes { get; set; }

        /// <summary>
        /// The URL that actually responded to the content (different from request if redirected)
        /// </summary>
        Uri ResponseUri { get; set; }

        /// <summary>
        /// HttpWebResponse.Server
        /// </summary>
        string Server { get; set; }

        /// <summary>
        /// Cookies returned by server with the response
        /// </summary>
        IList<Parameter> Cookies { get; }

        /// <summary>
        /// Headers returned by server with the response
        /// </summary>
        IList<Parameter> Headers { get; }

        /// <summary>
        /// Status of the request. Will return Error for transport errors.
        /// HTTP errors will still return ResponseStatus.Completed, check StatusCode instead
        /// </summary>
        ResponseStatus ResponseStatus { get; set; }

        /// <summary>
        /// Transport or other non-HTTP error generated while attempting request
        /// </summary>
        string ErrorMessage { get; set; }

        /// <summary>
        /// The exception thrown during the request, if any
        /// </summary>
        Exception ErrorException { get; set; }
    }

    /// <summary>
    /// Container for data sent back from API including deserialized data
    /// </summary>
    /// <typeparam name="T">Type of data to deserialize to</typeparam>
    public interface IRestResponse<T> : IRestResponse
    {
        /// <summary>
        /// Deserialized entity data
        /// </summary>
        T Data { get; set; }
    }
}