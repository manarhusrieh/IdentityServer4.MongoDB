using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.MongoDB.Entities;
using IdentityServer4.MongoDB.Options;
using IdentityServer4.MongoDB.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace IdentityServer4.MongoDB
{
    internal class TokenCleanup
    {
        private readonly ILogger<TokenCleanup> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval;
        private CancellationTokenSource _source;

        public TokenCleanup(IServiceProvider serviceProvider, ILogger<TokenCleanup> logger, TokenCleanupOptions options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            if (options == null) throw new ArgumentNullException(nameof(options));
            if (options.Interval < 1) throw new ArgumentException("interval must be more than 1 second");
            _interval = TimeSpan.FromSeconds(options.Interval);
        }

        public void Start()
        {
            if (_source != null) throw new InvalidOperationException("Already started. Call Stop first.");

            _logger.LogDebug("Starting token cleanup");

            _source = new CancellationTokenSource();
            Task.Factory.StartNew(() => Start(_source.Token));
        }

        public void Stop()
        {
            if (_source == null) throw new InvalidOperationException("Not started. Call Start first.");

            _logger.LogDebug("Stopping token cleanup");

            _source.Cancel();
            _source = null;
        }

        private async Task Start(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("CancellationRequested");
                    break;
                }

                try
                {
                    await Task.Delay(_interval, cancellationToken);
                }
                catch
                {
                    _logger.LogDebug("Task.Delay exception. exiting.");
                    break;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("CancellationRequested");
                    break;
                }

                await ClearTokens();
            }
        }

        private async Task ClearTokens()
        {
            try
            {
                _logger.LogTrace("Querying for tokens to clear");

                using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var persistedGrantRepository = serviceScope.ServiceProvider.GetService<IRepository<PersistedGrant>>();

                    var expired = await persistedGrantRepository.AsQueryable().Where(x => x.Expiration < DateTimeOffset.UtcNow).ToListAsync().ConfigureAwait(false);

                    _logger.LogDebug("Clearing {tokenCount} tokens", expired.Count);

                    if (expired.Count > 0)
                    {
                        var ids = expired.Select(x => x.Id).ToArray();
                        await persistedGrantRepository.DeleteAsync(x => ids.Contains(x.Id)).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception cleaning tokens {exception}", ex.Message);
            }
        }
    }
}