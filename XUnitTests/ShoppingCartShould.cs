using System;
using Checkout;
using FluentAssertions;
using Moq;
using Xunit;

namespace XUnitTests
{
    public class ShoppingCartShould
    {
        private Mock<IDataRespository> _repositoryMock;
        private readonly CheckoutService _service;
        private IShoppingCart _shoppingCart;


        public ShoppingCartShould()
        {
            _repositoryMock = new Mock<IDataRespository>();

            // Get prices from mock database - Starters £4.40, Mains £7.00
            _repositoryMock.Setup(x => x.GetPriceByFoodCategory(FoodCategoryEnum.Starter)).Returns(4.40m);
            _repositoryMock.Setup(x => x.GetPriceByFoodCategory(FoodCategoryEnum.Main)).Returns(7.00m);

            _shoppingCart = new Checkout.Models.ShoppingCart();
            _service = new CheckoutService(_repositoryMock.Object, _shoppingCart);
        }


        [Fact]
        public void CalculateBillForMultipleAddedItems()
        {
            _service.AddToCart(new MenuItem() { Name = "soup", Category = FoodCategoryEnum.Starter });
            _service.AddToCart(new MenuItem() { Name = "salad", Category = FoodCategoryEnum.Starter });
            _service.AddToCart(new MenuItem() { Name = "steak", Category = FoodCategoryEnum.Main });
            _service.AddToCart(new MenuItem() { Name = "chicken", Category = FoodCategoryEnum.Main });

            _service.CalculateTotalBill().Should().Be(22.80m);
        }

        [Fact]
        public void CalculateBillForSameAddedItems()
        {
            var starter = new MenuItem() { Name = "soup", Category = FoodCategoryEnum.Starter };

            _service.AddToCart(starter);
            _service.AddToCart(starter);
            _service.AddToCart(starter);

            _service.GetTotalItems().Should().Be(3);
            _service.CalculateTotalBill().Should().Be(13.20m);
        }

        [Fact]
        public void CallGetPriceByFoodCategoryWhenAddingMenuItemsToCart()
        {
            _service.AddToCart(new MenuItem() { Name = "soup", Category = FoodCategoryEnum.Starter });
            _service.AddToCart(new MenuItem() { Name = "chicken", Category = FoodCategoryEnum.Main });

            _repositoryMock.Verify(x => x.GetPriceByFoodCategory(It.IsAny<FoodCategoryEnum>()), Times.Exactly(2));
        }

        [Fact]
        public void CalculateCorrectUniqueItemsAdded()
        {
            var starter = new MenuItem() { Name = "salad", Category = FoodCategoryEnum.Starter };
            var main = new MenuItem() { Name = "steak", Category = FoodCategoryEnum.Main };

            _service.AddToCart(starter);
            _service.AddToCart(main);
            _service.AddToCart(main);

           _service.GetUniqueItemsCount().Should().Be(2);
        }


        [Fact]
        public void CalculateCorrectNumberOfItemsAdded()
        {
            _service.AddToCart(new MenuItem() { Name = "soup", Category = FoodCategoryEnum.Starter });
            _service.AddToCart(new MenuItem() { Name = "salad", Category = FoodCategoryEnum.Starter });
            _service.AddToCart(new MenuItem() { Name = "steak", Category = FoodCategoryEnum.Main });

            _service.GetTotalItems().Should().Be(3);

        }

        [Fact]
        public void BeAbleToRemoveItems()
        {
            var starter = new MenuItem() { Name = "salad", Category = FoodCategoryEnum.Starter };

            _service.AddToCart(starter);
            _service.AddToCart(starter);
            _service.RemoveFromCart(starter);

            _service.GetTotalItems().Should().Be(0, because: "all items have been removed from basket");
        }

        [Fact]
        public void IncreaseQuantityAddingSameItem()
        {
            var main = new MenuItem() { Name = "steak", Category = FoodCategoryEnum.Main };

            _service.AddToCart(main);
            _service.AddToCart(main);
            _service.AddToCart(main);

            var item = _service.GetAddedItem(main);

            item.Quantity.Should().Be(3);
        }

        [Fact]
        public void RemoveItemIfQuantityUpdatedToZero()
        {
            var starter = new MenuItem() { Name = "soup", Category = FoodCategoryEnum.Starter };
            var main = new MenuItem() { Name = "steak", Category = FoodCategoryEnum.Main };

            _service.AddToCart(starter);
            _service.AddToCart(main);
            _service.UpdateQuantity(starter, 0);

            _service.GetTotalItems().Should().Be(1);
        }

        [Fact]
        public void BeAbleToUpdateQuantityOfItem()
        {
            var main = new MenuItem() { Name = "steak", Category = FoodCategoryEnum.Main };

            _service.AddToCart(main);
            _service.AddToCart(main);
            _service.AddToCart(main);
            _service.UpdateQuantity(main, 1);

            var item = _service.GetAddedItem(main);

            item.Quantity.Should().Be(1);
            _service.GetTotalItems().Should().Be(1);
        }

        [Fact]
        public void BeEmptyIfNoOrdersPlaced()
        {   
            _service.GetTotalItems().Should().Be(0, because: "no orders have been placed");
        }

        [Fact]
        public void HaveNothingToPayIfEmpty()
        {            
            _service.CalculateTotalBill().Should().Be(0m, because: "there is nothing to pay");
        }

        [Fact]
        public void NotAllowNegativeQuantities()
        {
            var starter = new MenuItem() { Name = "soup", Category = FoodCategoryEnum.Starter };

            _service.AddToCart(new MenuItem() { Name = "soup", Category = FoodCategoryEnum.Starter });
            
            Action act = () => _service.UpdateQuantity(starter, -50);            

            act.Should().Throw<ArgumentOutOfRangeException>()
                        .WithMessage("Please specify a quantity greater than 0 (Parameter 'quantity')");
        }

        [Fact]
        public void NotAllowRemovingItemsNotAdded()
        {
            var starter = new MenuItem() { Name = "soup", Category = FoodCategoryEnum.Starter };

            _service.Invoking(x => x.RemoveFromCart(starter))
                    .Should().Throw<ArgumentException>()
                    .Where(x => x.Message.Contains("Cannot remove an item that is not in cart"));
        }
    }
}