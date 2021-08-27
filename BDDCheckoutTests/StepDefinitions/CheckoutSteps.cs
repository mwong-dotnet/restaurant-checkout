using Checkout;
using Checkout.Models;
using FluentAssertions;
using Moq;
using System;
using TechTalk.SpecFlow;

namespace BDDCheckoutTests.StepDefinitions
{
    [Binding]
    public class CheckoutSteps
    {
        private readonly ScenarioContext _scenarioContext;

        private Mock<IDataRespository> _repositoryMock;
        private CheckoutService _service;
        private IShoppingCart _shoppingCart;


        public CheckoutSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;

            _repositoryMock = new Mock<IDataRespository>();

            // Get prices from mock database - Starters £4.40, Mains £7.00
            _repositoryMock.Setup(x => x.GetPriceByFoodCategory(FoodCategoryEnum.Starter)).Returns(4.40m);
            _repositoryMock.Setup(x => x.GetPriceByFoodCategory(FoodCategoryEnum.Main)).Returns(7.00m);

            _shoppingCart = new ShoppingCart();
            _service = new CheckoutService(_repositoryMock.Object, _shoppingCart);
        }


        [Given(@"I have an empty cart")]
        public void GivenIHaveAnEmptyCart()
        {
            
        }

        [Then(@"the total price to pay is (.*)")]
        public void ThenTheTotalPriceToPayIs(decimal amount)
        {
            _service.CalculateTotalBill().Should().Be(amount, because: "there is nothing to pay");
        }


        [Given(@"I have added a (.*) named '(.*)'")]               
        public void GivenIHaveAStarterNamed(FoodCategoryEnum category, string foodName)
        {
               var foodItem = new MenuItem() { Name = foodName, Category = category };
               _service.AddToCart(foodItem);                
        }
     

        [When(@"I update the quantity of '(.*)' to (.*)")]
        public void WhenIUpdateTheQuantityOfTo(string foodName, int quantity)
        {
            var orderline = _service.GetAddedItem(foodName);
            _service.UpdateQuantity(orderline.MenuItem, quantity);
        }

        [Then(@"the total number of items should be (.*)")]
        public void ThenTheTotalNumberOfItemsShouldBe(int quantity)
        {
            _service.GetTotalItems().Should().Be(quantity);
        }
    }
}
