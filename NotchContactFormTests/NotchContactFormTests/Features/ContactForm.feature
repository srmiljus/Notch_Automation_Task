Feature: Contact Form Submission
  As a potential client
  I want to fill out the contact form on the Notch website
  So that I can get in touch with the Notch team about my project

Background:
	Given I navigate to the contact form page


  # HAPPY PATH

@smoke @regression @happy-path
Scenario: Successfully submit form with only required fields
	When I enter "John" in the first name field
	And I enter "Doe" in the last name field
	And I enter a generated email in the email field
	And I accept the privacy policy consent
	And I click the Send message button
	Then a success confirmation message 'Thanks for contacting us! We will get in touch with you shortly.' should be displayed

@regression @happy-path
Scenario: Successfully submit form with all fields filled
	When I enter "Jane" in the first name field
	And I enter "Smith" in the last name field
	And I enter a generated email in the email field
	And I enter "+381601234567" in the phone number field
	And I select "LinkedIn" from the how did you hear about us dropdown
	And I enter "Test Company d.o.o." in the company field
	And I select "€50.000 to €100.000" from the budget dropdown
	And I select the service "Custom Software Development"
	And I select the service "UX/UI Design"
	And I enter "We need a custom web application for internal process management." in the project details field
	And I upload the file "test-document.pdf"
	And I accept the privacy policy consent
	And I click the Send message button
	Then a success confirmation message 'Thanks for contacting us! We will get in touch with you shortly.' should be displayed

@regression @happy-path
Scenario: Successfully submit form with multiple services selected
	When I enter "Multi" in the first name field
	And I enter "Service" in the last name field
	And I enter a generated email in the email field
	And I select the service "Custom Software Development"
	And I select the service "Product Discovery"
	And I select the service "UX/UI Design"
	And I select the service "Team Extension"
	And I accept the privacy policy consent
	And I click the Send message button
	Then a success confirmation message 'Thanks for contacting us! We will get in touch with you shortly.' should be displayed

@regression @happy-path @referral-sources
Scenario Outline: Submit form with different referral sources
	When I enter "Test" in the first name field
	And I enter "User" in the last name field
	And I enter a generated email in the email field
	And I select "<source>" from the how did you hear about us dropdown
	And I accept the privacy policy consent
	And I click the Send message button
	Then a success confirmation message 'Thanks for contacting us! We will get in touch with you shortly.' should be displayed

Examples:
	| source         |
	| Recommendation |
	| Google         |
	| Clutch         |
	| LinkedIn       |
	| Facebook       |
	| Instagram      |

@regression @happy-path @budget-selection
Scenario Outline: Submit form with different budget selections
	When I enter "Budget" in the first name field
	And I enter "Tester" in the last name field
	And I enter a generated email in the email field
	And I select "<budget>" from the budget dropdown
	And I accept the privacy policy consent
	And I click the Send message button
	Then a success confirmation message 'Thanks for contacting us! We will get in touch with you shortly.' should be displayed

Examples:
	| budget               |
	| Up to €50.000        |
	| €50.000 to €100.000  |
	| €100.000 to €250.000 |
	| Over €250.000        |
	| Can't disclose       |



  # VALIDATION — REQUIRED FIELDS

@regression @validation @required-fields
Scenario: Show validation errors when submitting completely empty form
	When I click the Send message button
	Then validation error message "This field is required." should be displayed for "first name" field
	And validation error message "This field is required." should be displayed for "last name" field
	And validation error message "This field is required." should be displayed for "email" field
	And validation error message "This field is required." should be displayed for "consent" field

@regression @validation @required-fields
Scenario: Show validation error when first name is missing
	When I enter "Doe" in the last name field
	And I enter a generated email in the email field
	And I accept the privacy policy consent
	And I click the Send message button
	Then validation error message "This field is required." should be displayed for "first name" field

@regression @validation @required-fields
Scenario: Show validation error when last name is missing
	When I enter "John" in the first name field
	And I enter a generated email in the email field
	And I accept the privacy policy consent
	And I click the Send message button
	Then validation error message "This field is required." should be displayed for "last name" field

@regression @validation @required-fields
Scenario: Show validation error when email is missing
	When I enter "John" in the first name field
	And I enter "Doe" in the last name field
	And I accept the privacy policy consent
	And I click the Send message button
	Then validation error message "This field is required." should be displayed for "email" field

@regression @validation @required-fields
Scenario: Show validation error when consent is not accepted
	When I enter "John" in the first name field
	And I enter "Doe" in the last name field
	And I enter a generated email in the email field
	And I click the Send message button
	Then validation error message "This field is required." should be displayed for "consent" field


  # VALIDATION — EMAIL FORMAT

@regression @validation @email-format
Scenario Outline: Show validation error for invalid email formats
	When I enter "John" in the first name field
	And I enter "Doe" in the last name field
	And I enter "<invalid_email>" in the email field
	And I accept the privacy policy consent
	And I click the Send message button
	Then validation error message "The email address entered is invalid, please check the formatting (e.g. email@domain.com)." should be displayed for "email" field

Examples:
	| invalid_email       |
	| plaintext           |
	| missing@domain      |
	| @nodomain.com       |
	| double@@example.com |



@intentional-fail
Scenario: [INTENTIONAL FAIL] Verify incorrect success message text after submission
	When I enter "John" in the first name field
	And I enter "Doe" in the last name field
	And I enter a generated email in the email field
	And I accept the privacy policy consent
	And I click the Send message button
	Then the success message should contain "Your submission has been received and processed successfully"
