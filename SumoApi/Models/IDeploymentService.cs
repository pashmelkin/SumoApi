using System;
namespace Deployment.Models
{
    public interface IDeploymentService
    {
        DateTime GetDeployment(string commitId);
    }
}
