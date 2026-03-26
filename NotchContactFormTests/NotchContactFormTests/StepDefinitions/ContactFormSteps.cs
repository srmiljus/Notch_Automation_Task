using NUnit.Framework;
using Reqnroll;
using Bogus;
using NotchContactFormTests.Helpers;
using NotchContactFormTests.Pages;

namespace NotchContactFormTests.StepDefinitions
{
    [Binding]
    public class ContactFormSteps
    {
        private readonly ContactFormPage _contactPage;
        private readonly ExtentReportHelper _report;
        private static readonly Faker _faker = new();

        public ContactFormSteps(ContactFormPage contactPage)
        {
            _contactPage = contactPage;
            _report = ExtentReportHelper.Instance;
        }

        #region Given

        [Given(@"I navigate to the contact form page")]
        public void GivenINavigateToTheContactFormPage()
        {
            _contactPage.NavigateTo();
            _report.LogInfo("Navigated to contact form page");
        }

        #endregion

        #region When

        [When(@"I enter ""(.*)"" in the first name field")]
        public void WhenIEnterInTheFirstNameField(string value)
        {
            _contactPage.EnterFirstName(value);
            _report.LogInfo($"Entered first name: '{value}'");
        }

        [When(@"I enter ""(.*)"" in the last name field")]
        public void WhenIEnterInTheLastNameField(string value)
        {
            _contactPage.EnterLastName(value);
            _report.LogInfo($"Entered last name: '{value}'");
        }

        [When(@"I enter ""(.*)"" in the email field")]
        public void WhenIEnterInTheEmailField(string value)
        {
            _contactPage.EnterEmail(value);
            _report.LogInfo($"Entered email: '{value}'");
        }

        [When(@"I enter a generated email in the email field")]
        public void WhenIEnterAGeneratedEmailInTheEmailField()
        {
            var email = _faker.Internet.Email();
            _contactPage.EnterEmail(email);
            _report.LogInfo($"Entered generated email: '{email}'");
        }

        [When(@"I enter ""(.*)"" in the phone number field")]
        public void WhenIEnterInThePhoneNumberField(string value)
        {
            _contactPage.EnterPhoneNumber(value);
            _report.LogInfo($"Entered phone: '{value}'");
        }

        [When(@"I enter ""(.*)"" in the company field")]
        public void WhenIEnterInTheCompanyField(string value)
        {
            _contactPage.EnterCompany(value);
            _report.LogInfo($"Entered company: '{value}'");
        }

        [When(@"I enter ""(.*)"" in the project details field")]
        public void WhenIEnterInTheProjectDetailsField(string value)
        {
            _contactPage.EnterProjectDetails(value);
            _report.LogInfo($"Entered project details");
        }

        [When(@"I select ""(.*)"" from the how did you hear about us dropdown")]
        public void WhenISelectFromHowDidYouHearDropdown(string option)
        {
            _contactPage.SelectHowDidYouHear(option);
            _report.LogInfo($"Selected referral source: '{option}'");
        }

        [When(@"I select ""(.*)"" from the budget dropdown")]
        public void WhenISelectFromTheBudgetDropdown(string option)
        {
            _contactPage.SelectBudget(option);
            _report.LogInfo($"Selected budget: '{option}'");
        }

        [When(@"I select the service ""(.*)""")]
        public void WhenISelectTheService(string serviceLabel)
        {
            _contactPage.SelectService(serviceLabel);
            _report.LogInfo($"Selected service: '{serviceLabel}'");
        }

        [When(@"I accept the privacy policy consent")]
        public void WhenIAcceptThePrivacyPolicyConsent()
        {
            _contactPage.AcceptConsent();
            _report.LogInfo("Accepted privacy policy consent");
        }

        [When(@"I click the Send message button")]
        public void WhenIClickTheSendMessageButton()
        {
            _contactPage.ClickSendMessage();
            _report.LogInfo("Clicked Send message button");
        }


        [When("I upload the file {string}")]
        public void WhenIUploadTheFile(string fileName)
        {
            var filePath = Path.Combine(
               AppDomain.CurrentDomain.BaseDirectory, "TestData", fileName);

            _contactPage.UploadFile(filePath);
            _report.LogInfo($"Uploaded file: '{fileName}'");
        }

        #endregion

        #region Then

        [Then("a success confirmation message {string} should be displayed")]
        public void ThenASuccessConfirmationMessageShouldBeDisplayed(string message)
        {

            var actualText = _contactPage.GetSuccessMessageText();
            _report.LogInfo($"Expected: '{message}' | Actual: '{actualText}'");
            Assert.That(actualText, Does.Contain(message),
                $"Success message mismatch. Expected to contain: '{message}', Actual: '{actualText}'");
        }


        [Then(@"the success message should contain ""(.*)""")]
        public void ThenTheSuccessMessageShouldContain(string expectedText)
        {
            var actualText = _contactPage.GetSuccessMessageText();
            _report.LogInfo($"Success message text: '{actualText}' | Expected to contain: '{expectedText}'");
            Assert.That(actualText, Does.Contain(expectedText),
                $"Expected success message to contain '{expectedText}' but got: '{actualText}'");
        }

        [Then("validation error message {string} should be displayed for {string} field")]
        public void ThenValidationErrorMessageShouldBeDisplayedForField(string message, string fieldName)
        {
            var actualText = _contactPage.GetValidationErrorText(fieldName);
            _report.LogInfo($"Field: '{fieldName}' | Expected: '{message}' | Actual: '{actualText}'");
            Assert.That(actualText, Does.Contain(message),
                $"Validation error mismatch for '{fieldName}'. Expected: '{message}', Actual: '{actualText}'");
        }
    }
        #endregion
}
