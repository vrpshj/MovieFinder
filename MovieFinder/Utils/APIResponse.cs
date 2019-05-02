using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MovieFinder.Utils
{
    /// <summary>
    /// Generic response object
    /// </summary>
    /// <typeparam name="T">Data type of the response object</typeparam>
    public class ApiResponse<T> : ApiResponse
    {
        /// <summary>
        /// Retrieve the data as a given type
        /// </summary>
        /// <typeparam>Type of the data that ApiResponse contains</typeparam>
        /// <returns></returns>
        public T TryGetData()
        {
            try
            {
                return (T)data;
            }
            catch
            {
                return default(T);
            }
        }
    }
    /// <summary>
    /// Generic response object
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Response status of the API request
        /// </summary>
        public bool success { get; set; } = true;

        /// <summary>
        /// Error details related to the request. This will contain list of ApiResponseError objects
        /// </summary>
        public List<ApiResponseError> errors { get; set; } = new List<ApiResponseError>();

        /// <summary>
        /// Data related to the response. Example, objets.
        /// </summary>
        public object data { get; set; }
        /// <summary>
        /// This method add sever error to response
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ApiResponse AddServerError(string message = null)
        {
            success = false;
            errors.Add(new ApiResponseError() { error_code = string.Empty, error_message = string.IsNullOrWhiteSpace(message) ? "Internal Server Error" : message });

            return this;
        }
        /// <summary>
        /// This method adds error to response
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>

        public ApiResponse AddError(string message)
        {
            success = false;
            errors.Add(new ApiResponseError() { error_code = string.Empty, error_message = message });

            return this;
        }
        /// <summary>
        /// This method adds error to response
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public ApiResponse AddError(string code, string message)
        {
            success = false;
            errors.Add(new ApiResponseError() { error_code = code, error_message = message });

            return this;
        }
        /// <summary>
        /// This method adds error to response
        /// </summary>
        /// <param name="request"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public HttpResponseMessage CreateErrorResponse(HttpRequestMessage request, string code, string message)
        {
            success = false;
            errors.Add(new ApiResponseError() { error_code = code, error_message = message });

            return request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
    /// <summary>
    /// API response error object
    /// </summary>
    public class ApiResponseError
    {
        /// <summary>
        /// Error code
        /// </summary>
        public string error_code { get; set; }

        /// <summary>
        /// Error details
        /// </summary>
        public string error_message { get; set; }
    }
}
