using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp;
using MSU.HR.Commons.Enums;
using MSU.HR.Contexts;
using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Services.Interfaces;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

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

        public async Task<int> UploadAsync(IFormFile file)
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
                    CreatedDate = DateTime.Now
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
    }
}
