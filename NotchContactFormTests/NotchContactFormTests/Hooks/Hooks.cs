using Reqnroll;
using Reqnroll.BoDi;
using NotchContactFormTests.Config;
using NotchContactFormTests.Drivers;
using NotchContactFormTests.Helpers;
using NotchContactFormTests.Pages;

namespace NotchContactFormTests.Hooks
{
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer _container;
        private readonly ScenarioContext _scenarioContext;
        private DriverContext? _driverContext;

        public Hooks(IObjectContainer container, ScenarioContext scenarioContext)
        {
            _container = container;
            _scenarioContext = scenarioContext;
        }


        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            ExtentReportHelper.Instance.InitializeReport();
        }


        [AfterTestRun]
        public static void AfterTestRun()
        {
            ExtentReportHelper.Instance.FlushReport();
        }


        [BeforeScenario]
        public void BeforeScenario()
        {
            _driverContext = new DriverContext();
            _container.RegisterInstanceAs(_driverContext);

            var waitHelper = new WaitHelper(_driverContext.Driver);
            _container.RegisterInstanceAs(waitHelper);

            var screenshotHelper = new ScreenshotHelper(_driverContext.Driver);
            _container.RegisterInstanceAs(screenshotHelper);

            var contactPage = new ContactFormPage(_driverContext.Driver, waitHelper, ConfigReader.BaseUrl);
            _container.RegisterInstanceAs(contactPage);

            var tags = string.Join(", ", _scenarioContext.ScenarioInfo.Tags);
            var testDescription = string.IsNullOrEmpty(tags) ? null : $"Tags: {tags}";
            ExtentReportHelper.Instance.CreateTest(_scenarioContext.ScenarioInfo.Title, testDescription);
        }


        [AfterScenario]
        public void AfterScenario()
        {
            if (_scenarioContext.TestError != null)
            {
                try
                {
                    var screenshotHelper = _container.Resolve<ScreenshotHelper>();
                    var base64 = screenshotHelper.CaptureScreenshotAsBase64(
                        _scenarioContext.ScenarioInfo.Title, "FAIL");

                    if (!string.IsNullOrEmpty(base64))
                        ExtentReportHelper.Instance.AttachScreenshot(base64, "Screenshot at failure");
                }
                catch { /*screenshot failure must not mask the original error */ }
            }

            _driverContext?.Dispose();
        }


        [AfterStep]
        public void AfterStep()
        {
            var stepInfo = _scenarioContext.StepContext.StepInfo;
            var stepText = $"{stepInfo.StepDefinitionType} {stepInfo.Text}";
            var stepStatus = _scenarioContext.StepContext.Status;

            if (stepStatus == ScenarioExecutionStatus.TestError)
            {
                ExtentReportHelper.Instance.LogFail(
                    $"Step FAILED: {stepText}\n{_scenarioContext.TestError?.Message}");
            }
            else
            {
                ExtentReportHelper.Instance.LogInfo($"✓ {stepText}");
            }
        }
    }
}