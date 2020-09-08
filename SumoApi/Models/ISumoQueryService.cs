using System.Threading.Tasks;

namespace Deployment.Models
{
    public interface ISumoQueryService
    {
        public Task<ulong> GetListDeployments();
    }
}
