using OpenQA.Selenium;
using NotchContactFormTests.Config;
using NotchContactFormTests.Helpers;

namespace NotchContactFormTests.Pages
{
    public class ContactFormPage : BasePage
    {
        private readonly string _pageUrl;

        #region Locators

        private By FirstNameInput => By.CssSelector("input[name='input_5']");
        private By LastNameInput => By.CssSelector("input[name='input_18']");
        private By EmailInput => By.CssSelector("input[name='input_17']");
        private By PhoneInput => By.CssSelector("input[name='input_8']");
        private By HowDidYouHearDropdown => By.CssSelector("#input_7_9_chosen");
        private By CompanyInput => By.CssSelector("input[name='input_11']");
        private By BudgetDropdown => By.CssSelector("div.chosen-container.chosen-container-single:not(#input_7_9_chosen)");
        private By ProjectDetailsTextarea => By.CssSelector("textarea[name='input_15']");
        private By ServiceCheckboxByLabel(string label) => By.XPath($"//div[@class='gfield_checkbox ']//label[contains(.,'{label}')]");
        private By ConsentCheckbox => By.CssSelector("input[name='input_16.1']");
        private By SubmitButton => By.CssSelector("input[type='submit'], button[type='submit']");
        private By SuccessMessage => By.XPath("//div[@class='contact-form']//div[@id='gform_confirmation_message_7']");
        private By FirstNameError => By.XPath("(//*[contains(@id,'validation_message_7_5')])[1]");
        private By LastNameError => By.XPath("(//*[contains(@id,'validation_message_7_18')])[1]");
        private By EmailError => By.XPath("(//*[contains(@id,'validation_message_7_17')])[1]");
        private By ConsentError => By.XPath("(//*[contains(@id,'validation_message_7_16')])[1]");
        private By FileUploadInput => By.CssSelector("input[type='file']");
        private By FileUploadProgress => By.CssSelector(".gfield_fileupload_percent");

        #endregion 


        public ContactFormPage(IWebDriver driver, WaitHelper waitHelper, string baseUrl)
            : base(driver, waitHelper)
        {
            _pageUrl = baseUrl;
        }

        #region Actions

        public void NavigateTo() => GoToUrl(_pageUrl);
        public void EnterFirstName(string value) => TypeInto(FirstNameInput, value);
        public void EnterLastName(string value) => TypeInto(LastNameInput, value);
        public void EnterEmail(string value) => TypeInto(EmailInput, value);
        public void EnterPhoneNumber(string value) => TypeInto(PhoneInput, value);
        public void EnterCompany(string value) => TypeInto(CompanyInput, value);
        public void EnterProjectDetails(string value) => TypeInto(ProjectDetailsTextarea, value);

        public void SelectHowDidYouHear(string option) =>
            SelectFromChosenDropdown(HowDidYouHearDropdown, option);

        public void SelectBudget(string option) =>
            SelectFromChosenDropdown(BudgetDropdown, option);

        public void SelectService(string serviceLabel) =>
            ClickOn(ServiceCheckboxByLabel(serviceLabel));

        public void AcceptConsent() => EnsureChecked(ConsentCheckbox);
        public void ClickSendMessage() => ClickOn(SubmitButton);

        public void UploadFile(string filePath)
        {
            var inputs = Driver.FindElements(FileUploadInput);
            foreach (var input in inputs)
            {
                try
                {
                    ((IJavaScriptExecutor)Driver).ExecuteScript(
                        "arguments[0].style.display='block'; arguments[0].style.visibility='visible';", input);
                    input.SendKeys(filePath);
                    Wait.WaitForCondition(
                        d => d.FindElement(FileUploadProgress).Text.Trim() == "100%",
                        timeoutSeconds: 30);

                    Wait.WaitForElementNotVisible(FileUploadProgress, timeoutSeconds: 10);
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"File upload attempt failed: {ex.Message}");
                }
            }
            throw new InvalidOperationException($"Failed to upload file: {filePath}. No valid file input found.");
        }

        public string GetSuccessMessageText() =>
            GetText(SuccessMessage, timeoutSeconds: ConfigReader.PageLoadTimeoutSeconds);

        public string GetValidationErrorText(string fieldName)
        {
            var locator = fieldName.ToLower() switch
            {
                "first name" => FirstNameError,
                "last name" => LastNameError,
                "email" => EmailError,
                "consent" => ConsentError,
                _ => throw new ArgumentException($"Unknown field name: '{fieldName}'")
            };

            var text = GetText(locator, timeoutSeconds: 8);

            if (string.IsNullOrEmpty(text))
                throw new NoSuchElementException(
                    $"Validation error for '{fieldName}' not found or empty after 8s.");

            return text;
        }

        #endregion
    }
}