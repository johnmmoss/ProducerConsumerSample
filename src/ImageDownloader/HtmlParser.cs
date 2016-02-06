using System;
using System.Text.RegularExpressions;
using System.Threading;
using log4net;

namespace WebImageDownloader.Console
{
    public class HtmlParser
    {
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public delegate void ImgUrlScraped(object sender, EventArgs args);

        private const string REGEX_IMG_TAG = @"<img[^>]*>";
        private const string REGEX_IMGSRC = "\\ssrc=\"([^\"]*)\"";
        
        private readonly string html;

        public event ImgUrlScraped Scraped;

        public HtmlParser(string html)
        {
            this.html = html;
        }

        public void Scrape()
        {
            _logger.Info("Searching html page for img src urls...");

            var regexImgTag = new Regex(REGEX_IMG_TAG);

            var results = regexImgTag.Matches(html);

            _logger.InfoFormat("Found {0} img tags!", results.Count);

            foreach (Match imgMatch in results)
            {
                try
                {
                    var imgTag = imgMatch.Groups[0].Value;
                    var srcMatch = new Regex(REGEX_IMGSRC).Match(imgTag);
                    var srcUrl = srcMatch.Groups[1].Captures[0].Value;

                    if (String.IsNullOrWhiteSpace(srcUrl))
                    {
                        _logger.Warn("srcUrl was empty for tag: " + imgMatch.Groups[0].Value);
                        continue;
                    }

                    srcUrl = srcUrl.Split('?')[0]; // clean the querystring off

                    var uri = new Uri((new Uri(srcUrl).IsFile ? "http:" : string.Empty) + srcUrl);

                    if (Scraped != null)
                    {
                        Thread.Sleep(500); // Pause for effect :)
                        Scraped(uri, new EventArgs());
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    _logger.Error("Could not scrape: " + imgMatch.Groups[0].Value);
                }
            }
        }
    }
}