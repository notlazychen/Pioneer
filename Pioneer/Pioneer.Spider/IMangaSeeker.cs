using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pioneer.Spider
{
    public interface IMangaSeeker
    {
        /// <summary>
        /// 获取章节（章节的页面地址）列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<SectionInfo> GetIndexes(HtmlNode html);

        string GetImgUrl(HtmlNode html);

        string GetNextUrl(HtmlNode html);
    }
}
