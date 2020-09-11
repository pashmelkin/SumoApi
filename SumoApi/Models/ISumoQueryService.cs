using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployment.Models
{
    public interface ISumoQueryService
    {
        Task<string> SearchForDeployments();
        Task<List<(string commitId, string env, string date)>> GetAllDeployments(string searchJobId);
        Task<bool> WaitJobIsReady(string searchJobId);
    }
}
