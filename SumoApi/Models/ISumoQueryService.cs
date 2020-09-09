using System;
using System.Threading.Tasks;

namespace Deployment.Models
{
    public interface ISumoQueryService
    {
        Task<string> SearchForDeployments();
        Task<DateTime> GetAllDeployments(string searchJobId);
    }
}
