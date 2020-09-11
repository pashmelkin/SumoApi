using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployment.Models
{
    public interface IDeploymentService
    {
        Task<List<(string, DateTime)>>  GetDeployment(string commitId);
    }
}
