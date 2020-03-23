Feature: Accounts
  Background:
    Given service is started and running
  
  Scenario: Register
    When I send POST api/v1.0/account/register:
    
    