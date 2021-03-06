﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.IO;
using System.Collections;

using Newtonsoft.Json;

namespace Astrum.Http
{
    public class HttpClient
    {

        public static string UA = "Mozilla/5.0 (Linux; Android 4.0.4; Galaxy Nexus Build/IMM76B) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.133 Mobile Safari/535.19";

        private CookieContainer cc = null;

        public HttpClient()
        {
            clearCookie();
        }

        public void clearCookie()
        {
            cc = new CookieContainer();
        }

        public string Get(string url)
        {
            var request = CreateRequest(url);

            HttpWebResponse response = null;
            string result = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                result = ResponseToString(response);
                //Console.WriteLine(result);
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
            return result;
        }

        public string Post(string url, Dictionary<string, string> values)
        {
            var request = CreateRequest(url);
            request = PostForm(request, values);

            HttpWebResponse response = null;
            string result = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                result = ResponseToString(response);
                //Console.WriteLine(result);
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
            return result;
        }

        public HttpWebRequest CreateRequest(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = cc;
            request.UserAgent = UA;

            //request.Proxy = null;

            return request;
        }

        public HttpWebRequest PostForm(HttpWebRequest request, Dictionary<string, string> values)
        {
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            string param = "";
            foreach (string key in values.Keys)
            {
                param += String.Format("{0}={1}&", key, values[key]);
            }
            byte[] data = Encoding.UTF8.GetBytes(param);
            request.ContentLength = data.Length;

            Stream reqStream = request.GetRequestStream();
            reqStream.Write(data, 0, data.Length);
            reqStream.Close();

            return request;
        }

        public HttpWebRequest PostJson(HttpWebRequest request, Dictionary<string, object> values)
        {
            request.Method = "POST";
            request.ContentType = "application/json";

            string param = JsonConvert.SerializeObject(values);
            
            byte[] data = Encoding.UTF8.GetBytes(param);
            request.ContentLength = data.Length;

            Stream reqStream = request.GetRequestStream();
            reqStream.Write(data, 0, data.Length);
            reqStream.Close();

            return request;
        }

        public string ResponseToString(HttpWebResponse response)
        {
            var reader = new StreamReader(response.GetResponseStream());
            var responseString = reader.ReadToEnd();

            reader.Close();

            //Console.WriteLine(responseString);
            return responseString;
        }

        public bool DownloadBinary(string url, string fileName)
        {
            var request = CreateRequest(url);

            HttpWebResponse response = null;
            Stream sw = null;
            FileStream fs = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                long total = response.ContentLength;
                long loaded = 0;

                sw = response.GetResponseStream();

                //ファイルに書き込むためのFileStreamを作成
                fs = new FileStream(fileName, FileMode.Create);

                //応答データをファイルに書き込む
                byte[] readData = new byte[1024];
                for (;;)
                {
                    //データを読み込む
                    int readSize = sw.Read(readData, 0, readData.Length);
                    if (readSize == 0)
                    {
                        //すべてのデータを読み込んだ時
                        break;
                    }
                    //読み込んだデータをファイルに書き込む
                    fs.Write(readData, 0, readSize);

                    loaded += readSize;

                    Console.WriteLine("{0} / {1} loaded.", loaded, total);
                }
            }
            catch
            {

                return false;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
            }

            return true;
        } 
    }
}
