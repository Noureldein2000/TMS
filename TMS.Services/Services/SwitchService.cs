using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using TMS.Infrastructure.Helpers;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public class SwitchService : ISwitchService
    {
        public string Connect<T>(T obj, SwitchEndPointDTO PSC, string baseAddress, string tokenType)
        {
            try
            {
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                //var url = $"{PSC.URL}{baseAddress}";
                //var request = new RestRequest($"/btech/rest/service/{baseAddress}", Method.POST);
                //request.RequestFormat = DataFormat.Json;
                //request.Timeout = PSC.TimeOut;
                //if (tokenType.Contains("Bearer"))
                //    request.AddHeader("Authorization", tokenType + (PSC.UserPassword));
                //else
                //    request.AddHeader("Authorization", tokenType + Convert.ToBase64String(ASCII.GetBytes(PSC.UserName + ":" + PSC.UserPassword)));
                ////request.AddParameter("application/json", JsonConvert.SerializeObject(obj), ParameterType.RequestBody);
                //request.AddBody(obj);
                //var client = new RestClient();
                //client.BaseUrl = new Uri("http://164.160.104.66:7001");
                //var response = client.Execute(request);
                //return "";

                //PSC.URL = "http://164.160.104.66:7001/btech/rest/service/";
                var splitedUrl = PSC.URL.Split('/');
                var newUrl = $"http://164.160.104.66:7001/{splitedUrl[3]}/{splitedUrl[4]}/{splitedUrl[5]}/";
                PSC.URL = newUrl;
                var http = (HttpWebRequest)WebRequest.Create(new Uri(PSC.URL + baseAddress));
                if (tokenType.Contains("Bearer"))
                    http.Headers.Add(HttpRequestHeader.Authorization, tokenType + (PSC.UserPassword));
                else
                    http.Headers.Add(HttpRequestHeader.Authorization, tokenType + Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(PSC.UserName + ":" + PSC.UserPassword)));
                http.ContentType = "application/json";
                http.Method = "Post";
                http.Timeout = PSC.TimeOut;

                using (var streamWriter = new StreamWriter(http.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings { 
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }); //new JavaScriptSerializer().Serialize(obj);

                    streamWriter.Write(json);
                }

                HttpStatusCode statusCode;


                var httpResponse = (HttpWebResponse)http.GetResponse();
                statusCode = httpResponse.StatusCode;
                using var streamReader = new StreamReader(httpResponse.GetResponseStream());
                return streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                //return ex.Message;
                throw new TMSException(ex.Message, "");
            }
           
        }
    }
}
