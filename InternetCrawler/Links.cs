using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InternetCrawler
{
    public class Links
    {
        public List<string> LinkList = new List<string>();
        public void GetLinks(string Allow, int depth = 1, string lang = "lv", bool isList = false, int page = 1)
        {
            do
            {
                List<string> TempList = new List<string>(LinkList);
                LinkList.Clear();
                if (TempList.Count() == 0)
                {
                    TempList.Add("/");
                }
                foreach (string one in TempList)
                {
                    Net net = new Net();
                    string Link = Properties.Settings.Default.RootLink + one;
                    Link = Link.Replace("/lv/", "/" + lang + "/");
                    if (isList)
                    {
                        Link += "all/page" + page.ToString() + ".html";
                    }
                    System.Threading.Thread.Sleep(Properties.Settings.Default.DelaySec * 1000);
                    string rootPage = net.getHttpPage(Link);
                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(rootPage);
                    if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0)
                    {
                        throw new Exception(String.Format("{0} errors found", htmlDoc.ParseErrors.Count()));
                    }
                    else
                    {
                        if (htmlDoc.DocumentNode != null)
                        {
                            IEnumerable<String> lines = File.ReadLines("ignoreTemp.txt");
                            HtmlNodeCollection tableDataNodes = htmlDoc.DocumentNode.SelectNodes("//td");
                            if (tableDataNodes != null && tableDataNodes.Count() > 0)
                            {
                                Dictionary<string, HtmlNode> Dict = new Dictionary<string, HtmlNode>();
                                foreach (HtmlNode tableDataNode in tableDataNodes)
                                {
                                    if (tableDataNode.HasChildNodes)
                                    {
                                        GetHrefAttributes(Allow, tableDataNode.ChildNodes, lines, Dict);
                                    }
                                }
                            }
                        }
                    }
                }
                depth--;
            } while (depth > 0);
        }
        void GetHrefAttributes(string Allow, HtmlNodeCollection tableDataNodes,
                    IEnumerable<String> lines, Dictionary<string, HtmlNode> Dict)
        {
            foreach (HtmlNode tableDataNode in tableDataNodes)
            {
                if (tableDataNode != null && tableDataNode.Name == "a")
                {
                    if (tableDataNode.Attributes["href"].Value != Allow && lines.Contains(tableDataNode.Attributes["href"].Value))
                    {
                        continue;
                    }
                    else
                    {
                        if (!Dict.ContainsKey(tableDataNode.Attributes["href"].Value))
                        {
                            Dict.Add(tableDataNode.Attributes["href"].Value, tableDataNode);
                            LinkList.Add(tableDataNode.Attributes["href"].Value);
                        }
                    }
                }
                if (tableDataNode.HasChildNodes)
                {
                    GetHrefAttributes(Allow, tableDataNode.ChildNodes, lines, Dict);
                }
            }
        }
    }
}
