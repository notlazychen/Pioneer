using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Html;
using ScrapySharp.Html.Forms;
using ScrapySharp.Network;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Pioneer.Spider
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.WriteLine("Hello World!");

            Manga manga = new Manga
            {
                Name = "擅长捉弄的高木同学",
                Charset = "GBK",
                BaseUrl = "http://comic.ikkdm.com",
                IndexUrl = "http://comic.ikkdm.com/comiclist/2112/index.htm"
            };

            ScrapingBrowser browser = new ScrapingBrowser();
            browser.Encoding = Encoding.GetEncoding(manga.Charset);
            //set UseDefaultCookiesParser as false if a website returns invalid cookies format
            //browser.UseDefaultCookiesParser = false;
            WebPage homePage = browser.NavigateToPage(new Uri(manga.IndexUrl));

            IMangaSeeker seeker = new Kuku(manga);
            var indexes = seeker.GetIndexes(homePage.Html);

            int i = 0;
            foreach (var index in indexes)
            {
                _ = DownloadIndexAsync(index, i, manga, seeker);
                i++;
            }

            Console.ReadLine();
        }

        static async Task DownloadIndexAsync(SectionInfo section, int i, Manga manga, IMangaSeeker seeker)
        {
            bool hasNext = false;
            int p = 0;
            var pageUrl = section.FirstPageUrl.ToAbsolutedUrl(manga.BaseUrl);
            Console.WriteLine($"downloading: [{i}]");
            ScrapingBrowser browser = new ScrapingBrowser();
            browser.Encoding = Encoding.GetEncoding(manga.Charset);
            do
            {
                try
                {
                    var page = await browser.NavigateToPageAsync(new Uri(pageUrl));
                    var imgUrl = seeker.GetImgUrl(page.Html);

                    string indexDir = $"{manga.Name}/{section.Title}";
                    if (!Directory.Exists(indexDir))
                    {
                        Directory.CreateDirectory(indexDir);
                    }

                    Console.WriteLine($"                   -- {p}");
                    string picPath = $"{indexDir}/{p}.jpg";
                    if (!File.Exists(picPath))
                    {
                        WebClient client = new WebClient();
                        client.DownloadFileAsync(new Uri(imgUrl), picPath);
                    }
                    var next = seeker.GetNextUrl(page.Html);
                    if (next != null)
                    {
                        hasNext = true;
                        pageUrl = next.ToAbsolutedUrl(manga.BaseUrl);
                    }
                    else
                    {
                        hasNext = false;
                        pageUrl = null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    break;
                }
                p++;
            } while (hasNext);
        }
    }

    public class Manga
    {
        public string Name { get; set; }
        public string BaseUrl { get; set; }
        public string IndexUrl { get; set; }

        public string SelectIndex { get; set; }
        public string SelectNext { get; set; }

        public string Charset { get; set; }
    }

    public static class UrlExtensions
    {
        public static string ToAbsolutedUrl(this string url, string baseUrl)
        {
            if (url.StartsWith("http"))
            {
                return url;
            }
            return $"{baseUrl}{url}";
        }
    }
}
