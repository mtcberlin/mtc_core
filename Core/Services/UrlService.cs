using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using MtcMvcCore.Core.Models;
using MtcMvcCore.Core.Models.PageModels;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Services
{
    public class UrlService : IUrlService
    {

        private IContextService _contextService;

        public UrlService(IContextService contextService)
        {
            _contextService = contextService;
        }

        public LinkInformation GetLinkInformationById(string id, string langOverride = null, bool easy = false)
        {
            return GetLinkInformationById(Guid.Parse(id), langOverride, easy);
        }

        public LinkInformation GetLinkInformationById(Guid id, string langOverride = null, bool easy = false)
        {
            if (Guid.Empty == id)
            {
                return new LinkInformation();
            }

            var currentLang = string.IsNullOrEmpty(langOverride) ? _contextService.CurrentLanguage : langOverride;
            var pageContextModel = SiteConfiguration.PageContextModels.FirstOrDefault(i => i.Value.Page.GroupId == id && i.Value.Language
             == currentLang);

            if (pageContextModel.Value == null)
            {
                return new LinkInformation();
            }

            var pageTitle = string.IsNullOrEmpty(pageContextModel.Value.Page.Title) ? pageContextModel.Value.Page.Name : pageContextModel.Value.Page.Title;

            var lang = string.IsNullOrEmpty(pageContextModel.Value.Language) ? easy ? "easy" : string.Empty : easy ? $"/{pageContextModel.Value.Language}_easy" : $"/{pageContextModel.Value.Language}";
            return new LinkInformation { Href = $"{lang}{pageContextModel.Value.SeoUrlWithoutLang}", Title = pageTitle };
        }

        public string CreateEasyLink(HttpContext context)
        {
            var lang = context.Items["lang"] ?? Settings.DefaultLanguage;
            var isEasy = context.Items["easy"]?.ToString().ToLower();
            var easyPath = "_easy";
            if (isEasy == "true") easyPath = string.Empty;
            var path = context.Items["path"]?.ToString() ?? context.Request.Path.ToString();
            return $"/{lang}{easyPath}{path.Replace($"/{lang}", string.Empty)}";
        }

        public string GetEasyUrl(HttpContext context)
        {
            var lang = context.Items["lang"] ?? Settings.DefaultLanguage;
            var easyPath = "_easy";
            var path = context.Items["path"]?.ToString() ?? context.Request.Path.ToString();
            return $"/{lang}{easyPath}{path.Replace($"/{lang}", string.Empty)}";
        }

        public string GetNormalUrl(HttpContext context)
        {
            var lang = context.Items["lang"] ?? Settings.DefaultLanguage;
            var path = context.Items["path"]?.ToString() ?? context.Request.Path.ToString();
            return $"/{lang}{path.Replace($"/{lang}", string.Empty)}";
        }


        private BaseItem FindPage(List<BaseItem> pages, Guid pageId)
        {
            BaseItem result = null;
            foreach (var page in pages)
            {
                if (page.Id == pageId)
                {
                    result = page;
                    break;
                }

                if (page.SubItems.Count > 0)
                {
                    var found = FindPage(page.SubItems, pageId);
                    if (found != null)
                    {
                        result = found;
                    }
                }
            }

            return result;
        }

    }
}