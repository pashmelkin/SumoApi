using System;
namespace SumoApi.Models
{
    public interface ISumoAuth
    {
        string AccessID { get; set; }
        string AccessKey { get; set; }
    }
}
