using Microsoft.AspNetCore.Http;

namespace TT.Services.Models
{
    public class InboundUploadFileDto
    {
        public IFormFile File { get; set; }
        public string CustomerCode { get; set; }
        public string SupplierID { get; set; }
    }
}
