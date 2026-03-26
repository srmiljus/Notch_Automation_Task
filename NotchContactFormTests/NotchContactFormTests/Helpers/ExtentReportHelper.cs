using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;
using NotchContactFormTests.Config;

namespace NotchContactFormTests.Helpers
{
    /// <summary>
    /// Thread-safe singleton that manages the ExtentReports HTML report lifecycle.
    /// Call InitializeReport() once before the test run and FlushReport() once after.
    /// </summary>
    public sealed class ExtentReportHelper
    {
        private static ExtentReportHelper? _instance;
        private static readonly object _lock = new();

        private ExtentReports _extent = null!;
        private string _reportPath = string.Empty;

        // [ThreadStatic] ensures each thread (parallel scenario) tracks its own current test node
        [ThreadStatic]
        private static ExtentTest? _currentTest;

        // Double-checked locking for thread-safe lazy initialization
        public static ExtentReportHelper Instance
        {
            get
            {
                lock (_lock)
                {
                    _instance ??= new ExtentReportHelper();
                    return _instance;
                }
            }
        }

        private ExtentReportHelper() { }

        public void InitializeReport()
        {
            var reportsDir = ConfigReader.ReportsPath;
            Directory.CreateDirectory(reportsDir);

            // Timestamp in filename prevents overwriting previous runs
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var reportFileName = $"{ConfigReader.ReportFileName}_{timestamp}.html";
            _reportPath = Path.Combine(reportsDir, reportFileName);

            var htmlReporter = new ExtentSparkReporter(_reportPath);
            htmlReporter.Config.DocumentTitle = "Notch Contact Form - Test Report";
            htmlReporter.Config.ReportName = "Notch QA Task — Contact Form Automation";
            htmlReporter.Config.Theme = Theme.Dark;
            htmlReporter.Config.TimeStampFormat = "dd/MM/yyyy HH:mm:ss";

            _extent = new ExtentReports();
            _extent.AttachReporter(htmlReporter);
            _extent.AddSystemInfo("Application", "Notch Contact Form");
            _extent.AddSystemInfo("URL", ConfigReader.BaseUrl);
            _extent.AddSystemInfo("Browser", ConfigReader.Browser);
            _extent.AddSystemInfo("Environment", "QA");
            _extent.AddSystemInfo("Framework", "Selenium + Reqnroll + NUnit");

            Console.WriteLine($"[Report] Initialized: {_reportPath}");
        }

        public ExtentTest CreateTest(string testName, string? description = null)
        {
            _currentTest = _extent.CreateTest(testName, description);
            return _currentTest;
        }

        public ExtentTest? GetCurrentTest() => _currentTest;

        // Logging methods delegate to the current test node — safe to call even if test is null
        public void LogInfo(string message) => _currentTest?.Info(message);
        public void LogPass(string message) => _currentTest?.Pass(message);
        public void LogFail(string message) => _currentTest?.Fail(message);
        public void LogWarning(string message) => _currentTest?.Warning(message);
        public void LogSkip(string message) => _currentTest?.Skip(message);

        public void AttachScreenshot(string base64Screenshot, string title = "Screenshot")
        {
            if (string.IsNullOrEmpty(base64Screenshot)) return;
            _currentTest?.AddScreenCaptureFromBase64String(base64Screenshot, title);
        }

        /// <summary>
        /// Writes the report to disk. Must be called after all tests complete (e.g., in [AfterTestRun] hook).
        /// </summary>
        public void FlushReport()
        {
            _extent?.Flush();
            Console.WriteLine($"[Report] Flushed to: {_reportPath}");
        }
    }
}