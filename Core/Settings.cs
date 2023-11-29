using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core
{
	public abstract class Settings
	{
		
		private static IConfiguration _appConfiguration;
		public static bool EnableBasicAuthProtection => _appConfiguration.GetValue("AppProtection:EnableBasicAuth", false);
		public static string BasicAuthUsername => _appConfiguration.GetValue<string>("AppProtection:BasicAuthUsername", string.Empty);
		public static string BasicAuthPassword => _appConfiguration.GetValue<string>("AppProtection:BasicAuthPassword", string.Empty);
		public static string DefaultLanguage => _appConfiguration.GetValue<string>("Language:default", "de");
		public static List<string> AllowedLanguage => _appConfiguration.GetSection("Language:allowedLanguages").Get<List<string>>();

		public static bool AllowEditMode =>
			_appConfiguration.GetValue("AdditionalFunctions:AllowEditMode", false);
		public static bool AllowDebugMode =>
			_appConfiguration.GetValue("AdditionalFunctions:AllowDebugMode", false);
		public static bool AllowContentEditor =>
			_appConfiguration.GetValue("AdditionalFunctions:AllowContentEditor", false);
		public static bool AllowAssetLib =>
			_appConfiguration.GetValue("AdditionalFunctions:AllowAssetLib", false);
		public static bool AllowContentPackages =>
			_appConfiguration.GetValue("AdditionalFunctions:AllowContentPackages", false);
		public static bool EnableFileVersions =>
			_appConfiguration.GetValue("AdditionalFunctions:EnableFileVersions", false);

		public static bool IsDebugInformationEnabled =>
			_appConfiguration.GetValue("AdditionalFunctions:DebugInformation", false);

		public static string PathsCorePath => _appConfiguration.GetValue<string>("Paths:CorePath");
		public static bool IsCachingEnabled => _appConfiguration.GetValue("Caching:Enable", true);
		public static string DatastoreType => _appConfiguration.GetValue("Datastore:PageStructure", "Xml");
		public static string MongoDbConnectionString => _appConfiguration.GetValue<string>("MongoDbOptions:ConnectionString");
		public static string MongoDbAuthSource => _appConfiguration.GetValue<string>("MongoDbOptions:AuthSource", string.Empty);
		public static string MongoDbStructureDbName => _appConfiguration.GetValue<string>("MongoDbOptions:StructureDBName");

		#region Email

		public static string EmailServer => _appConfiguration.GetValue<string>("EmailSettings:Server");
		public static int EmailPort => _appConfiguration.GetValue<int>("EmailSettings:Port");
		public static bool EmailSsl => _appConfiguration.GetValue<bool>("EmailSettings:SSL");
		public static string EmailUser => _appConfiguration.GetValue<string>("EmailSettings:EmailUser");
		public static string EmailPasswort => _appConfiguration.GetValue<string>("EmailSettings:EmailPasswort");

		#endregion

		public static int StaticFilesCachingDuration => _appConfiguration.GetValue("StaticFilesCaching:DurationInDays", 0);

		public static string ImageTransformFormat =>
			_appConfiguration.GetValue("ImageTransformation:useExtension", "webp");

		public static List<double> AdditionalImageSizes => _appConfiguration.GetSection("ImageTransformation:additionalImageSizes").Get<List<double>>();

		public static void Configure(IConfiguration appConfiguration)
		{
			_appConfiguration = appConfiguration;
		}

	}
}