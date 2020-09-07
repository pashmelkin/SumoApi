using System;
namespace Deployment.Models
{
    public interface ISumoAuth
    {
        string AccessID { get; set; }
        string AccessKey { get; set; }
    }
}
