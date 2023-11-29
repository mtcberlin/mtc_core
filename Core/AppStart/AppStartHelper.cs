using System;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MtcMvcCore.Areas.Admin.Pages.ContentEditor.Services;
using MtcMvcCore.Core.Controllers;
using MtcMvcCore.Core.DataProvider.LiteDb;
using MtcMvcCore.Core.DataProvider.Rest;
using MtcMvcCore.Core.DataProvider.Xml;
using MtcMvcCore.Core.Helper;
using MtcMvcCore.Core.Services;
using NLog;
using Microsoft.Net.Http.Headers;
using AspNetCore.Identity.Mongo;
using MtcMvcCore.Core.DataProvider;
using MtcMvcCore.Core.DataProvider.MongoDb;
using MtcMvcCore.Core.Constants;
using MtcMvcCore.Core.DataProvider.MongoDb.Seed;
using MtcMvcCore.Core.Models.Authentication;
using MongoDB.Bson.Serialization.Conventions;
using MtcMvcCore.Areas.Admin.Pages.AssetLib.Services;
using MtcMvcCore.Areas.Admin.Pages.ContentPackages.Services;
using MongoDB.Bson.Serialization;
using MtcMvcCore.Core.Models.Media;
using MtcMvcCore.Core.DataProvider.Seed;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson;
using MtcMvcCore.Core.Models.PageModels;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Globalization;
using MtcMvcCore.Core.SharedDictionary;
using Microsoft.Extensions.Localization;
using MtcMvcCore.SharedComponents.CoreVideoPlayer.Service;
using MtcMvcCore.Core.Models.Attributes;
using System.Linq;
using MongoDB.Extensions.Migration;
using MtcMvcCore.Core.DataProvider.MongoDb.Migrations;
using MongoDB.Bson.Serialization.Serializers;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.AppStart
{
	public class AppStartHelper
	{
		private static Logger _logger;

		public static void AddCoreServiceConfiguration(IServiceCollection services, IConfiguration configuration)
		{
			Settings.Configure(configuration);

			RegisterServicesByAttribute(services);
			services.AddScoped<IContentEditorService, ContentEditorService>();
			services.AddScoped<IAssetLibService, AssetLibService>();
			services.AddScoped<IEditModeService, EditModeService>();
			services.AddSingleton<RouteValueTransformer>();
			services.AddScoped<IUrlService, UrlService>();
			services.AddScoped<IContextService, ContextService>();
			services.AddSingleton<IMediaService, MediaService>();
			services.AddSingleton<ICoreVideoPlayerService, CoreVideoPlayerService>();
			services.AddSingleton<IMailService, MailService>();

			services.AddRazorPages().AddViewLocalization();
			services.Configure<RequestLocalizationOptions>(options =>
			{
				List<CultureInfo> supportedCultures = new List<CultureInfo>();

				foreach (var lang in Settings.AllowedLanguage)
				{
					supportedCultures.Add(new CultureInfo(lang));
				}

				options.DefaultRequestCulture = new RequestCulture(culture: Settings.DefaultLanguage, uiCulture: Settings.DefaultLanguage);
				options.SupportedCultures = supportedCultures;
				options.SupportedUICultures = supportedCultures;
				options.RequestCultureProviders.Clear();

				options.RequestCultureProviders.Add(new CoreRequestCultureProvider());
			});

			HttpClient httpClient = new HttpClient() { };
			services.AddSingleton<HttpClient>(httpClient);

			//DataProvider
			services.AddSingleton<IXmlDataProvider, XmlDataProvider>();
			services.AddSingleton<IRestDataProvider, RestDataProvider>();
			services.AddSingleton<ILiteDbDataProvider, LiteDbDataProvider>();
			services.AddSingleton<ILiteDbContext, LiteDbContext>();
			services.AddSingleton<IMongoDbDataProvider, MongoDbDataProvider>();
			services.AddScoped<IContentPackagesService, ContentPackagesService>();

			services.Configure<RouteOptions>(options =>
			{
				options.ConstraintMap.Add("lang", typeof(LanguageRouteConstraint));
			});

			_logger = LogManager.GetCurrentClassLogger();
			if (configuration.GetValue<string>("Datastore:Identity") == DatastoreType.MONGODB)
			{
				_logger.Info($"Datastore:Identity use MongoDB");
#pragma warning disable 618
				BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
#pragma warning restore

				services.AddIdentityMongoDbProvider<MongoDbUserModel, MongoDbRole, Guid>(identity =>
				{
					identity.Password.RequiredLength = 8;
					// other options
				},
				mongo =>
				{
					mongo.ConnectionString = configuration.GetValue<string>("MongoDbOptions:ConnectionString")
						+ configuration.GetValue<string>("MongoDbOptions:IdentityUserDBName")
						+ configuration.GetValue<string>("MongoDbOptions:AuthSource", string.Empty);
					// other options
				});

				services.AddScoped<IUserDataProvider, MongoDbUserDataProvider>();
				services.AddScoped<IPageDataProvider, MongoDbPageDataProvider>();
				services.AddSingleton<IComponentDataProvider, MongoDbComponentDataProvider>();
				services.AddSingleton<IMediaDataProvider, MongoDbMediaDataProvider>();


				var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };

				ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);

				// TODO: check why
				BsonClassMap.RegisterClassMap<CoreMediaFolder>();
				BsonClassMap.RegisterClassMap<CoreImage>();
				BsonClassMap.RegisterClassMap<CoreVideo>();
				BsonClassMap.RegisterClassMap<ContentFolder>();
				BsonClassMap.RegisterClassMap<WebRoot>();

				var objectSerializer = new ObjectSerializer(x => true);
				BsonSerializer.RegisterSerializer(objectSerializer);
			}
			else
			{
				_logger.Info($"Datastore:Identity use XML File");
				services.AddScoped<IUserDataProvider, XmlUserDataProvider>();
				services.AddScoped<IPageDataProvider, XmlPageDataProvider>();
				services.AddSingleton<IComponentDataProvider, XmlComponentDataProvider>();
				services.AddSingleton<IMediaDataProvider, XmlMediaDataProvider>();

				services.AddAuthentication(options =>
				{
					options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
					options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				});
			}

			services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = new PathString("/admin/login");
				options.ExpireTimeSpan = TimeSpan.FromMinutes(30.0);
			});

			services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", options => { });
			services.AddAuthorization(options =>
			{
				options.AddPolicy("BasicAuthentication", new AuthorizationPolicyBuilder("BasicAuthentication").RequireAuthenticatedUser().Build());
			});
		}

		public static void ConfigureCore(IApplicationBuilder app, IServiceProvider serviceProvider)
		{
			var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
			app.UseRequestLocalization(options.Value);

			if (Settings.DatastoreType == DatastoreType.MONGODB)
			{
				app.UseMongoMigration(m => m
					.ForEntity<CoreImage>(e => e
						.AtVersion(2)
						.WithMigration(new UpdateCoreImageCopyrightMigration())));
			}
			Translation.AddLocalizer(app.ApplicationServices.GetService<IStringLocalizer<SharedCoreDictionary>>());

			ReadSiteConfiguration(app, serviceProvider);
			UseStaticFiles(app);
		}

		private static void UseStaticFiles(IApplicationBuilder app)
		{
			if (Settings.StaticFilesCachingDuration == 0)
			{
				app.UseStaticFiles(new StaticFileOptions
				{
					ContentTypeProvider = AdditionalFileExtensions()
				});
			}
			else
			{
				int durationInSeconds = 60 * 60 * 24 * Settings.StaticFilesCachingDuration;

				app.UseStaticFiles(new StaticFileOptions
				{
					OnPrepareResponse = ctx =>
					{
						ctx.Context.Response.Headers[HeaderNames.CacheControl]
						= "public,max-age=" + durationInSeconds;
					},
					ContentTypeProvider = AdditionalFileExtensions()
				});
			}
		}

		private static FileExtensionContentTypeProvider AdditionalFileExtensions()
		{
			var provider = new FileExtensionContentTypeProvider();

			provider.Mappings[".vtt"] = "text/vtt";

			return provider;
		}

		public static void ReadSiteConfiguration(IApplicationBuilder app, IServiceProvider serviceProvider)
		{
			if (Settings.DatastoreType == DatastoreType.MONGODB)
			{
				new MongoDbSeeding(serviceProvider.GetService<IMongoDbDataProvider>());
			}
			new IdentitySeeding(serviceProvider.GetService<IUserDataProvider>());

			SiteConfiguration.Configure(serviceProvider.GetService<IXmlDataProvider>(),
				serviceProvider.GetService<IMongoDbDataProvider>(),
				app.ApplicationServices.GetRequiredService<ILogger<SiteConfiguration>>(),
				serviceProvider.GetService<IPageDataProvider>()
			);
			SiteConfiguration.ReadConfigurations();
		}

		public static void RegisterRoutes(IEndpointRouteBuilder endpoints)
		{
			endpoints.MapDynamicControllerRoute<RouteValueTransformer>("{**part}");
		}



		public static void SetRequestMiddleware(IApplicationBuilder app)
		{
			app.UseMiddleware<MediaRequestMiddleware>();
			app.UseMiddleware<CoreRequestMiddleware>();
		}

		private static void RegisterServicesByAttribute(IServiceCollection services)
		{
			Type scopedService = typeof(ScopedServiceAttribute);
			Type singletonService = typeof(SingletonServiceAttribute);

			var types = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(s => s.GetTypes())
				.Where(p => p.IsDefined(scopedService, true) || p.IsDefined(singletonService, true) && !p.IsInterface).Select(s => new
				{
					Service = s.GetInterface($"I{s.Name}"),
					Implementation = s
				}).Where(x => x.Service != null);

			foreach (var type in types)
			{
				if (type.Implementation.IsDefined(scopedService, false))
				{
					services.AddScoped(type.Service, type.Implementation);
				}

				if (type.Implementation.IsDefined(singletonService, false))
				{
					services.AddSingleton(type.Service, type.Implementation);
				}
			}
		}
	}
}
