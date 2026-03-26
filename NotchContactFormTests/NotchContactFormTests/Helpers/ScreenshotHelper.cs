using OpenQA.Selenium;
using NotchContactFormTests.Config;

namespace NotchContactFormTests.Helpers
{
    /// <summary>
    /// Captures screenshots and saves them to disk as PNG files.
    /// Also returns the screenshot as a base64 string for embedding in ExtentReports.
    /// </summary>
    public class ScreenshotHelper
    {
        private readonly IWebDriver _driver;
        private readonly string _screenshotsDir;

        public ScreenshotHelper(IWebDriver driver)
        {
            _driver = driver;
            _screenshotsDir = ConfigReader.ScreenshotsPath;
            Directory.CreateDirectory(_screenshotsDir);
        }

        /// <summary>
        /// Captures a screenshot, saves it to disk, and returns it as a base64 string.
        /// Returns an empty string if the capture fails (e.g., driver is unavailable).
        /// </summary>
        public string CaptureScreenshotAsBase64(string scenarioTitle, string stepName = "")
        {
            try
            {
                var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();

                var sanitizedTitle = SanitizeFileName(scenarioTitle);
                var sanitizedStep = string.IsNullOrWhiteSpace(stepName) ? "" : $"_{SanitizeFileName(stepName)}";
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
                var fileName = $"{sanitizedTitle}{sanitizedStep}_{timestamp}.png";
                var filePath = Path.Combine(_screenshotsDir, fileName);

                screenshot.SaveAsFile(filePath);
                Console.WriteLine($"[Screenshot] Saved: {filePath}");
                return screenshot.AsBase64EncodedString;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Screenshot] Failed: {ex.Message}");
                return string.Empty;
            }
        }

        // Replaces invalid filename characters with underscores and enforces an 80-char limit
        private static string SanitizeFileName(string input)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var sanitized = string.Join("_", input.Split(invalid, StringSplitOptions.RemoveEmptyEntries));
            return sanitized.Length > 80 ? sanitized[..80] : sanitized;
        }
    }
}