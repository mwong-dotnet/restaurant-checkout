Feature: Demo restaurant checkout
		 In order dine in a restaurant
		 As a customer
		 I want be able to purchase food 


Scenario: Empty cart nothing to pay
    Given I have an empty cart
	Then the total price to pay is 0


Scenario: Update quantity of item to 0 should remove from cart
	Given I have added a Starter named 'soup'
	And I have added a Starter named 'soup'
	When I update the quantity of 'soup' to 0
	Then the total number of items should be 0


