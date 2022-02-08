using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InternetCrawler
{
    public class Entity
    {
        public Links Links = new Links();
        Dictionary<string, int> MaxPageDict = new Dictionary<string, int>();
        public virtual void Write(HtmlDocument htmlDoc, IEnumerable<String> ads)
        {

        }
        public virtual IEnumerable<String> GetExisting()
        {
            return new List<string>();
        }
        public void Parse(string Allow)
        {
            Directory.CreateDirectory("data");           
            int MaxPage = 150;
            for (int i = 1; i <= MaxPage; i++)
            {
                Console.Write(String.Format("\r{0}", i));
                List<int> maxPages = new List<int>();
                if (MaxPageDict.Count() > 0)
                {                   
                    foreach (KeyValuePair<string, int> oneDict in MaxPageDict)
                    {
                        maxPages.Add(oneDict.Value);
                    }
                }
                if (maxPages.Count() > 0 && maxPages.Max() < i)
                {
                    return;
                }
                Links.LinkList.Clear();
                Links.GetLinks(Allow, 2);
                Links.GetLinks(Allow, 1, "ru", true, i);
                List<string> addLinks = Links.LinkList.Where(x => x.Contains("/msg/")).ToList();
                List<string> pageLinks = Links.LinkList.Where(x => x.Contains("/page")).ToList();
                if (MaxPageDict.Count < 1)
                {
                    foreach (string onePageLink in pageLinks)
                    {
                        string PageEnd = onePageLink.Substring(onePageLink.IndexOf("/page") - 3);
                        string PageBase = onePageLink.Replace(PageEnd, "");
                        if (!MaxPageDict.ContainsKey(PageBase))
                        {
                            List<string> CurrentPageLinks = pageLinks.Where(x => x.Contains(PageBase)).ToList();
                            List<string> CurrentPageLinkEndings = new List<string>();
                            List<int> PageNumbers = new List<int>();
                            foreach (string oneCurrentPageLink in CurrentPageLinks)
                            {
                                CurrentPageLinkEndings.Add(oneCurrentPageLink.Substring(oneCurrentPageLink.IndexOf("/page") + 5));
                            }
                            foreach (string oneCurrentPageLinkEnding in CurrentPageLinkEndings)
                            {
                                PageNumbers.Add(Convert.ToInt32(oneCurrentPageLinkEnding.Replace(".html", "")));
                            }
                            MaxPageDict.Add(PageBase, PageNumbers.Max());
                        }
                    }
                }
                IEnumerable<String> ads = GetExisting();
                foreach (string oneAddLink in addLinks)
                {
                    bool contains = false;
                    foreach(KeyValuePair<string, int> oneMaxPage in MaxPageDict)
                    {
                        if (oneAddLink.IndexOf(oneMaxPage.Key) > -1)
                        {
                            contains = true;
                            break;
                        }
                    }
                    if (!contains && i > 1)
                    {
                        continue;
                    }
                    string currentAddLink = Properties.Settings.Default.RootLink + oneAddLink;
                    Net net = new Net();
                    System.Threading.Thread.Sleep(Properties.Settings.Default.DelaySec * 1000);
                    string Page = net.getHttpPage(currentAddLink);
                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(Page);
                    if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0)
                    {
                        throw new Exception(String.Format("{0} errors found", htmlDoc.ParseErrors.Count()));
                    }
                    else
                    {
                        if (htmlDoc.DocumentNode != null)
                        {
                            Write(htmlDoc, ads);
                        }
                    }
                }
            }
        }
    }
}
