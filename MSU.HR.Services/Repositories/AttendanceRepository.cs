using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MSU.HR.Commons.Enums;
using MSU.HR.Contexts;
using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Paginations;
using MSU.HR.Models.Requests;
using MSU.HR.Models.Responses;
using MSU.HR.Services.Interfaces;
using System.Security.Claims;
using System.Text.Json;

namespace MSU.HR.Services.Repositories
{
    public class AttendanceRepository : IAttendance
    {
        private readonly DatabaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserIdentityModel userIdentity;
        private readonly ILogError _logError;

        public AttendanceRepository(DatabaseContext context, IHttpContextAccessor httpContextAccessor, ILogError logError)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            userIdentity = new UserIdentityModel(_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity);
            _logError = logError;
        }

        private List<DocumentAttendanceDetail> GetDocumentAttendanceDetails(Stream streamFile, Guid batchId)
        {
            List<DocumentAttendanceDetail> documentAttendanceDetails = new List<DocumentAttendanceDetail>();

            using (var streamReader = new StreamReader(streamFile))
            {
                string separator = ",";
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    DocumentAttendanceDetail documentAttendanceDetail = new DocumentAttendanceDetail();

                    string[] values = line.Split(separator);
                    int index = values.Length - 1;

                    if (index > 0)
                    {
                        documentAttendanceDetail = new DocumentAttendanceDetail
                        {
                            Id = Guid.NewGuid(),
                            DocumentAttendanceId = batchId,
                            separator = separator
                        };

                        if (index >= 0)
                            documentAttendanceDetail.Column0 = values[0].Trim();

                        if (index >= 1)
                            documentAttendanceDetail.Column1 = values[1].Trim();

                        if (index >= 2)
                            documentAttendanceDetail.Column2 = values[2].Trim();

                        if (index >= 3)
                            documentAttendanceDetail.Column3 = values[3].Trim();

                        if (index >= 4)
                            documentAttendanceDetail.Column4 = values[4].Trim();

                        if (index >= 5)
                            documentAttendanceDetail.Column5 = values[5].Trim();

                        if (index >= 6)
                            documentAttendanceDetail.Column6 = values[6].Trim();

                        if (index >= 7)
                            documentAttendanceDetail.Column7 = values[7].Trim();

                        if (index >= 8)
                            documentAttendanceDetail.Column8 = values[8].Trim();

                        if (index >= 9)
                            documentAttendanceDetail.Column9 = values[9].Trim();

                        if (index >= 10)
                            documentAttendanceDetail.Column10 = values[10].Trim();
                    }
                    documentAttendanceDetails.Add(documentAttendanceDetail);
                }

            }
            return documentAttendanceDetails;
        }

        private async Task<int> BulkInsertDocumentAttendanceDetail(List<DocumentAttendanceDetail> documentAttendanceDetails)
        {
            int maxData = 1000;

            if (documentAttendanceDetails.Count > maxData)
            {
                int total = documentAttendanceDetails.Count;
                int count = 0;
                int index = 0;

                while (count < total)
                {
                    var data = documentAttendanceDetails.Skip(index).Take(maxData).ToList();
                    _context.DocumentAttendanceDetails.AddRange(data);
                    var task = await _context.SaveChangesAsync();

                    count += maxData;
                    index += maxData;
                }

                return count;
            }
            else
            {
                _context.DocumentAttendanceDetails.AddRange(documentAttendanceDetails);
                return await _context.SaveChangesAsync();
            }
        }

        public async Task<int> UploadAsync(IFormFile file, DateTime DocumentDate)
        {
            try
            {
                string path = string.Empty;//Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", file.FileName);
                Guid BatchId = Guid.NewGuid();

                DocumentAttendance documentAttendance = new DocumentAttendance
                {
                    Id = BatchId,
                    DocumentName = file.FileName,
                    Path = path,
                    Size = file.Length.ToString(),
                    Type = file.ContentType,
                    Extension = Path.GetExtension(file.FileName),
                    Status = StatusDocumentAttendanceEnum.PENDING.ToString(),
                    Remarks = "",
                    CreatedBy = userIdentity.Id,
                    CreatedDate = DateTime.Now,
                    DocumentDate = DocumentDate
                };

                _context.DocumentAttendances.Add(documentAttendance);
                var task = await _context.SaveChangesAsync();

                var details = GetDocumentAttendanceDetails(file.OpenReadStream(), BatchId);
                var taskDetails = await BulkInsertDocumentAttendanceDetail(details);

                return task;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(file));

                throw new Exception("Bank CheckCodeExistsAsync Error : " + ex.Message);
            }
        }

        public Task<DocumentAttendancePagination> GetAttendanceAsync(string search, PaginationModel pagination)
        {
            throw new NotImplementedException();
        }

        public async Task<DataTableResponse> GetDataTableDocumentAsync(DataTableRequest request)
        {
            DataTableResponse response = new DataTableResponse();
            int totalRecord = 0;
            int filterRecord = 0;

            var data = _context.Set<DocumentAttendance>().AsQueryable();
            //get total count of data in table
            totalRecord = data.Count();
            // search data when search value found
            if (!string.IsNullOrEmpty(request.searchValue))
            {
                data = data.Where(x =>
                x.DocumentName.ToLower().Contains(request.searchValue.ToLower()) ||
                x.Status.ToLower().Contains(request.searchValue.ToLower()) ||
                x.Size.ToLower().Contains(request.searchValue.ToLower()) ||
                x.Type.ToLower().Contains(request.searchValue.ToLower()));
            }

            filterRecord = data.Count();

            if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortColumnDirection))
            {
                switch (request.sortColumn)
                {
                    case nameof(DocumentAttendance.DocumentName):
                        data = request.sortColumnDirection == "desc" ? data.OrderByDescending(x => x.DocumentName) : data.OrderBy(x => x.DocumentName);
                        break;
                    case nameof(DocumentAttendance.Status):
                        data = request.sortColumnDirection == "desc" ? data.OrderByDescending(x => x.Status) : data.OrderBy(x => x.Status);
                        break;
                    case nameof(DocumentAttendance.Size):
                        data = request.sortColumnDirection == "desc" ? data.OrderByDescending(x => x.Size) : data.OrderBy(x => x.Size);
                        break;
                    case nameof(DocumentAttendance.Type):
                        data = request.sortColumnDirection == "desc" ? data.OrderByDescending(x => x.Type) : data.OrderBy(x => x.Type);
                        break;
                    default:
                        data = request.sortColumnDirection == "desc" ? data.OrderByDescending(x => x.CreatedDate) : data.OrderBy(x => x.CreatedDate);
                        break;
                }
            }

            var list = data.Skip(request.skip).Take(request.pageSize).ToList();

            response.draw = request.draw;
            response.recordsTotal = totalRecord;
            response.recordsFiltered = filterRecord;
            response.data = list;

            return response;
        }



        public async Task<DataTableResponse> GetDataTableDocumentDetailAsync(DataTableRequest request, Guid id)
        {
            DataTableResponse response = new DataTableResponse();
            int totalRecord = 0;
            int filterRecord = 0;

            var data = _context.Set<DocumentAttendanceDetail>().Where(i => i.DocumentAttendanceId == id).AsQueryable();

            totalRecord = data.Count();

            if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortColumnDirection))
            {
                switch (request.sortColumn)
                {
                    case nameof(DocumentAttendanceDetail.Column0):
                        data = request.sortColumnDirection == "desc" ? data.OrderByDescending(x => x.Column0) : data.OrderBy(x => x.Column0);
                        break;
                    case nameof(DocumentAttendanceDetail.Column1):
                        data = request.sortColumnDirection == "desc" ? data.OrderByDescending(x => x.Column1) : data.OrderBy(x => x.Column1);
                        break;
                    case nameof(DocumentAttendanceDetail.Column2):
                        data = request.sortColumnDirection == "desc" ? data.OrderByDescending(x => x.Column2) : data.OrderBy(x => x.Column2);
                        break;
                    case nameof(DocumentAttendanceDetail.Column3):
                        data = request.sortColumnDirection == "desc" ? data.OrderByDescending(x => x.Column3) : data.OrderBy(x => x.Column3);
                        break;
                    case nameof(DocumentAttendanceDetail.Column4):
                        data = request.sortColumnDirection == "desc" ? data.OrderByDescending(x => x.Column4) : data.OrderBy(x => x.Column4);
                        break;
                    case nameof(DocumentAttendanceDetail.Column5):
                        data = request.sortColumnDirection == "desc" ? data.OrderByDescending(x => x.Column5) : data.OrderBy(x => x.Column5);
                        break;
                    case nameof(DocumentAttendanceDetail.Column6):
                        data = request.sortColumnDirection == "desc" ? data.OrderByDescending(x => x.Column6) : data.OrderBy(x => x.Column6);
                        break;
                    case nameof(DocumentAttendanceDetail.Column7):
                        data = request.sortColumnDirection == "desc" ? data.OrderByDescending(x => x.Column7) : data.OrderBy(x => x.Column7);
                        break;
                    case nameof(DocumentAttendanceDetail.Column8):
                        data = request.sortColumnDirection == "desc" ? data.OrderByDescending(x => x.Column8) : data.OrderBy(x => x.Column8);
                        break;
                    case nameof(DocumentAttendanceDetail.Column9):
                        data = request.sortColumnDirection == "desc" ? data.OrderByDescending(x => x.Column9) : data.OrderBy(x => x.Column9);
                        break;
                    case nameof(DocumentAttendanceDetail.Column10):
                        data = request.sortColumnDirection == "desc" ? data.OrderByDescending(x => x.Column10) : data.OrderBy(x => x.Column10);
                        break;
                    default:
                        data = request.sortColumnDirection == "desc" ? data.OrderByDescending(x => x.Id) : data.OrderBy(x => x.Id);
                        break;
                }

            }

            if (!string.IsNullOrEmpty(request.searchValue))
            {
                data = data.Where(x =>
                x.Column0.ToLower().Contains(request.searchValue.ToLower()) ||
                x.Column1.ToLower().Contains(request.searchValue.ToLower()) ||
                x.Column2.ToLower().Contains(request.searchValue.ToLower()) ||
                x.Column3.ToLower().Contains(request.searchValue.ToLower()) ||
                x.Column4.ToLower().Contains(request.searchValue.ToLower()) ||
                x.Column5.ToLower().Contains(request.searchValue.ToLower()) ||
                x.Column6.ToLower().Contains(request.searchValue.ToLower()) ||
                x.Column7.ToLower().Contains(request.searchValue.ToLower()) ||
                x.Column8.ToLower().Contains(request.searchValue.ToLower()) ||
                x.Column9.ToLower().Contains(request.searchValue.ToLower()) ||
                x.Column10.ToLower().Contains(request.searchValue.ToLower())
                );
            }

            filterRecord = data.Count();

            //pagination
            var list = await data.Skip(request.skip).Take(request.pageSize).ToListAsync();

            response.draw = request.draw;
            response.recordsTotal = totalRecord;
            response.recordsFiltered = filterRecord;
            response.data = list;

            return response;
        }

        public async Task<DocumentAttendance> GetDocumentAttendance(Guid id)
        {
            var data = await _context.DocumentAttendances.FirstOrDefaultAsync(i => i.Id == id);
            return data;
        }

        public async Task<int> ActionDocumentUploadAsync(Guid id, string action)
        {
            var data = await _context.DocumentAttendances.FirstOrDefaultAsync(i => i.Id == id);
            data.Status = action;
            var task = await _context.SaveChangesAsync();

            return task;
        }
    }
}
