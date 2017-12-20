﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web; 
using System.Net;
using System.IO;
using System.Text;

namespace ERP.Ctrl
{
    public class ProxyTools
    {

        public static string EasyWeb(string _url = "http://www.baidu.com")
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create( _url );    //创建一个请求示例
            HttpWebResponse response  = (HttpWebResponse)request.GetResponse();　　//获取响应，即发送请求
            Stream responseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
            string html = streamReader.ReadToEnd();
            return html;
            //Console.WriteLine(html); 
            //Console.ReadKey(); 
        }
        public static string DownLoadHtml(string url, int timeout = 10, bool enableProxy = false,
            string proxyUrl="localhost",int proxyPort=80 )
        {
            try
            {
                string html = "";
                var myRequest = (HttpWebRequest)System.Net.WebRequest.Create(url);
                myRequest.Method = "GET";
                myRequest.Timeout = 1000 * timeout;
                myRequest.AllowAutoRedirect = true;
                if (enableProxy)
                {
                    //如果启用WEBPROXY代理
                    var webProxy = new WebProxy(proxyUrl,proxyPort);
                    myRequest.Proxy = webProxy;
                }
                var myResponse = (HttpWebResponse)myRequest.GetResponse();
                using (var sr = new StreamReader(myResponse.GetResponseStream(), Encoding.GetEncoding((myResponse.CharacterSet))))
                {
                    html = sr.ReadToEnd();
                    myResponse.Close();
                }
                return html;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        //test3
        static string Web3(string url = "http://localhost:2539/")
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            CookieContainer cc = new CookieContainer();
            request = (HttpWebRequest)WebRequest.Create( url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:19.0) Gecko/20100101 Firefox/19.0";

            string requestForm = "userName=1693372175&userPassword=123456";     //拼接Form表单里的信息
            byte[] postdatabyte = Encoding.UTF8.GetBytes(requestForm);
            request.ContentLength = postdatabyte.Length;
            request.AllowAutoRedirect = false;
            request.CookieContainer = cc;
            request.KeepAlive = true;

            Stream stream;
            stream = request.GetRequestStream();
            stream.Write(postdatabyte, 0, postdatabyte.Length); //设置请求主体的内容
            stream.Close();

            //接收响应
            response = (HttpWebResponse)request.GetResponse();
            Console.WriteLine();

            Stream stream1 = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream1);
            string str = sr.ReadToEnd();
            return str;
            //Console.WriteLine(str); 
            //Console.ReadKey();
        }

        //test4 virtual Upload
        #region ....
        /** 
         * 如果要在客户端向服务器上传文件，我们就必须模拟一个POST multipart/form-data类型的请求 
         * Content-Type必须是multipart/form-data。 
         * 以multipart/form-data编码的POST请求格式与application/x-www-form-urlencoded完全不同 
         * multipart/form-data需要首先在HTTP请求头设置一个分隔符,例如7d4a6d158c9： 
         * 我们模拟的提交要设定 content-type不同于非含附件的post时候的content-type 
         * 这里需要： ("Content-Type", "multipart/form-data; boundary=ABCD"); 
         * 然后，将每个字段用“--7d4a6d158c9”分隔，最后一个“--7d4a6d158c9--”表示结束。 
         * 例如，要上传一个title字段"Today"和一个文件C:\1.txt，HTTP正文如下： 
         *  
         * --7d4a6d158c9 
         * Content-Disposition: form-data; name="title" 
         * \r\n\r\n 
         * Today 
         * --7d4a6d158c9 
         * Content-Disposition: form-data; name="1.txt"; filename="C:\1.txt" 
         * Content-Type: text/plain 
         * 如果是图片Content-Type: application/octet-stream 
         * \r\n\r\n 
         * <这里是1.txt文件的内容> 
         * --7d4a6d158c9 
         * \r\n 
         * 请注意，每一行都必须以\r\n结束value前面必须有2个\r\n，包括最后一行。 
        */

