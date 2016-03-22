using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using log4net;

namespace WebImageDownloader.ConsoleApp
{
    // Downloads an html file from a specified url
    // and scrapes out all Img Src values
    public class ImageScraper
    {
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string targetUrl;
        private readonly string dirName;

        private readonly SimpleQueue queue;


        public ImageScraper(SimpleQueue queue, string targetUrl, string dirName)
        {
            this.queue = queue;
            this.targetUrl = targetUrl;
            this.dirName = dirName;
        }

        public void ScrapeAll()
        {
            _logger.Info("Starting download, url: " + targetUrl);

            string html;
            using (var client = new WebClient())
            {
                client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko");

                _logger.InfoFormat("Downloading html from: {0}", targetUrl);

                html = client.DownloadString(targetUrl);
            }

            // Save html file to disk for sh!ts and giggles
            File.AppendAllText(Path.Combine(dirName, "source.htm"), html);

            // Now parse the img src links out...
            var parser = new HtmlParser(html);
            parser.Scraped += ParserScraped;
            parser.Scrape();

            // Enqueue a null to say we are done
            _logger.InfoFormat("Scrape complete sending ENDTOKEN");
            queue.Enqueue(null);

        }

        void ParserScraped(object sender, EventArgs args)
        {
            var uri = sender as Uri;
            _logger.InfoFormat("ENQUEUEING Uri: {0}", uri.AbsoluteUri);
            queue.Enqueue(uri);
        }
    }
}