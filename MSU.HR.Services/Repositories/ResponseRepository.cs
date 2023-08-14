using MSU.HR.Models.Others;
using MSU.HR.Services.Interfaces;

namespace MSU.HR.Services.Repositories
{
    public class ResponseRepository : IResponse
    {
        public Task<ErrorModel> BadRequest(string message, object data)
        {
            throw new NotImplementedException();
        }

        public Task<ErrorModel> Error(string message, object data)
        {
            throw new NotImplementedException();
        }
    }
}
