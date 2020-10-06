using Xunit;
using Moq;
using Deployment.Service;
using Deployment.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Deployment.UnitTests.Service
{
    public class DeploymentService_Should
    {
        private readonly DeploymentService _deploymentService;
        private readonly ISumoQueryService _sumoService;
        private readonly IMemoryCache _cache;

        public DeploymentService_Should()
        {
            var mockSumoService = new Mock<ISumoQueryService>();
            var mockCache = new Mock<IMemoryCache>();
            _deploymentService = new DeploymentService(mockSumoService.Object, mockCache.Object);
        }

        [Fact]
        public void ShouldReturnNull_IfNoCommitSha()
        {
            var res = _deploymentService.GetDeployment(string.Empty);
            Assert.Null(res);
        }
    }
}