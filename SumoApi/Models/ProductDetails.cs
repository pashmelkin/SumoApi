using System;
namespace DeploymentAPI.Models
{
    public class ProductDetails
    {
        public const string ProductDetailsSection = "ProductDetails";
        public string Environment { get; set; }
        public string ProductName { get; set; }
    }
}
