﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MSU.HR.Models.Others;
using MSU.HR.Services.Interfaces;

namespace MSU.HR.Services.Repositories
{
    public class ErrorHandler : ControllerBase
    {
        public BadRequestObjectResult BadRequestAsync(string message)
        {
            var error = new ErrorModel() { Message = message, IsSuccess = false, ErrorCode = 400 };
            return BadRequest(error: error);
        }

        public Task<BadRequestObjectResult> BadRequestAsync(string message, object data)
        {
            throw new NotImplementedException();
        }

        public Task<BadRequestObjectResult> BadRequestAsync(string message, int errorCode)
        {
            throw new NotImplementedException();
        }

        public Task<BadRequestObjectResult> BadRequestAsync(string message, object data, int errorCode)
        {
            throw new NotImplementedException();
        }
    }
}
