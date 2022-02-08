using System;
using System.IO;
using System.Text;
using System.Net;

namespace InternetCrawler
{
    public class Net
    {

        public String getHttpPage(string url)
        {
            try
            {
                WebRequest req = WebRequest.Create(url);
                req.Timeout = 2000;
                WebResponse res = req.GetResponse();
                using (StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.UTF8)) { return reader.ReadToEnd(); }
            }
            catch (Exception ex) { return ""; }
        }

    }
}
