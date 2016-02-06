using System.Diagnostics;
using System.IO;
using System.Threading;

namespace WebImageDownloader.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            var targetUrl = "http://www.dailymail.co.uk/home/index.html";

            var dirName = CreateOutputDir(targetUrl);

            var queue = new SimpleQueue();
            var scraper = new ImageScraper(queue, targetUrl, dirName);
            var downloader = new ImageDownloader(queue, dirName);

            var scraperThread = new Thread(scraper.ScrapeAll);
            var downloaderThread = new Thread(downloader.Download);

            downloaderThread.Start();
            scraperThread.Start();

            Process.Start(dirName);

            downloaderThread.Join();
            scraperThread.Join();
        }

        public static string CreateOutputDir(string targetUrl)
        {
            var dirName = targetUrl.Substring(targetUrl.IndexOf("//") + 2).Replace("/", "_");
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
            return dirName;
        }
    }
}
