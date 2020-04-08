Feature: Landing page
  
  Background: 
    Given service is running
    And web browser window with size 800x600
    
  Scenario: As a user I should see correct Landing page
    When I open page at /
    Then I should see correct landing page
    