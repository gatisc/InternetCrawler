using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace InternetCrawler.RealEstate
{
    public class Flat : Entity
    {
        public string Id { get; set; }
        public string Link { get; set; }
        public string Price { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Street { get; set; }
        public decimal Rooms { get; set; }
        public decimal Area { get; set; }
        public string Floor { get; set; }
        public string Series { get; set; }
        public string Type { get; set; }
        public string Comfort { get; set; }
        public string Text { get; set; }
        public override void Write(HtmlDocument htmlDoc, IEnumerable<String> ads)
        {
            base.Write(htmlDoc, ads);
            string text = htmlDoc.GetElementbyId("msg_div_msg").InnerHtml;
            text = text.Substring(0, text.IndexOf("<table"));
            text = Regex.Replace(text, "<.*?>", String.Empty);
            text = text.Replace("\r", "");
            text = text.Replace("\n", "");
            string date = string.Empty;
            string href = "#";
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
                href = oneRelNode.Attributes["href"].Value;
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
            HtmlNode ComfortNode = htmlDoc.GetElementbyId("tdo_1734");
            string comfort = "#";
            if (ComfortNode != null)
            {
                comfort = ComfortNode.InnerText;
            }
            HtmlNode PriceNode = htmlDoc.GetElementbyId("tdo_8");
            string price = "#";
            if (PriceNode != null)
            {
                price = PriceNode.InnerText;
            }
            HtmlNode TypeNode = htmlDoc.GetElementbyId("tdo_2");
            string type = "#";
            if (TypeNode != null)
            {
                type = TypeNode.InnerText;
            }
            HtmlNode SeriesNode = htmlDoc.GetElementbyId("tdo_6");
            string series = "#";
            if (SeriesNode != null)
            {
                series = SeriesNode.InnerText;
            }
            HtmlNode FloorNode = htmlDoc.GetElementbyId("tdo_4");
            string floor = "#";
            if (FloorNode != null)
            {
                floor = FloorNode.InnerText;
            }
            HtmlNode AreaNode = htmlDoc.GetElementbyId("tdo_3");
            string area = "#";
            if (AreaNode != null)
            {
                area = AreaNode.InnerText;
            }
            HtmlNode RoomsNode = htmlDoc.GetElementbyId("tdo_1");
            string rooms = "#";
            if (RoomsNode != null)
            {
                rooms = RoomsNode.InnerText;
            }
            HtmlNode RegionNode = htmlDoc.GetElementbyId("tdo_856");
            string region = "#";
            if (RegionNode != null)
            {
                region = RegionNode.InnerText;
            }
            HtmlNode CityNode = htmlDoc.GetElementbyId("tdo_20");
            string city = "#";
            if (CityNode != null)
            {
                city = CityNode.InnerText;
            }
            HtmlNode StreetNode = htmlDoc.GetElementbyId("tdo_11");
            string street = "#";
            if (StreetNode != null)
            {
                street = StreetNode.InnerText.Replace(" [Карта]", string.Empty);
                street = street.Replace(" [Karte]", string.Empty);
            }
            if (ads == null || !ads.Any(x => x.Contains(uniqueKey)))
            {
                File.AppendAllText("data/Flats.txt", uniqueKey + "\t" + href + "\t" + price + "\t" + city + "\t" + region + "\t" +
                    street + "\t" + rooms + "\t" + area + "\t" + floor + "\t" + series + "\t" + type + "\t" + comfort + "\t" +
                    text + Environment.NewLine);
            }
        }
        public override IEnumerable<String> GetExisting()
        {
            base.GetExisting();
            string path = "data/Flats.txt";
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
