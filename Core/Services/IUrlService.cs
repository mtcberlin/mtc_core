using System;
using Microsoft.AspNetCore.Http;
using MtcMvcCore.Core.Models;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Services
{
    public interface IUrlService
    {
        LinkInformation GetLinkInformationById(string id, string langOverride = null, bool easy = false);
        LinkInformation GetLinkInformationById(Guid id, string langOverride = null, bool easy = false);
        string CreateEasyLink(HttpContext context);
        string GetEasyUrl(HttpContext context);
        string GetNormalUrl(HttpContext context);

    }
}
