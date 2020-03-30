Feature: Landing page
  Background: 
    Given service is running
    And web browser window with size 800x600
  
  Scenario: Open landing page
    When I open page at /
    Then I should see correct landing page within 5 sec
    