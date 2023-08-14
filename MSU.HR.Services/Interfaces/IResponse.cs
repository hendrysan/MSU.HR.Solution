using MSU.HR.Models.Others;

namespace MSU.HR.Services.Interfaces
{
    public interface IResponse
    {
        Task<ErrorModel> BadRequest(string message, object data);
        Task<ErrorModel> Error(string message, object data);
    }
}
