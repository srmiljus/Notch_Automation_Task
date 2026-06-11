# Contact Form — Test Automation Suite

End-to-end UI test automation suite for a web contact form, built to demonstrate a clean,
maintainable C# automation framework.

**Stack:** C# · Selenium WebDriver · Reqnroll (BDD) · NUnit · ExtentReports · Bogus

---

## Highlights

- Page Object Model with a shared `BasePage`
- BDD scenarios in Gherkin, bound via Reqnroll step definitions
- Centralized configuration via `appsettings.json` (URL, browser, timeouts, headless mode)
- Rich HTML reporting (ExtentReports) with embedded screenshots per scenario
- Test data generation with Bogus
- GitHub Actions CI: runs on every push/PR, headless Chrome, uploads the HTML report as an artifact
- ~27 scenarios across smoke, regression, happy-path and validation suites

---

## Project Structure

```
src/
└── ContactFormTests/
    ├── Config/
    │   ├── AssemblyInfo.cs          # Assembly-level test configuration
    │   └── ConfigReader.cs          # Reads appsettings.json into typed Settings
    ├── Drivers/
    │   ├── DriverContext.cs         # WebDriver lifecycle wrapper
    │   └── WebDriverFactory.cs      # Creates ChromeDriver with configured options
    ├── Features/
    │   └── ContactForm.feature      # Gherkin scenarios
    ├── Helpers/
    │   ├── ExtentReportHelper.cs    # Singleton Extent Reports wrapper
    │   ├── ScreenshotHelper.cs      # Screenshot capture (file + base64)
    │   └── WaitHelper.cs            # Explicit wait helpers
    ├── Hooks/
    │   └── Hooks.cs                 # BeforeScenario / AfterScenario / AfterStep hooks
    ├── Pages/
    │   ├── BasePage.cs              # Base page with shared actions and waits
    │   └── ContactFormPage.cs       # Page Object Model for the contact form
    ├── StepDefinitions/
    │   └── ContactFormSteps.cs      # Reqnroll step bindings
    ├── TestData/
    │   └── test-document.pdf        # Sample file for upload scenarios
    ├── appsettings.json             # Test configuration
    └── ContactFormTests.csproj
```

---

## Prerequisites

| Tool          | Version       | Notes                                       |
| ------------- | ------------- | ------------------------------------------- |
| .NET SDK      | 8.0+          | https://dotnet.microsoft.com/download       |
| Google Chrome | Latest stable | ChromeDriver is managed via NuGet           |
| Git           | Any           | For cloning the repo                        |

---

## Getting Started

```bash
# 1. Clone
git clone <repository-url>
cd src/ContactFormTests

# 2. Restore
dotnet restore

# 3. Build
dotnet build

# 4. Run all tests
dotnet test
```

Configure the target URL, browser and timeouts in `appsettings.json`. Set `"Headless": true`
to run without a visible browser (used in CI).

---

## Running Specific Suites

Scenarios are tagged; filter with NUnit category format:

```bash
dotnet test --filter "Category=smoke"
dotnet test --filter "Category=regression"
dotnet test --filter "Category=happy-path"
dotnet test --filter "Category=validation"
```

---

## Test Scenarios Overview

| Tag           | Count           | Description                          |
| ------------- | --------------- | ------------------------------------ |
| `@smoke`      | 1               | Critical path — quick sanity check   |
| `@regression` | 17              | Full regression suite                |
| `@happy-path` | 3 + 11 outlines | Successful form submissions          |
| `@validation` | 9 + 4 outlines  | Required field and format validation |

---

## Reporting

After each run an HTML report is generated under `bin/Debug/net8.0/Reports/`, including:

- Pass/fail status per scenario
- Step-by-step execution log
- Embedded screenshots (final state for every scenario, and at the failing step)
- Run metadata (browser, URL, framework)

Screenshots are also saved under `bin/Debug/net8.0/Screenshots/`.

---

## CI/CD

A GitHub Actions workflow (`.github/workflows/tests.yml`) runs on every push and PR to `main`:
installs Chrome, runs in headless mode, and uploads the generated HTML report as a build artifact.
It can also be triggered manually from the **Actions** tab with a tag filter
(`all`, `smoke`, `regression`, `happy-path`, `validation`).

---

## Maintaining Locators

All locators live in `Pages/ContactFormPage.cs`. If the form's DOM changes, update the
`By` selectors there only — step definitions and feature files stay untouched.

---

## Notes & Limitations

| Item                       | Notes                                                              |
| -------------------------- | ------------------------------------------------------------------ |
| Real form submissions      | Happy-path scenarios submit a real request to the target form      |
| ChromeDriver compatibility | Managed automatically via the `Selenium.WebDriver` NuGet package   |
| Cookie consent banner      | Hooks dismiss it automatically; selector may need updating         |
| Rate limiting / CAPTCHA    | If the target adds CAPTCHA, happy-path scenarios will need rework  |
