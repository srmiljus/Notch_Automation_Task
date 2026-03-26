using Microsoft.Extensions.Configuration;

namespace NotchContactFormTests.Config
{
    /// <summary>
    /// Provides strongly-typed access to appsettings.json configuration.
    /// Headless mode can be overridden at runtime via the BROWSER_HEADLESS environment variable,
    /// which allows CI pipelines to run headless without modifying the config file.
    /// </summary>
    public static class ConfigReader
    {
        private static readonly IConfiguration Config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        public static string BaseUrl => Config["Settings:BaseUrl"]!;
        public static string Browser => Config["Settings:Browser"] ?? "Chrome";

        // Environment variable takes precedence over appsettings.json to support CI overrides
        public static bool Headless => bool.TryParse(Environment.GetEnvironmentVariable("BROWSER_HEADLESS"), out var envHeadless)
                                                          ? envHeadless
                                                          : bool.Parse(Config["Settings:Headless"] ?? "false");

        public static int DefaultTimeout => int.Parse(Config["Settings:DefaultTimeoutSeconds"] ?? "15");
        public static int PageLoadTimeoutSeconds => int.Parse(Config["Settings:PageLoadTimeoutSeconds"] ?? "30");

        // Paths are resolved relative to the build output directory
        public static string ScreenshotsPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                          Config["Settings:ScreenshotsFolder"] ?? "Screenshots");
        public static string ReportsPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                          Config["Settings:ReportsFolder"] ?? "Reports");
        public static string ReportFileName => Config["Settings:ReportFileName"] ?? "NotchContactFormReport";
    }
}