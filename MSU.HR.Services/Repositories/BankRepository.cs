﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSU.HR.Commons.Extensions;
using MSU.HR.Contexts;
using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Paginations;
using MSU.HR.Services.Interfaces;
using System.Security.Claims;

namespace MSU.HR.Services.Repositories
{
    public class BankRepository : IBank
    {
        private readonly DatabaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserIdentityModel userIdentity;
        private readonly ILogError _logError;
        private readonly ErrorHandler errorHandler;
        //private readonly IResponse _response;

        public BankRepository(DatabaseContext context, IHttpContextAccessor httpContextAccessor, ILogError logError)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            userIdentity = new UserIdentityModel(_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity);
            _logError = logError;
            //_response = response;
            errorHandler = new ErrorHandler();
        }

        public async Task<bool> CheckCodeExistsAsync(string code)
        {
            try
            {
                var any = await _context.Banks.Where(i => i.IsActive == true && i.Code == code).AnyAsync();

                return any;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, new { code = code });

                throw new Exception("Bank CheckCodeExistsAsync Error : " + ex.Message);
            }
        }

        public async Task<int> CreateAsync(Bank entity)
        {
            try
            {
                entity.CreatedBy = userIdentity.Id.ToString();
                entity.CreatedDate = DateTime.Now;
                entity.IsActive = true;
                entity.LastUpdatedBy = userIdentity.Id.ToString();
                entity.LastUpdatedDate = DateTime.Now;
                _context.Banks.Add(entity);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, entity);
                throw new Exception("Bank Create Error : " + ex.Message);
            }
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _context.Banks.Where(i => i.IsActive == true && i.Id == id).FirstOrDefaultAsync();
                if (entity == null)
                    throw new Exception("badrequest Data Not found");


                entity.IsActive = false;
                entity.LastUpdatedBy = userIdentity.Id.ToString();
                entity.LastUpdatedDate = DateTime.Now;

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, new { id = id });
                throw new Exception("Bank Delete Error : " + ex.Message);
            }
        }

        public async Task<Bank> GetBankAsync(Guid id)
        {
            try
            {
                var entity = await _context.Banks.Where(i => i.IsActive == true && i.Id == id).FirstOrDefaultAsync();
                return entity;

            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, new { id = id });
                throw new Exception("Bank Find Error : " + ex.Message);
            }
        }

        public async Task<BankPagination> GetBanksAsync(string search, PaginationModel pagination)
        {
            try
            {
                BankPagination result = new BankPagination();
                result.Pagination = pagination;
                result.Pagination.TotalRecord = await _context.Banks.Where(i => i.IsActive == true && i.Code.Contains(search) || i.Name.Contains(search)).CountAsync();

                var list = await _context.Banks.Where(i => i.IsActive == true && i.Code.Contains(search) || i.Name.Contains(search)).Page(pagination.PageNumber, pagination.PageSize).ToListAsync();

                result.Pagination.TotalPage = (int)Math.Ceiling((double)result.Pagination.TotalRecord / pagination.PageSize);
                result.Banks = list;

                return result;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, new { search = search, pagination = pagination });
                throw new Exception("Bank Pagination Error : " + ex.Message);
            }
        }

        public async Task<IEnumerable<Bank>> GetBanksAsync()
        {
            try
            {
                var list = await _context.Banks.Where(i => i.IsActive == true).ToListAsync();

                return list;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, string.Empty);
                throw new Exception("Bank Get All Error : " + ex.Message);
            }
        }

        public async Task<IEnumerable<DropdownModel>> GetDropdownModelAsync()
        {
            try
            {
                var list = await _context.Banks.Where(i => i.IsActive == true).ToListAsync();

                var result = list.Select(i => new DropdownModel()
                {
                    Code = i.Id.ToString(),
                    Name = i.Name
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, string.Empty);
                throw new Exception("Bank Dropdown Error : " + ex.Message);
            }
        }

        public async Task<int> UpdateAsync(Guid id, Bank entity)
        {
            try
            {
                var find = await _context.Banks.Where(i => i.IsActive == true && i.Id == id).FirstOrDefaultAsync();
                if (find == null)
                    throw new Exception("badrequest Data Not found");

                find.LastUpdatedBy = userIdentity.Id.ToString();
                find.LastUpdatedDate = DateTime.Now;
                find.Name = entity.Name;
                find.Code = entity.Code;

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, new { id = id, entity = entity });
                throw new Exception("Bank Update Error : " + ex.Message);
            }
        }
    }
}
