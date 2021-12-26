using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using TMS.Infrastructure;
using TMS.Infrastructure.Helpers;
using TMS.Infrastructure.Utils;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public class SwitchService : ISwitchService
    {
        private readonly bool _isEnvirnomentDevelopment;
        public SwitchService(bool isEnvirnomentDevelopment)
        {
            _isEnvirnomentDevelopment = isEnvirnomentDevelopment;
        }
        public string Connect<T>(T obj, SwitchEndPointDTO PSC, string baseAddress, string tokenType, UrlType urlType = UrlType.Custom)
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
                var newUrl = "";
                var splitedUrl = PSC.URL.Split('/');
                if (urlType == UrlType.Fixed)
                {
                    splitedUrl[2] = "164.160.104.66:7001";
                    PSC.URL = $"http://164.160.104.66:7001/{splitedUrl[3]}/{splitedUrl[4]}";
                }
                else
                {
                    if (splitedUrl.Length > 5)
                        newUrl = $"http://164.160.104.66:7001/{splitedUrl[3]}/{splitedUrl[4]}/{splitedUrl[5]}/";
                    else
                        newUrl = $"http://164.160.104.66:7001/{splitedUrl[3]}/{splitedUrl[4]}";
                    PSC.URL = newUrl;
                }
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
                    string json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
                    {
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

        public string Connect(SwitchEndPointDTO PSC, string baseAddress, UrlType urlType = UrlType.Custom)
        {

            try
            {
                //var splitedUrl = PSC.URL.Split('/');
                //var newUrl = $"http://164.160.104.66:7001/{splitedUrl[3]}/{splitedUrl[4]}/{splitedUrl[5]}/";
                //PSC.URL = newUrl;
                string newUrl = baseAddress;

                if(_isEnvirnomentDevelopment)
                {
                    var splitedUrl = baseAddress.Split('/');

                    if (splitedUrl.Length > 5)
                        newUrl = $"http://164.160.104.66:7001/{splitedUrl[3]}/{splitedUrl[4]}?{splitedUrl[5]}";
                    else
                        if (splitedUrl.Length > 4)
                        newUrl = $"http://164.160.104.66:7001/{splitedUrl[3]}/{splitedUrl[4]}";
                    else
                        newUrl = $"http://164.160.104.66:7001/{splitedUrl[3]}";
                }

                var http = (HttpWebRequest)WebRequest.Create(new Uri(newUrl));
                http.ContentType = "application/json";
                http.Method = "GET";
                http.Timeout = PSC.TimeOut;

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

        public ProviderResponseDTO Connect(SwitchEndPointDTO PSC, string baseAddress)
        {
            ProviderResponseDTO PR = new ProviderResponseDTO();
            try
            {
                //var splitedUrl = PSC.URL.Split('/');
                //var newUrl = $"http://164.160.104.66:7001/{splitedUrl[3]}/{splitedUrl[4]}/{splitedUrl[5]}/";
                //PSC.URL = newUrl;
                string newUrl = baseAddress;

                if (_isEnvirnomentDevelopment)
                {
                    var splitedUrl = baseAddress.Split('/');

                    if (splitedUrl.Length > 5)
                        newUrl = $"http://164.160.104.66:7001/{splitedUrl[3]}/{splitedUrl[4]}?{splitedUrl[5]}";
                    else
                        if (splitedUrl.Length > 4)
                        newUrl = $"http://164.160.104.66:7001/{splitedUrl[3]}/{splitedUrl[4]}";
                    else
                        newUrl = $"http://164.160.104.66:7001/{splitedUrl[3]}";
                }

                var http = (HttpWebRequest)WebRequest.Create(new Uri(newUrl));
                http.ContentType = "application/json";
                http.Method = "GET";
                http.Timeout = PSC.TimeOut;

                HttpStatusCode statusCode;
                var httpResponse = (HttpWebResponse)http.GetResponse();
                statusCode = httpResponse.StatusCode;

                if (statusCode == HttpStatusCode.OK)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        PR.Code = 200;
                        PR.StatusCode = 200;
                        PR.Message = streamReader.ReadToEnd();
                    };
                }
                else
                {
                    PR.Code = int.Parse(statusCode.ToString());
                    PR.Message = "Unkown Error";
                }

                return PR;
            }
            catch (WebException e)
            {
                string Message = "";
                //PR.Message=((HttpWebResponse)e.Response).StatusDescription;
                using (WebResponse response = e.Response)
                {
                    if (response != null)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)response;
                        using (var streamReader = new StreamReader(response.GetResponseStream()))
                            Message = streamReader.ReadToEnd();

                        int x = (int)httpResponse.StatusCode;

                        if (Validates.CheckJSON(Message))
                        {

                            JObject o = JObject.Parse(Message);

                            if ((o["responseCode"] != null))
                                PR.Code = int.Parse(o["responseCode"].ToString());
                            else
                                PR.Code = -300;

                            if (!String.IsNullOrEmpty(httpResponse.StatusCode.ToString()))
                                PR.StatusCode = (int)httpResponse.StatusCode;
                            else
                                PR.StatusCode = 0;

                            PR.Message = Message;
                        }
                        else
                        {
                            PR.Code = -300;
                            PR.StatusCode = 0;
                            PR.Message = Message;
                        }
                    }
                    else
                    {

                        if (e.Status == WebExceptionStatus.Timeout)
                        {
                            PR.Code = -200;
                            PR.StatusCode = (int)WebExceptionStatus.Timeout;
                        }
                        else if (e.Status == WebExceptionStatus.ConnectFailure)
                        {
                            PR.Code = -300;
                            PR.StatusCode = (int)WebExceptionStatus.ConnectFailure;
                        }
                        else if (e.Status == WebExceptionStatus.ReceiveFailure)
                        {
                            PR.Code = -300;
                            PR.StatusCode = (int)WebExceptionStatus.ReceiveFailure;
                        }
                        else
                        {

                            PR.Code = -200;
                            PR.StatusCode = (int)(WebExceptionStatus)e.Status;
                        }
                        PR.Message = e.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();

                PR.Code = -200;
                PR.Message = "Line : " + line + "-" + ex.StackTrace + "-" + ex.Message;
            }

            return PR;
        }

        public ProviderResponseDTO Connect<T>(T obj, SwitchEndPointDTO PSC, string baseAddress, string tokenType)
        {
            ProviderResponseDTO PR = new ProviderResponseDTO();
            try
            {

                //PSC.URL = "http://164.160.104.66:7001/btech/rest/service/";
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
                    string json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }); //new JavaScriptSerializer().Serialize(obj);

                    streamWriter.Write(json);
                }

                HttpStatusCode statusCode;
                var httpResponse = (HttpWebResponse)http.GetResponse();
                statusCode = httpResponse.StatusCode;

                if (statusCode == HttpStatusCode.OK)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        PR.Code = 200;
                        PR.StatusCode = 200;
                        PR.Message = streamReader.ReadToEnd();

                    };
                }
                else
                {
                    PR.Code = int.Parse(statusCode.ToString());
                    PR.Message = "Unkown Error";
                }
            }
            catch (WebException e)
            {
                string Message = "";
                //PR.Message=((HttpWebResponse)e.Response).StatusDescription;
                using (WebResponse response = e.Response)
                {
                    if (response != null)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)response;
                        using (var streamReader = new StreamReader(response.GetResponseStream()))
                            Message = streamReader.ReadToEnd();

                        int x = (int)httpResponse.StatusCode;

                        if (Validates.CheckJSON(Message))
                        {

                            JObject o = JObject.Parse(Message);

                            if ((o["responseCode"] != null))
                                PR.Code = int.Parse(o["responseCode"].ToString());
                            else
                                PR.Code = -300;

                            if (!String.IsNullOrEmpty(httpResponse.StatusCode.ToString()))
                                PR.StatusCode = (int)httpResponse.StatusCode;
                            else
                                PR.StatusCode = 0;

                            PR.Message = Message;
                        }
                        else
                        {
                            PR.Code = -300;
                            PR.StatusCode = 0;
                            PR.Message = Message;
                        }
                    }
                    else
                    {

                        if (e.Status == WebExceptionStatus.Timeout)
                        {
                            PR.Code = -200;
                            PR.StatusCode = (int)WebExceptionStatus.Timeout;
                        }
                        else if (e.Status == WebExceptionStatus.ConnectFailure)
                        {
                            PR.Code = -300;
                            PR.StatusCode = (int)WebExceptionStatus.ConnectFailure;
                        }
                        else if (e.Status == WebExceptionStatus.ReceiveFailure)
                        {
                            PR.Code = -300;
                            PR.StatusCode = (int)WebExceptionStatus.ReceiveFailure;
                        }
                        else
                        {

                            PR.Code = -200;
                            PR.StatusCode = (int)(WebExceptionStatus)e.Status;
                        }
                        PR.Message = e.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();

                PR.Code = -200;
                PR.Message = "Line : " + line + "-" + ex.StackTrace + "-" + ex.Message;
            }

            return PR;
        }

        public string GetToken(SwitchEndPointDTO PSC)
        {
            string token = "";
            //Connect Switch
            try
            {
                SwitchToken ST = new SwitchToken();
                ST.Password = PSC.UserPassword;
                ST.Username = PSC.UserName;

                //PSC[0] = "http://164.160.104.136:8087/momknswitch/api/v1.0/";
                string Response = Connect(ST, PSC, "login", "Basic ", UrlType.Custom);
                if (Validates.CheckJSON(Response))
                {
                    JObject o = JObject.Parse(Response);
                    if (o["responseCode"].ToString() == "200")
                    {
                        string data = o["data"].ToString();
                        JObject oToken = JObject.Parse(data);
                        token = oToken["token"].ToString();
                    }
                }
            }
            catch (Exception exp)
            {
                token = exp.Message;
            }
            return token;
        }
    }
}
