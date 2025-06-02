
using Microsoft.AspNetCore.Http;

namespace PizzaShop.service.Interfaces
{
    public interface IUploadService
    {
        public string GetUniqueFileName(string fileName);

        public string? Upload(IFormFile? Image, string folder_name);

    }
}