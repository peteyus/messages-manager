namespace Services
{
    using Core.Extensions;
    using Core.Interfaces;
    using Microsoft.Extensions.Hosting;
    using System.Threading;
    using System.Threading.Tasks;

    public class TempCleanupHostedService : IHostedService
    {
        private IUnzipService unzipService;

        public TempCleanupHostedService(IUnzipService unzipService)
        {
            unzipService.ThrowIfNull(nameof(unzipService));

            this.unzipService = unzipService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await this.unzipService.CleanUpTempFiles();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await this.unzipService.CleanUpTempFiles();
        }
    }
}
