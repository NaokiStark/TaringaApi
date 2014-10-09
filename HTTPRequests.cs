﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace ClassLibrary1
{
    class HTTPRequests
    {
        CookieContainer cookieCont = new CookieContainer();
        HttpWebRequest request;
        HttpWebResponse response;
        public string getRequest(string Url)
        {
            try
            {
                StreamReader ddResponse;
                request = (HttpWebRequest)HttpWebRequest.Create(Url);
                request.UserAgent = "PTaringa/2.2";
                request.CookieContainer = cookieCont;

                ProcessRequest ee = ReturnRequestStream;
                wee err = returnWebException;

                //response = (HttpWebResponse)request.GetResponse();
                request.BeginGetResponse(r =>
                {
                    var httpRequest = (HttpWebRequest)r.AsyncState;
                    try
                    {
                        var httpResponse = (HttpWebResponse)httpRequest.EndGetResponse(r);
                        //This Works?
                        //ee.Invoke(new StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8));
                        ee.BeginInvoke(new StreamReader(httpResponse.GetResponseStream(),Encoding.UTF8),null,request);
                    }
                    catch (WebException ex)
                    {
                        err.Invoke(ex);
                    }
              
                },request);

                ddResponse = rRequestStream;
                //ddResponse =new StreamReader(response.GetResponseStream(), Encoding.UTF8) ;
                while (ddResponse == null && wer == null)
                {
                    ddResponse = rRequestStream;
                }
                rRequestStream = null;
                if (wer == null)
                    
                    return ddResponse.ReadToEnd();

                throw wer;
            }
            catch (WebException we)
            {
                if (we.Response != null)
                {
                    HttpWebResponse errorResponse = we.Response as HttpWebResponse;
                    if (errorResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        return "{\"code\":\"404\",\"message\":\"" + errorResponse.StatusDescription + "\"}";
                    }
                }
                else
                {
                    return "{\"code\":\"0\",\"message\":\"" + we.Message + "\"}";
                }

            }

            return "false";
            
        }

        public string postRequest(string Url,string postData){

            StreamReader ddResponse;
            ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] postBytes = ascii.GetBytes(postData.ToString());

            request = (HttpWebRequest)HttpWebRequest.Create(Url);
            request.CookieContainer = cookieCont;
  
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postBytes.Length;
            //request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36";
            request.UserAgent = "PTaringa/2.2";
            request.Headers["X-Requested-With"]="XMLHttpRequest";
            request.Headers["Origin"] = "http://www.taringa.net";
            //request.Host = "http://www.taringa.net";
            request.Referer = "http://www.taringa.net/";
            request.Accept = "application/json, text/javascript, */*; q=0.01";

            Stream postStream = request.GetRequestStream();
            postStream.Write(postBytes, 0, postBytes.Length);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            ddResponse = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            postStream.Flush();
            postStream.Close();

            return ddResponse.ReadToEnd();
        }
        public string postRequest(string Url, string postData,bool isAjaxRequest)
        {

            StreamReader ddResponse;
            ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] postBytes = ascii.GetBytes(postData.ToString());

            request = (HttpWebRequest)HttpWebRequest.Create(Url);
            request.CookieContainer = cookieCont;

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postBytes.Length;
            //request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36";
            request.UserAgent = "PTaringa/2.2";
            if (isAjaxRequest)
            {
               request.Headers["X-Requested-With"] = "XMLHttpRequest";
            }
            request.Headers["Origin"] = "http://www.taringa.net";
            //request.Host = "http://www.taringa.net";
            request.Referer = "http://www.taringa.net/";
            request.Accept = "application/json, text/javascript, */*; q=0.01";

            Stream postStream = request.GetRequestStream();
            postStream.Write(postBytes, 0, postBytes.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            ddResponse = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            postStream.Flush();
            postStream.Close();

            return ddResponse.ReadToEnd();
        }

        //Delegate For ascyn request
        public delegate void wee(WebException e);
        public WebException wer { get; set; }
        public void returnWebException(WebException e) { wer = e; }
        

        public delegate void ProcessRequest(StreamReader e);
        public StreamReader rRequestStream { get; set; }
        public void ReturnRequestStream(StreamReader e) { rRequestStream = e; }
        
    }
}
