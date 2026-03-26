using Microsoft.Extensions.Configuration;

namespace NotchContactFormTests.Config
{
    // Provides strongly-typed access to all test configuration values read from appsettings.json.
    // All members are static so the reader can be used without instantiation throughout the project.
    public static class ConfigReader
    {
        // Build the configuration once at startup by loading appsettings.json from the working directory.
        private static readonly IConfiguration Config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        // Base URL of the application under test (e.g. the contact form page).
        public static string BaseUrl             => Config["Settings:BaseUrl"]!;

        // Browser to use for the test run; defaults to "Chrome" when not specified in config.
        public static string Browser             => Config["Settings:Browser"] ?? "Chrome";

        // Whether to run the browser in headless mode.
        // The BROWSER_HEADLESS environment variable takes priority over the appsettings.json value.
        public static bool   Headless            => bool.TryParse(Environment.GetEnvironmentVariable("BROWSER_HEADLESS"), out var envHeadless)
                                                        ? envHeadless
                                                        : bool.Parse(Config["Settings:Headless"] ?? "false");

        // Maximum number of seconds to wait for an element before timing out; defaults to 15.
        public static int    DefaultTimeout      => int.Parse(Config["Settings:DefaultTimeoutSeconds"] ?? "15");

        // Maximum number of seconds to wait for a full page load; defaults to 30.
        public static int    PageLoadTimeoutSeconds => int.Parse(Config["Settings:PageLoadTimeoutSeconds"] ?? "30");

        // Absolute path to the folder where screenshots are saved during test runs.
        public static string ScreenshotsPath     => Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                        Config["Settings:ScreenshotsFolder"] ?? "Screenshots");

        // Absolute path to the folder where HTML test reports are written.
        public static string ReportsPath         => Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                        Config["Settings:ReportsFolder"] ?? "Reports");

        // Base file name used when generating the HTML test report (a timestamp is appended at runtime).
        public static string ReportFileName      => Config["Settings:ReportFileName"] ?? "NotchContactFormReport";
    }
}
