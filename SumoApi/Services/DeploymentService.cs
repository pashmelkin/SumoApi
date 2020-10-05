using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Deployment.Models;
using DeploymentAPI.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Deployment.Service
{
    public class DeploymentService : IDeploymentService
    {
        private readonly ISumoQueryService _sumoService;
        private IMemoryCache _cache;

        public DeploymentService(ISumoQueryService sumoService, IMemoryCache memoryCache)
        {
            this._sumoService = sumoService;
            this._cache = memoryCache;
        }

        public async Task<List<DeploymentDetails>> CacheTryGetValueSet()
        {
            var cacheEntry = new List<DeploymentDetails>();

            // Look for cache key.
            if (!_cache.TryGetValue(CacheKeys.Entry, out cacheEntry))
            {
                // Key not in cache, so get data.
                cacheEntry = await GetAllDeployments();

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromHours(12));

                // Save data in cache.
                _cache.Set(CacheKeys.Entry, cacheEntry, cacheEntryOptions);
            }
            return cacheEntry;
        }

        public async Task<DeploymentDetails> GetDeployment(string commitSha)
        {
            List<DeploymentDetails> deployments = await CacheTryGetValueSet();

            deployments.Sort((x, y) => DateTime.Compare(x.date, y.date));
            var prodDeps = deployments.FindAll(d => d.environment.ToLower().Equals("prod"));
            var commitDeployment = prodDeps.Find(d => d.commitSha.ToLower().Equals(commitSha));

            if (commitDeployment != null)
            {
                return new DeploymentDetails
                {
                    commitSha = commitDeployment.commitSha,
                    date = Convert.ToDateTime(commitDeployment.date),
                    environment = "production"
                };
            }

            var devDeps = deployments.Find(d => d.environment.ToLower().Equals("dev") && d.commitSha.ToLower().Equals(commitSha));
            if (devDeps == null)
                return new DeploymentDetails();
            var startDate = Convert.ToDateTime(devDeps.date);

            var firstProdDeployment = prodDeps.Find(d => DateTime.Compare(d.date, startDate) > 0);

            if (firstProdDeployment == null)
                firstProdDeployment = new DeploymentDetails()
                {
                    commitSha = "not deployed yet",
                    environment = "prod",
                    date = DateTime.Today
                };
            return firstProdDeployment;

        }

        private async Task<List<DeploymentDetails>> GetAllDeployments()
        {
            var searchJobId = await _sumoService.SearchForDeployments();
            await _sumoService.WaitJobIsReady(searchJobId);
            var deployments = await _sumoService.GetAllDeployments(searchJobId);
            return deployments;
        }
    }
}
