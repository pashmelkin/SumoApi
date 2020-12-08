using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Deployment.Models;
using DeploymentAPI.Models;
using DeploymentAPI.Utils;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Deployment.Service
{
    public class DeploymentService : IDeploymentService
    {
        private readonly ISumoQueryService _sumoService;
        private IMemoryCache _cache;
        private readonly ProductDetails _prodDetails;

        public DeploymentService(ISumoQueryService sumoService, IMemoryCache memoryCache, IConfiguration configuration)
        {
            this._sumoService = sumoService;
            this._cache = memoryCache;
            this._prodDetails = new ProductDetails();
            configuration.GetSection(ProductDetails.ProductDetailsSection).Bind(this._prodDetails);
        }

        public async Task<List<DeploymentDetails>> CacheTryGetValueSet()
        {
            List<DeploymentDetails> cacheEntry;
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
            if (string.IsNullOrEmpty(commitSha))
                return null;

            List<DeploymentDetails> deployments = await CacheTryGetValueSet();
            deployments.Sort((x, y) => Nullable.Compare(x.date, y.date));
            var prodDeps = deployments.FindAll(d => d.environment.LowCase().Equals(_prodDetails.Environment.LowCase()));
            var commitDeployment = prodDeps.Find(d => d.commitSha.LowCase().Equals(commitSha));

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
                return NotFoundDeployment();
            var startDate = Convert.ToDateTime(devDeps.date);

            var firstProdDeployment = prodDeps.Find(d => Nullable.Compare(d.date, startDate) > 0);

            if (firstProdDeployment == null)
                firstProdDeployment = NotFoundDeployment();
            return firstProdDeployment;

        }

        private  DeploymentDetails NotFoundDeployment()
        {
            return new DeploymentDetails
            {
                commitSha = "not deployed yet",
                environment = "prod",
                date = null
            };
        }

        private async Task<List<DeploymentDetails>> GetAllDeployments()
        {
            var searchJobId = await _sumoService.SearchForDeployments(_prodDetails.ProductName, _prodDetails.Environment);
            await _sumoService.WaitJobIsReady(searchJobId);
            var deployments = await _sumoService.GetAllDeployments(searchJobId);
            return deployments;
        }
    }
}
