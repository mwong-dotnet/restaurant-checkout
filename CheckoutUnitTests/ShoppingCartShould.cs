using Checkout;
using Moq;
using NUnit.Framework;
using System;

namespace CheckoutUnitTests
{
    public class ShoppingCartShould
    {
        private Mock<IDataRespository> _repositoryMock;
        private CheckoutService _service;
        private IShoppingCart _shoppingCart;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IDataRespository>();

            // Get prices from mock database - Starters £4.40, Mains £7.00
            _repositoryMock.Setup(x => x.GetPriceByFoodCategory(FoodCategoryEnum.Starter)).Returns(4.40m);
            _repositoryMock.Setup(x => x.GetPriceByFoodCategory(FoodCategoryEnum.Main)).Returns(7.00m);

            _shoppingCart = new Checkout.Models.ShoppingCart();
            _service = new CheckoutService(_repositoryMock.Object, _shoppingCart);            
        }


        [Test]
        public void CalculateBillForMultipleAddedItems()
        {
            _service.AddToCart(new MenuItem() { Name = "soup", Category = FoodCategoryEnum.Starter });
            _service.AddToCart(new MenuItem() { Name = "salad", Category = FoodCategoryEnum.Starter });
            _service.AddToCart(new MenuItem() { Name = "steak", Category = FoodCategoryEnum.Main });
            _service.AddToCart(new MenuItem() { Name = "chicken", Category = FoodCategoryEnum.Main });

            Assert.That(_service.CalculateTotalBill(), Is.EqualTo(22.80m));
        }

        [Test]
        public void CalculateBillForSameAddedItems()
        {
            var starter = new MenuItem() { Name = "soup", Category = FoodCategoryEnum.Starter };

            _service.AddToCart(starter);
            _service.AddToCart(starter);
            _service.AddToCart(starter);

            Assert.That(_service.GetTotalItems(), Is.EqualTo(3));
            Assert.That(_service.CalculateTotalBill(), Is.EqualTo(13.20m));
        }

        [Test]
        public void GetPriceOfMenuItemsWhenAddingToCart()
        {
            _service.AddToCart(new MenuItem() { Name = "soup", Category = FoodCategoryEnum.Starter });
            _service.AddToCart(new MenuItem() { Name = "chicken", Category = FoodCategoryEnum.Main });

            _repositoryMock.Verify(x => x.GetPriceByFoodCategory(It.IsAny<FoodCategoryEnum>()), Times.Exactly(2));
        }

        [Test]
        public void ShouldCalculateCorrectUniqueItemsAdded()
        {
            var starter = new MenuItem() { Name = "salad", Category = FoodCategoryEnum.Starter };
            var main = new MenuItem() { Name = "steak", Category = FoodCategoryEnum.Main };

            _service.AddToCart(starter);
            _service.AddToCart(main);
            _service.AddToCart(main);

            Assert.That(_service.GetUniqueItemsCount(), Is.EqualTo(2));
        }


        [Test]
        public void ShouldCalculateCorrectNumberOfItemsAdded()
        {
            _service.AddToCart(new MenuItem() { Name = "soup", Category = FoodCategoryEnum.Starter });
            _service.AddToCart(new MenuItem() { Name = "salad", Category = FoodCategoryEnum.Starter });
            _service.AddToCart(new MenuItem() { Name = "steak", Category = FoodCategoryEnum.Main });

            Assert.That(_service.GetTotalItems(), Is.EqualTo(3));
        }

        [Test]
        public void ShouldBeAbleToRemoveItems()
        {
            var starter = new MenuItem() { Name = "salad", Category = FoodCategoryEnum.Starter };

            _service.AddToCart(starter);
            _service.AddToCart(starter);
            _service.RemoveFromCart(starter);

            Assert.That(_service.GetTotalItems(), Is.EqualTo(0));
        }

        [Test]
        public void ShouldIncreaseQuantityAddingSameItem()
        {
            var main = new MenuItem() { Name = "steak", Category = FoodCategoryEnum.Main };

            _service.AddToCart(main);
            _service.AddToCart(main);
            _service.AddToCart(main);

            var item = _service.GetAddedItem(main);
            Assert.That(item.Quantity, Is.EqualTo(3));
        }

        [Test]
        public void ShouldRemoveItemIfQuantityUpdatedToZero()
        {
            var starter = new MenuItem() { Name = "soup", Category = FoodCategoryEnum.Starter };
            var main = new MenuItem() { Name = "steak", Category = FoodCategoryEnum.Main };

            _service.AddToCart(starter);
            _service.AddToCart(main);
            _service.UpdateQuantity(starter, 0);

            Assert.That(_service.GetTotalItems(), Is.EqualTo(1));
        }

        [Test]
        public void ShouldBeAbleToUpdateQuantityOfItem()
        {
            var main = new MenuItem() { Name = "steak", Category = FoodCategoryEnum.Main };

            _service.AddToCart(main);
            _service.AddToCart(main);
            _service.AddToCart(main);
            _service.UpdateQuantity(main, 1);

            var item = _service.GetAddedItem(main);
            Assert.That(item.Quantity, Is.EqualTo(1));
            Assert.That(_service.GetTotalItems(), Is.EqualTo(1));
        }

        [Test]
        public void BeEmptyIfNoOrdersPlaced()
        {
            Assert.That(_service.GetTotalItems(), Is.EqualTo(0));
        }

        [Test]
        public void ShouldHaveNothingToPayIfEmpty()
        {
            Assert.That(_service.CalculateTotalBill(), Is.EqualTo(0m));
        }

        [Test]
        public void NotAllowNegativeQuantities()
        {
            var starter = new MenuItem() { Name = "soup", Category = FoodCategoryEnum.Starter };

            _service.AddToCart(new MenuItem() { Name = "soup", Category = FoodCategoryEnum.Starter });

            Assert.That(() => _service.UpdateQuantity(starter, -50), Throws.TypeOf<ArgumentOutOfRangeException>()
                        .With
                        .Message
                        .EqualTo("Please specify a quantity greater than 0 (Parameter 'quantity')"));
        }

        [Test]
        public void NotAllowRemovingItemsNotAdded()
        {
            var starter = new MenuItem() { Name = "soup", Category = FoodCategoryEnum.Starter };

            Assert.That(() => _service.RemoveFromCart(starter), Throws.TypeOf<ArgumentException>()
                        .With
                        .Message
                        .EqualTo("Cannot remove an item that is not in cart"));
        }
    }
}