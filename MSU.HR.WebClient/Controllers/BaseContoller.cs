﻿using Microsoft.AspNetCore.Mvc;
using MSU.HR.Models.ViewModels;
using System.Text.Json;

namespace MSU.HR.WebClient.Controllers
{
    public abstract class BaseContoller : Controller
    {
        public BaseContoller()
        {

        }

        public void GetAlert()
        {
            try
            {
                var alert = HttpContext.Session.GetString("Alert");
                if (!string.IsNullOrEmpty(alert))
                {
                    ViewBag.Alert = JsonSerializer.Deserialize<AlertModel>(alert);
                    HttpContext.Session.SetString("Alert", "");
                }
            }
            catch (Exception er)
            {

            }
        }

        public void SetAlert(string message, AlertType type)
        {
            var alert = new AlertModel
            {
                Message = message,
                Type = type
            };
            HttpContext.Session.SetString("Alert", JsonSerializer.Serialize(alert));
        }

        public string? GetUserCode()
        {
            var code = User.Claims.FirstOrDefault(c => c.Type.Contains("Code")).Value;
            return code;
        }

        public string? GetUserFullName()
        {
            var fullName = User.Claims.FirstOrDefault(c => c.Type.Contains("FullName")).Value;
            return fullName;
        }

        public string? GetUserName()
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type.Contains("UserName")).Value;
            return userName;
        }

        public string? GetUserRoleId()
        {
            var roleId = User.Claims.FirstOrDefault(c => c.Type.Contains("RoleId")).Value;
            return roleId;
        }

        public string? GetUserRoleName()
        {
            var roleName = User.Claims.FirstOrDefault(c => c.Type.Contains("RoleName")).Value;
            return roleName;
        }

        public string? GetUserId()
        {
            var id = User.Claims.FirstOrDefault(c => c.Type.Contains("Id")).Value;
            return id;
        }
    }
}
