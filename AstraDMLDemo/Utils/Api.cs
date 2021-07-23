using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AstraDMLDemo.Utils
{
    public class Api
    {
        /// <summary>
        /// HTTP Get
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static IRestResponse Get(string url, string token)
        {
            try
            {
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-Cassandra-Token", token);
                request.AddHeader("Content-Type", "application/json");
                IRestResponse response = client.Execute(request);

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// HTTP Post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IRestResponse Post(string url, string token, string data)
        {
            try
            {
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("X-Cassandra-Token", token);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", data, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// HTTP Post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IRestResponse GetToken(string url, string username, string password)
        {
            try
            {
                // create the authorization body
                JObject o = new JObject();
                o.Add("username", username);
                o.Add("password", password);

                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", o.ToString(), ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// HTTP Delete
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static IRestResponse Delete(string url, string token)
        {
            try
            {
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.DELETE);
                request.AddHeader("X-Cassandra-Token", token);
                request.AddHeader("Content-Type", "application/json");
                IRestResponse response = client.Execute(request);

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// HTTP Put
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IRestResponse Put(string url, string token, string data)
        {
            try
            {
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.PUT);
                request.AddHeader("X-Cassandra-Token", token);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", data, ParameterType.RequestBody); 

                //Call API
                IRestResponse response = client.Execute(request);

                return response;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// HTTP Patch
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IRestResponse Patch(string url, string token, string data)
        {
            try
            {
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.PATCH);
                request.AddHeader("X-Cassandra-Token", token);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", data, ParameterType.RequestBody);

                //Call API
                IRestResponse response = client.Execute(request);

                return response;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
