using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pioneer.Spider
{
    public class Kuku: IMangaSeeker
    {
        public Manga Manga { get; private set; }

        public Kuku(Manga manga)
        {
            Manga = manga;
        }

        public IEnumerable<string> GetIndexes(HtmlNode html)
        {
         foreach(var node in html.SelectNodes("//*[@id=\"comiclistn\"]/dd"))
            {
                var ns = node.SelectNodes("a");
                var n = ns[0];
                string href = n.GetAttributeValue("href", null);
                if (href != null)
                    yield return href;
            }
        }

        public string GetImgUrl(HtmlNode html)
        {
            var n = html.SelectSingleNode("/html[1]/body[1]/table[2]/tr[1]/td[1]/script[1]");
            string[] txts = n.InnerText.Split('\'', '\"');
            var img = txts.FirstOrDefault(x=>x.StartsWith("newkuku"));
            return img.ToAbsolutedUrl("http://n9.1whour.com/");
        }

        public string GetNextUrl(HtmlNode html)
        {
            var n = html.SelectSingleNode("/html[1]/body[1]/table[2]/tr[1]/td[1]/a");
            var nextUrl = n.GetAttributeValue("href", null);
            return nextUrl;
        }
    }
}
