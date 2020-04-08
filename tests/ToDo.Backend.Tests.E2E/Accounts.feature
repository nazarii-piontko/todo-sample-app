Feature: Accounts
  Background:
    Given service is running
    And web browser window with size 800x600
    
  Scenario: As a user I should see correct Register page
    When I open page at /register
    Then I should see correct register page
    
  Scenario: As a user I should be able to register
    Given opened page at /register
    When I input 'user@company.com' into 'Email'
    And input 'Qwerty_1234' into 'Password'
    And click button 'Register'
    Then page should be redirected to /login within 5 sec