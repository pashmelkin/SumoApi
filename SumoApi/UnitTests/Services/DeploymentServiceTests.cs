using Xunit;
using Moq;
using Deployment.Service;
using Deployment.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Deployment.UnitTests.Service
{
    public class DeploymentService_Should
    {
        private readonly DeploymentService _deploymentService;

        public DeploymentService_Should()
        {
            var mockSumoService = new Mock<ISumoQueryService>();
            var mockCache = new Mock<IMemoryCache>();
            var configurationMock = new Mock<IConfiguration>();
            _deploymentService = new DeploymentService(mockSumoService.Object, mockCache.Object, configurationMock.Object);
        }

        [Fact]
        public void ShouldReturnNull_IfNoCommitSha()
        {
            var res = _deploymentService.GetDeployment(string.Empty);
            Assert.Null(res);
        }
    }
}