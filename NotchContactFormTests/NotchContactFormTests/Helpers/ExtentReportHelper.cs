using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;
using NotchContactFormTests.Config;

namespace NotchContactFormTests.Helpers
{
    // Thread-safe singleton that wraps AventStack ExtentReports.
    // One report is created per test run, and individual test nodes are tracked per thread.
    public sealed class ExtentReportHelper
    {
        // Singleton instance; created lazily inside a lock to be thread-safe.
        private static ExtentReportHelper? _instance;
        private static readonly object _lock = new();

        private ExtentReports _extent = null!;
        // Full path to the generated HTML report file, set during InitializeReport.
        private string _reportPath = string.Empty;

        // Each test thread gets its own ExtentTest node so parallel runs don't interfere.
        [ThreadStatic]
        private static ExtentTest? _currentTest;

        // Returns the singleton instance, creating it on first access inside a lock.
        public static ExtentReportHelper Instance
        {
            get
            {
                lock (_lock)
                {
                    // Null-coalescing assignment ensures only one instance is ever created.
                    _instance ??= new ExtentReportHelper();
                    return _instance;
                }
            }
        }

        // Private constructor prevents external instantiation (enforces singleton pattern).
        private ExtentReportHelper() { }

        // Initialises the ExtentReports engine and configures the HTML Spark reporter.
        // Must be called once before any tests run (e.g., from a [BeforeTestRun] hook).
        public void InitializeReport()
        {
            var reportsDir = ConfigReader.ReportsPath;
            Directory.CreateDirectory(reportsDir);

            // Append a timestamp to the file name so each run creates a unique report.
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

            // Add system-level metadata that appears in the report header.
            _extent.AddSystemInfo("Application", "Notch Contact Form");
            _extent.AddSystemInfo("URL", ConfigReader.BaseUrl);
            _extent.AddSystemInfo("Browser", ConfigReader.Browser);
            _extent.AddSystemInfo("Environment", "QA");
            _extent.AddSystemInfo("Framework", "Selenium + Reqnroll + NUnit");

            Console.WriteLine($"[Report] Initialized: {_reportPath}");
        }

        // Creates a new test node in the report for the current scenario and stores it
        // in the thread-static field so subsequent log calls target this test.
        public ExtentTest CreateTest(string testName, string? description = null)
        {
            _currentTest = _extent.CreateTest(testName, description);
            return _currentTest;
        }

        // Returns the ExtentTest node currently associated with the running thread.
        public ExtentTest? GetCurrentTest() => _currentTest;

        // Logs an informational message to the current test node.
        public void LogInfo(string message)
            => _currentTest?.Info(message);

        // Logs a pass message to the current test node.
        public void LogPass(string message)
            => _currentTest?.Pass(message);

        // Logs a failure message to the current test node.
        public void LogFail(string message)
            => _currentTest?.Fail(message);

        // Logs a warning message to the current test node.
        public void LogWarning(string message)
            => _currentTest?.Warning(message);

        // Marks the current test node as skipped with the provided message.
        public void LogSkip(string message)
            => _currentTest?.Skip(message);

        // Logs a warning prefixed with "[EXPECTED FAILURE]" to distinguish intentional failures.
        public void LogKnownFailure(string message)
            => _currentTest?.Warning($"[EXPECTED FAILURE] {message}");

        // Annotates the current test node as a known/expected failure with a descriptive reason.
        public void MarkTestAsKnownFailure(string reason)
        {
            _currentTest?.Warning($"⚠️ [KNOWN / EXPECTED FAILURE] This test is intentionally failing to demonstrate failure reporting.\nReason: {reason}");
        }

        // Embeds a Base64-encoded screenshot into the current test node.
        // Silently skips if the screenshot string is null or empty.
        public void AttachScreenshot(string base64Screenshot, string title = "Screenshot")
        {
            if (string.IsNullOrEmpty(base64Screenshot)) return;
            _currentTest?.AddScreenCaptureFromBase64String(base64Screenshot, title);
        }

        // Writes all buffered test data to the HTML report file.
        // Must be called once after all tests have finished (e.g., from an [AfterTestRun] hook).
        public void FlushReport()
        {
            _extent?.Flush();
            Console.WriteLine($"[Report] Flushed to: {_reportPath}");
        }


    }
}
