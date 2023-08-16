using Microsoft.AspNetCore.Mvc;

namespace MSU.HR.Services.Interfaces
{
    public interface IErrorHandler
    {
        Task<BadRequestObjectResult> BadRequestAsync(string message);
        Task<BadRequestObjectResult> BadRequestAsync(string message, object data);
        Task<BadRequestObjectResult> BadRequestAsync(string message, int errorCode);
        Task<BadRequestObjectResult> BadRequestAsync(string message, object data, int errorCode);
        
    }
}
