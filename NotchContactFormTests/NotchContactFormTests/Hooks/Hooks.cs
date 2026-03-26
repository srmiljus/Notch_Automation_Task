using Reqnroll;
using Reqnroll.BoDi;
using NotchContactFormTests.Config;
using NotchContactFormTests.Drivers;
using NotchContactFormTests.Helpers;
using NotchContactFormTests.Pages;

namespace NotchContactFormTests.Hooks
{
    // Reqnroll binding class that manages shared test infrastructure for every scenario.
    // Handles WebDriver lifecycle, ExtentReports integration, and screenshot capture on failure.
    [Binding]
    public class Hooks
    {
        // Tag used in feature files to mark scenarios that are expected to fail intentionally.
        private const string IntentionalFailTag = "intentional-fail";

        // Reqnroll dependency-injection container — used to register and resolve shared objects.
        private readonly IObjectContainer _container;
        // Provides metadata about the currently running scenario (title, tags, status).
        private readonly ScenarioContext _scenarioContext;
        // Holds the WebDriver for the current scenario; disposed in AfterScenario.
        private DriverContext? _driverContext;

        // Constructor receives the DI container and scenario context via Reqnroll's built-in injection.
        public Hooks(IObjectContainer container, ScenarioContext scenarioContext)
        {
            _container = container;
            _scenarioContext = scenarioContext;
        }


        // Initialises the ExtentReports HTML report once before any test in the run starts.
        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            ExtentReportHelper.Instance.InitializeReport();
        }


        // Flushes all buffered report data to disk after all tests in the run have finished.
        [AfterTestRun]
        public static void AfterTestRun()
        {
            ExtentReportHelper.Instance.FlushReport();
        }


        // Runs before each scenario: creates the WebDriver, registers all shared helpers in the
        // DI container, and opens a new ExtentReports test node for the current scenario.
        [BeforeScenario]
        public void BeforeScenario()
        {
            // Create and register the driver context so it is accessible to step definitions.
            _driverContext = new DriverContext();
            _container.RegisterInstanceAs(_driverContext);

            // Register the wait helper scoped to the current driver instance.
            var waitHelper = new WaitHelper(_driverContext.Driver);
            _container.RegisterInstanceAs(waitHelper);

            // Register the screenshot helper, which also creates the screenshots directory.
            var screenshotHelper = new ScreenshotHelper(_driverContext.Driver);
            _container.RegisterInstanceAs(screenshotHelper);

            // Register the page object pre-configured with the base URL from config.
            var contactPage = new ContactFormPage(_driverContext.Driver, waitHelper, ConfigReader.BaseUrl);
            _container.RegisterInstanceAs(contactPage);

            // Build an optional description showing the tags attached to the scenario.
            var tags = string.Join(", ", _scenarioContext.ScenarioInfo.Tags);
            var testDescription = string.IsNullOrEmpty(tags) ? null : $"Tags: {tags}";
            ExtentReportHelper.Instance.CreateTest(_scenarioContext.ScenarioInfo.Title, testDescription);
        }


        // Runs after each scenario: captures a failure screenshot if the scenario errored,
        // marks intentional failures appropriately in the report, and disposes the driver.
        [AfterScenario]
        public void AfterScenario()
        {
            if (_scenarioContext.TestError != null)
            {
                // Check whether this scenario is tagged as an expected/intentional failure.
                bool isKnownFailure = _scenarioContext.ScenarioInfo.Tags.Contains(IntentionalFailTag);

                try
                {
                    var screenshotHelper = _container.Resolve<ScreenshotHelper>();
                    var screenshotLabel = isKnownFailure ? "EXPECTED FAIL" : "FAIL";
                    var base64 = screenshotHelper.CaptureScreenshotAsBase64(
                        _scenarioContext.ScenarioInfo.Title, screenshotLabel);

                    // Attach the screenshot to the report only when it was captured successfully.
                    if (!string.IsNullOrEmpty(base64))
                        ExtentReportHelper.Instance.AttachScreenshot(base64, $"Screenshot at {screenshotLabel}");
                }
                catch { /* screenshot failure must not mask the original error */ }

                if (isKnownFailure)
                {
                    // Annotate the report node to show this failure was intentional.
                    ExtentReportHelper.Instance.MarkTestAsKnownFailure(
                        "Assertion uses a deliberately incorrect expected value to demonstrate failure reporting in ExtentReports.");
                }
            }

            // Quit the browser regardless of test outcome.
            _driverContext?.Dispose();
        }


        // Runs after each individual step: logs a pass or fail message for the step to the report.
        // Differentiates between intentional failures and genuine errors in the log output.
        [AfterStep]
        public void AfterStep()
        {
            var stepInfo = _scenarioContext.StepContext.StepInfo;
            var stepText = $"{stepInfo.StepDefinitionType} {stepInfo.Text}";
            var stepStatus = _scenarioContext.StepContext.Status;

            if (stepStatus == ScenarioExecutionStatus.TestError)
            {
                bool isKnownFailure = _scenarioContext.ScenarioInfo.Tags.Contains(IntentionalFailTag);

                if (isKnownFailure)
                {
                    // Log as a warning so the report clearly shows the failure was intentional.
                    ExtentReportHelper.Instance.LogWarning(
                        $"[EXPECTED FAILURE] Step: {stepText}\n{_scenarioContext.TestError?.Message}");
                }
                else
                {
                    ExtentReportHelper.Instance.LogFail(
                        $"Step FAILED: {stepText}\n{_scenarioContext.TestError?.Message}");
                }
            }
            else
            {
                // Step passed — log a simple confirmation with a checkmark prefix.
                ExtentReportHelper.Instance.LogInfo($"✓ {stepText}");
            }
        }
    }
}