        /*Server Code
          public ActionResult Test()
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpFileCollection FileCollect = request.Files;
            if (FileCollect.Count > 0)          //如果集合的数量大于0
            {
                foreach (string str in FileCollect)
                {
                    HttpPostedFile FileSave = FileCollect[str];  //用key获取单个文件对象HttpPostedFile
                    string imgName = DateTime.Now.ToString("yyyyMMddhhmmss");
                    string AbsolutePath = FileSave.FileName;
                    FileSave.SaveAs(AbsolutePath);              //将上传的东西保存
                }
            }
            return Content("键值对数目：" + FileCollect.Count);
        }
         */
        #endregion
        public static string Web4(string[] args)
        {
            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");//元素分割标记 
            StringBuilder sb = new StringBuilder();
            sb.Append("--" + boundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"file1\"; filename=\"D:\\upload.xml" + "\"");
            sb.Append("\r\n");
            sb.Append("Content-Type: application/octet-stream");
            sb.Append("\r\n");
            sb.Append("\r\n");//value前面必须有2个换行  

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:9170/upload/test");
            request.ContentType = "multipart/form-data; boundary=" + boundary;//其他地方的boundary要比这里多--  
            request.Method = "POST";

            FileStream fileStream = new FileStream(@"D:\123.xml", FileMode.OpenOrCreate, FileAccess.Read);

            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sb.ToString());
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            //http请求总长度  
            request.ContentLength = postHeaderBytes.Length + fileStream.Length + boundaryBytes.Length;
            Stream requestStream = request.GetRequestStream(); 
            requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
            byte[] buffer = new Byte[checked((uint)Math.Min(4096, (int)fileStream.Length))];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                requestStream.Write(buffer, 0, bytesRead);
            }
            fileStream.Dispose();
            requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
            WebResponse webResponse2 = request.GetResponse();
            Stream htmlStream = webResponse2.GetResponseStream();
            StreamReader sr = new StreamReader(htmlStream);
            string HTML = sr.ReadToEnd();
            htmlStream.Dispose();
            return HTML;
            //string HTML = GetHtml(htmlStream , "UTF-8");
            //Console.WriteLine(HTML);
        }
　　


        //test2
        public static string WebWithCookie(string[] args)
        {
            HttpHeader header = new HttpHeader();
            header.accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/x-silverlight, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/x-ms-application, application/x-ms-xbap, application/vnd.ms-xpsdocument, application/xaml+xml, application/x-silverlight-2-b1, */*";
            header.contentType = "application/x-www-form-urlencoded";
            header.method = "POST";
            header.userAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";
            header.maxTry = 300;

            //在这里自己拼接一下Cookie，不用复制过来的那个GetCookie方法了，原来的那个写法还是比较严谨的
            CookieContainer cc = new CookieContainer();
            Cookie cUserName = new Cookie("cSpaceUserEmail", "742783833%40qq.com");
            cUserName.Domain = ".7soyo.com";
            Cookie cUserPassword = new Cookie("cSpaceUserPassWord", "4f270b36a4d3e5ee70b65b1778e8f793");
            cUserPassword.Domain = ".7soyo.com";
            cc.Add(cUserName);
            cc.Add(cUserPassword);

            string html = HTMLHelper.GetHtml("http://user.7soyo.com/CollectUser/List", cc, header);
            return html;
            //FileStream fs = new FileStream(@"D:\123.txt", FileMode.CreateNew, FileAccess.ReadWrite);
            //fs.Write(Encoding.UTF8.GetBytes(html), 0, html.Length);
            //fs.Flush();
            //fs.Dispose(); 
            //Console.WriteLine(html); 
            //Console.ReadKey();
        }
    }

    public class HTMLHelper
    {
        /// <summary>
        /// 获取CooKie
        /// </summary>
        /// <param name="loginUrl"></param>
        /// <param name="postdata"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static CookieContainer GetCooKie(string loginUrl, string postdata, HttpHeader header)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                CookieContainer cc = new CookieContainer();
                request = (HttpWebRequest)WebRequest.Create(loginUrl);
                request.Method = header.method;
                request.ContentType = header.contentType;
                byte[] postdatabyte = Encoding.UTF8.GetBytes(postdata);     //提交的请求主体的内容
                request.ContentLength = postdatabyte.Length;    //提交的请求主体的长度
                request.AllowAutoRedirect = false;
                request.CookieContainer = cc;
                request.KeepAlive = true;

                //提交请求
                Stream stream;
                stream = request.GetRequestStream();
                stream.Write(postdatabyte, 0, postdatabyte.Length);     //带上请求主体
                stream.Close();

                //接收响应
                response = (HttpWebResponse)request.GetResponse();      //正式发起请求
                response.Cookies = request.CookieContainer.GetCookies(request.RequestUri);

                CookieCollection cook = response.Cookies;
                //Cookie字符串格式
                string strcrook = request.CookieContainer.GetCookieHeader(request.RequestUri);

                return cc;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 获取html
        /// </summary>
        /// <param name="getUrl"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static string GetHtml(string getUrl, CookieContainer cookieContainer, HttpHeader header)
        {
            System.Threading.Thread.Sleep(1000); 
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            try
            {
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(getUrl);
                httpWebRequest.CookieContainer = cookieContainer;
                httpWebRequest.ContentType = header == null ? "" : header.contentType;
                httpWebRequest.ServicePoint.ConnectionLimit = header == null ? 1024 : header.maxTry;
                httpWebRequest.Referer = getUrl;
                if (header != null)
                {
                    httpWebRequest.Accept = header == null ? "" : header.accept;
                    httpWebRequest.UserAgent = header == null ? "" : header.userAgent;
                }
                httpWebRequest.Method = "GET";
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                string html = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                httpWebRequest.Abort();
                httpWebResponse.Close();
                return html;
            }
            catch (Exception e)
            {
                if (httpWebRequest != null) httpWebRequest.Abort();
                if (httpWebResponse != null) httpWebResponse.Close();
                return string.Empty;
            }
        }
    }

    public class HttpHeader
    {
        public string contentType { get; set; }

        public string accept { get; set; }

        public string userAgent { get; set; }

        public string method { get; set; }

        public int maxTry { get; set; }
    }
}