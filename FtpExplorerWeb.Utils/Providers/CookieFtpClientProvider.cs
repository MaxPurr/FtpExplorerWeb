using FtpExplorerWeb.DataAccess.Clients;
using FtpExplorerWeb.Domain.Options;
using FtpExplorerWeb.Domain.Interfaces;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace FtpExplorerWeb.Utils.Providers
{
    public class CookieFtpClientProvider : IFtpClientProvider
    {
        private const string _ftpConnectionCookieKey = "ftp_connection";

        private readonly IHttpContextAccessor _httpContextAccessor;
        public CookieFtpClientProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public IFtpClient FtpClient
        {
            get
            {
                var context = _httpContextAccessor.HttpContext;
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }
                var cookies = context.Request.Cookies;
                string ftpConnection = cookies[_ftpConnectionCookieKey];
                if (ftpConnection == null)
                {
                    throw new ArgumentNullException(nameof(ftpConnection));
                }
                var options = JsonSerializer.Deserialize<FtpConnectionOptions>(ftpConnection);
                if (options == null)
                {
                    throw new ArgumentNullException(nameof(options));
                }
                return new FtpClient(options);
            }
        }

        public async Task RegisterFtpClientAsync(FtpConnectionOptions options)
        {
            var context = _httpContextAccessor.HttpContext;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, options.User),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await context.SignInAsync(principal);

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            var cookies = context.Response.Cookies;
            string ftpConnection = JsonSerializer.Serialize(options);
            cookies.Append(_ftpConnectionCookieKey, ftpConnection);
        }
    }
}
