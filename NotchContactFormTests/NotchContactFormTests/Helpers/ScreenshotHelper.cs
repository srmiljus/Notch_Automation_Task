using OpenQA.Selenium;
using NotchContactFormTests.Config;

namespace NotchContactFormTests.Helpers
{
    // Handles capturing browser screenshots and persisting them to disk.
    // Also returns the screenshot as a Base64 string so it can be embedded in reports.
    public class ScreenshotHelper
    {
        private readonly IWebDriver _driver;
        // Directory where screenshot files are saved; created on first use if it doesn't exist.
        private readonly string _screenshotsDir;

        // Initialises the helper, resolves the screenshots directory from config,
        // and ensures the directory exists before any screenshot is attempted.
        public ScreenshotHelper(IWebDriver driver)
        {
            _driver = driver;
            _screenshotsDir = ConfigReader.ScreenshotsPath;
            Directory.CreateDirectory(_screenshotsDir);
        }

        // Takes a screenshot of the current browser state, saves it to disk with a
        // descriptive file name, and returns the image as a Base64-encoded string.
        // Returns an empty string if the screenshot fails, to avoid crashing the test.
        public string CaptureScreenshotAsBase64(string scenarioTitle, string stepName = "")
        {
            try
            {
                var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();

                // Build a safe file name from the scenario title, optional step name, and a timestamp.
                var sanitizedTitle = SanitizeFileName(scenarioTitle);
                var sanitizedStep = string.IsNullOrWhiteSpace(stepName) ? "" : $"_{SanitizeFileName(stepName)}";
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
                var fileName = $"{sanitizedTitle}{sanitizedStep}_{timestamp}.png";
                var filePath = Path.Combine(_screenshotsDir, fileName);

                screenshot.SaveAsFile(filePath);
                Console.WriteLine($"[Screenshot] Saved: {filePath}");
                // Return the Base64 string so callers can embed it directly in ExtentReports.
                return screenshot.AsBase64EncodedString;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Screenshot] Failed: {ex.Message}");
                return string.Empty;
            }
        }

        // Replaces characters that are illegal in file names with underscores
        // and caps the result at 80 characters to avoid OS path-length limits.
        private static string SanitizeFileName(string input)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var sanitized = string.Join("_", input.Split(invalid, StringSplitOptions.RemoveEmptyEntries));
            return sanitized.Length > 80 ? sanitized[..80] : sanitized;
        }
    }
}
