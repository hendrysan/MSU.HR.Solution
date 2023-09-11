using Microsoft.AspNetCore.Http;
using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Requests;
using MSU.HR.Models.Responses;

namespace MSU.HR.Services.Interfaces
{
    public interface IAttendance
    {
        Task<DocumentAttendance> GetDocumentAttendance(Guid id);
        Task<int> UploadAsync(IFormFile file, DateTime DocumentDate);
        Task<DataTableResponse> GetDataTableDocumentResponseAsync(DataTableRequest request);
        Task<DataTableResponse> GetDataTableDocumentDetailResponseAsync(DataTableRequest request, Guid id);

        Task<int> ActionDocumentUploadAsync(Guid id, string action);


    }
}
