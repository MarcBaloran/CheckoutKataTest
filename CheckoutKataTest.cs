using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CheckoutKata.Test
{
    public class CheckoutKataTest
    {
        public class CheckoutPricing
        {
            private Checkout subject;

            public CheckoutPricing()
            {
                subject = new Checkout(
                    new ProductPricing("A", 10),
                    new ProductPricing("B", 15, 3, 5),
                    new ProductPricing("C", 40),
                    new ProductPricing("D", 55, 2, 27.5));
            }

            [Theory]
            [InlineData("A", 10)]
            [InlineData("B", 15)]
            [InlineData("C", 40)]
            [InlineData("D", 55)]
            public void Add_WhenValidProductIsAdded_ReturnsExpectedTotalPrice(string sku, int expected)
            {
                subject.Add(sku);

                Assert.Equal(expected, subject.GetTotalPrice());
            }

            [Fact]
            public void Add_WhenThreeProductBIsAdded_TotalPriceIs40()
            {
                subject.Add("B");
                subject.Add("B");
                subject.Add("B");

                Assert.Equal(40, subject.GetTotalPrice());
            }

            [Fact]
            public void Add_WhenTwoProductDIsAdded_TotalPriceIs82point5()
            {
                subject.Add("D");
                subject.Add("D");
                var totalPrice = subject.GetTotalPrice();
                Assert.Equal(82.5, subject.GetTotalPrice());
            }

            [Fact]
            public void Add_WhenInvalidProductIsAdded_TotalPriceIs0()
            {
                subject.Add("Z");

                Assert.Equal(0, subject.GetTotalPrice());
            }
        }

        class Checkout
        {
            private double total;
            private IDictionary<string, int> productCount = new Dictionary<string, int>();
            private IEnumerable<ProductPricing> pricing;

            public Checkout(params ProductPricing[] pricing)
            {
                this.pricing = pricing;
            }

            public void Add(string product)
            {
                var productCount = RegisterProduct(product);

                var productPricing = pricing
                    .Where(x => x.IsFor(product))
                    .FirstOrDefault();

                if (productPricing == null) return;

                total += productPricing.GetTotal(productCount);
            }

            private int RegisterProduct(string product)
            {
                if (!productCount.ContainsKey(product))
                    productCount[product] = 0;

                return ++productCount[product];
            }

            public double GetTotalPrice()
            {
                return total;
            }
        }

        class ProductPricing
        {
            private string product;
            private int price;
            private int discountThreshold;
            private double discountAmount;

            public ProductPricing(string product, int price)
            {
                this.product = product;
                this.price = price;
            }

            public ProductPricing(string product, int price, int discountThreshold, double discountAmount)
            {
                this.product = product;
                this.price = price;
                this.discountThreshold = discountThreshold;
                this.discountAmount = discountAmount;
            }

            public bool IsFor(string product)
            {
                return product == this.product;
            }

            public double GetTotal(int quantity)
            {
                if (discountThreshold != 0)
                    if (quantity % discountThreshold == 0)
                        return (double)(price - discountAmount);

                return price;
            }
        }
    }
}
