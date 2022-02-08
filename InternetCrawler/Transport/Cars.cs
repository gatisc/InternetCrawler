using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace InternetCrawler.Transport
{
    public class Cars : Entity
    {
        public override void Write(HtmlDocument htmlDoc, IEnumerable<String> ads)
        {
            base.Write(htmlDoc, ads);
            string text = htmlDoc.GetElementbyId("msg_div_msg").InnerHtml;
            text = text.Substring(0, text.IndexOf("<table"));
            text = Regex.Replace(text, "<.*?>", String.Empty);
            text = text.Replace("\r", "");
            text = text.Replace("\n", "");
            string date = string.Empty;
            HtmlNodeCollection dateNodes = htmlDoc.DocumentNode.SelectNodes("//td[contains(@class, 'msg_footer')]");
            foreach (HtmlNode oneDateNode in dateNodes)
            {
                if (oneDateNode.InnerHtml.StartsWith("Дата:") || oneDateNode.InnerHtml.StartsWith("Datums:"))
                {
                    date = oneDateNode.InnerText.Substring(oneDateNode.InnerText.IndexOf(" ") + 1);
                    date = date.Replace(".", "");
                    date = date.Replace(" ", "");
                    date = date.Replace(":", "");
                }
            }
            string linkUniq = string.Empty;
            HtmlNodeCollection relNodes = htmlDoc.DocumentNode.SelectNodes("//link[@rel='alternate']");
            foreach (HtmlNode oneRelNode in relNodes)
            {
                string href = oneRelNode.Attributes["href"].Value;
                if (href.IndexOf(@"//m.ss.lv/") > -1)
                {
                    string foundChar = string.Empty;
                    int count = 1;
                    do
                    {
                        foundChar = href.Substring(href.IndexOf(".html") - count);
                        count++;
                    } while (foundChar.IndexOf("/") < 0);
                    linkUniq = foundChar.Substring(foundChar.IndexOf("/") + 1).Replace(".html", "").ToUpper();
                }
            }
            string uniqueKey = date + "_" + linkUniq;
            if (ads == null || !ads.Any(x => x.Contains(uniqueKey)))
            {
                File.AppendAllText("data/Cars.txt", uniqueKey + "\t" + text + Environment.NewLine);
            }
        }
        public override IEnumerable<String> GetExisting()
        {
            base.GetExisting();
            string path = "data/Cars.txt";
            if (File.Exists(path))
            {
                return File.ReadLines(path);
            }
            else
            {
                return null;
            }
        }
    }
}
