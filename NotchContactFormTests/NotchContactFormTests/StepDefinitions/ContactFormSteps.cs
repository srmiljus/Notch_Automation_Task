using NUnit.Framework;
using Reqnroll;
using Bogus;
using NotchContactFormTests.Helpers;
using NotchContactFormTests.Pages;

namespace NotchContactFormTests.StepDefinitions
{
    // Reqnroll step definition class that maps Gherkin steps to ContactFormPage actions.
    // Each method corresponds to a Given/When/Then step pattern from the feature files.
    [Binding]
    public class ContactFormSteps
    {
        private readonly ContactFormPage _contactPage;
        // Reference to the singleton report helper for logging step details.
        private readonly ExtentReportHelper _report;
        // Shared Bogus Faker instance used to generate realistic random test data.
        private static readonly Faker _faker = new();

        // Receives the ContactFormPage from Reqnroll's DI container (registered in Hooks).
        public ContactFormSteps(ContactFormPage contactPage)
        {
            _contactPage = contactPage;
            _report = ExtentReportHelper.Instance;
        }

        #region Given

        // Navigates to the contact form page before the scenario's When steps execute.
        [Given(@"I navigate to the contact form page")]
        public void GivenINavigateToTheContactFormPage()
        {
            _contactPage.NavigateTo();
            _report.LogInfo("Navigated to contact form page");
        }

        #endregion

        #region When

        // Enters the literal value provided in the feature file into the First Name field.
        [When(@"I enter ""(.*)"" in the first name field")]
        public void WhenIEnterInTheFirstNameField(string value)
        {
            _contactPage.EnterFirstName(value);
            _report.LogInfo($"Entered first name: '{value}'");
        }

        // Enters the literal value provided in the feature file into the Last Name field.
        [When(@"I enter ""(.*)"" in the last name field")]
        public void WhenIEnterInTheLastNameField(string value)
        {
            _contactPage.EnterLastName(value);
            _report.LogInfo($"Entered last name: '{value}'");
        }

        // Enters the literal email address provided in the feature file into the Email field.
        [When(@"I enter ""(.*)"" in the email field")]
        public void WhenIEnterInTheEmailField(string value)
        {
            _contactPage.EnterEmail(value);
            _report.LogInfo($"Entered email: '{value}'");
        }

        // Generates a random, realistic email address via Bogus and enters it into the Email field.
        // Useful for avoiding duplicate-submission issues when running tests against a live site.
        [When(@"I enter a generated email in the email field")]
        public void WhenIEnterAGeneratedEmailInTheEmailField()
        {
            var email = _faker.Internet.Email();
            _contactPage.EnterEmail(email);
            _report.LogInfo($"Entered generated email: '{email}'");
        }

        // Enters the literal value provided in the feature file into the Phone Number field.
        [When(@"I enter ""(.*)"" in the phone number field")]
        public void WhenIEnterInThePhoneNumberField(string value)
        {
            _contactPage.EnterPhoneNumber(value);
            _report.LogInfo($"Entered phone: '{value}'");
        }

        // Enters the literal value provided in the feature file into the Company field.
        [When(@"I enter ""(.*)"" in the company field")]
        public void WhenIEnterInTheCompanyField(string value)
        {
            _contactPage.EnterCompany(value);
            _report.LogInfo($"Entered company: '{value}'");
        }

        // Enters the literal value provided in the feature file into the Project Details textarea.
        [When(@"I enter ""(.*)"" in the project details field")]
        public void WhenIEnterInTheProjectDetailsField(string value)
        {
            _contactPage.EnterProjectDetails(value);
            _report.LogInfo($"Entered project details");
        }

        // Selects the given option from the "How did you hear about us?" Chosen dropdown.
        [When(@"I select ""(.*)"" from the how did you hear about us dropdown")]
        public void WhenISelectFromHowDidYouHearDropdown(string option)
        {
            _contactPage.SelectHowDidYouHear(option);
            _report.LogInfo($"Selected referral source: '{option}'");
        }

        // Selects the given budget range from the Budget Chosen dropdown.
        [When(@"I select ""(.*)"" from the budget dropdown")]
        public void WhenISelectFromTheBudgetDropdown(string option)
        {
            _contactPage.SelectBudget(option);
            _report.LogInfo($"Selected budget: '{option}'");
        }

        // Clicks the checkbox for the service whose label matches the given text.
        [When(@"I select the service ""(.*)""")]
        public void WhenISelectTheService(string serviceLabel)
        {
            _contactPage.SelectService(serviceLabel);
            _report.LogInfo($"Selected service: '{serviceLabel}'");
        }

        // Checks the privacy policy consent checkbox.
        [When(@"I accept the privacy policy consent")]
        public void WhenIAcceptThePrivacyPolicyConsent()
        {
            _contactPage.AcceptConsent();
            _report.LogInfo("Accepted privacy policy consent");
        }

        // Clicks the "Send message" submit button to submit the contact form.
        [When(@"I click the Send message button")]
        public void WhenIClickTheSendMessageButton()
        {
            _contactPage.ClickSendMessage();
            _report.LogInfo("Clicked Send message button");
        }


        // Resolves the file path relative to the test output's TestData folder and uploads the file.
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

        // Asserts that the success confirmation message contains the expected text.
        // Logs both the expected and actual text for easier debugging in the report.
        [Then("a success confirmation message {string} should be displayed")]
        public void ThenASuccessConfirmationMessageShouldBeDisplayed(string message)
        {

            var actualText = _contactPage.GetSuccessMessageText();
            _report.LogInfo($"Expected: '{message}' | Actual: '{actualText}'");
            Assert.That(actualText, Does.Contain(message),
                $"Success message mismatch. Expected to contain: '{message}', Actual: '{actualText}'");
        }


        // Asserts that the success message contains the expected substring.
        // An alternative step pattern that can be used for partial-match checks.
        [Then(@"the success message should contain ""(.*)""")]
        public void ThenTheSuccessMessageShouldContain(string expectedText)
        {
            var actualText = _contactPage.GetSuccessMessageText();
            _report.LogInfo($"Success message text: '{actualText}' | Expected to contain: '{expectedText}'");
            Assert.That(actualText, Does.Contain(expectedText),
                $"Expected success message to contain '{expectedText}' but got: '{actualText}'");
        }

        // Asserts that the inline validation error for the given field contains the expected message.
        // Retrieves the error text via the page object and compares it with a partial match.
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
