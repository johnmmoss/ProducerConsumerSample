using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace WebImageDownloader.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintHelp();
                Console.Read();
                return;
            }

            log4net.Config.XmlConfigurator.Configure();

            string targetUrl = args[0], outputDirectory = string.Empty;

            if (args.Length > 1)
            {
                outputDirectory = args[1];
            }

            var dirName = CreateOutputDir(outputDirectory, targetUrl);

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

        private static void PrintHelp()
        {
            Console.WriteLine("Scrapes images from a specified url. ");
            Console.WriteLine("");
            Console.WriteLine("USAGE: imgscraper <url> <outputdir> ");
            Console.WriteLine("");
            Console.WriteLine("     url         -   the url to scrape the images from.");
            Console.WriteLine("     outputdir   -   (optional) the outputdirectory for results");
            Console.WriteLine("");
            Console.WriteLine(@"Example: ");
            Console.WriteLine("");
            Console.WriteLine(@"    imgscraper http://www.dailymail.co.uk C:\Users\Jemima\Desktop ");
        }

        private static string CreateOutputDir(string outputDirectory, string targetUrl)
        {
            var dirName = targetUrl.Substring(targetUrl.IndexOf("//") + 2).Replace("/", "_");

            var outputPathFull = string.IsNullOrEmpty(outputDirectory) ? dirName : string.Format("{0}\\{1}", outputDirectory, dirName);

            if (!Directory.Exists(outputPathFull))
            {
                Directory.CreateDirectory(outputPathFull);
            }
            return outputPathFull;
        }
    }
}
