using Microsoft.Extensions.Configuration;

namespace NotchContactFormTests.Config
{
    public static class ConfigReader
    {
        private static readonly IConfiguration Config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        public static string BaseUrl             => Config["Settings:BaseUrl"]!;
        public static string Browser             => Config["Settings:Browser"] ?? "Chrome";
        public static bool   Headless            => bool.Parse(Config["Settings:Headless"] ?? "false");
        public static int    DefaultTimeout      => int.Parse(Config["Settings:DefaultTimeoutSeconds"] ?? "15");
        public static int    PageLoadTimeoutSeconds => int.Parse(Config["Settings:PageLoadTimeoutSeconds"] ?? "30");
        public static string ScreenshotsPath     => Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                        Config["Settings:ScreenshotsFolder"] ?? "Screenshots");
        public static string ReportsPath         => Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                        Config["Settings:ReportsFolder"] ?? "Reports");
        public static string ReportFileName      => Config["Settings:ReportFileName"] ?? "NotchContactFormReport";
    }
}
