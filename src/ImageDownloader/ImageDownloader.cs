using System;
using System.IO;
using System.Linq;
using System.Net;
using log4net;

namespace WebImageDownloader.Console
{
    /// <summary>
    /// Connects to a SimpleQueue as a consumer and downloads resources at all URIs.
    /// </summary>
    public class ImageDownloader
    {
        private string[] VALID_FILES = { ".jpg", ".png" };
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly SimpleQueue queue;
        private readonly string dirName;

        public ImageDownloader(SimpleQueue queue, string dirName)
        {
            this.queue = queue;
            this.dirName = dirName;
        }

        public void Download()
        {
            try
            {
                for (;;)
                {
                    var uri = queue.Dequeue();

                    if (uri == null)
                    {
                        _logger.Info("Recieved ENDTOKEN... finishing processing.");
                        break;
                    }

                    using (var client = new WebClient())
                    {
                        if (VALID_FILES.Any(x => uri.AbsoluteUri.ToLower().EndsWith(x)))
                        {
                            var fileName = uri.AbsoluteUri.Substring(uri.AbsoluteUri.LastIndexOf('/') + 1);
                            _logger.DebugFormat("Downloading resource at url: {0}", uri.AbsoluteUri);
                            client.DownloadFile(uri, Path.Combine(dirName, fileName));
                            _logger.InfoFormat("Download of {0} complete", fileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Whoops! Something went wrong :( ", ex);
            }
        }
    }
}
