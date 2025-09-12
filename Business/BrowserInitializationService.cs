using CemSys2.Interface;
using PuppeteerSharp;

namespace CemSys2.Business
{
    public class BrowserInitializationService : IHostedService
    {
        private readonly BrowserHolder _holder;

        public BrowserInitializationService(BrowserHolder holder)
        {
            _holder = holder;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            _holder.Browser = (Browser)await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" } // Quité la coma extra
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
