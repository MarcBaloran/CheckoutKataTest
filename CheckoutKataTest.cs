using System;
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
                new ProductPricing("A", 50, 3, 20),
                new ProductPricing("B", 30, 2, 15),
                new ProductPricing("C", 20),
                new ProductPricing("D", 15));
        }

        [Fact]
        public void price_for_1_product_A_is_50()
        {
            subject.Scan("A");

            Assert.Equal(50, subject.GetTotalPrice());
        }

        [Fact]
        public void price_for_2_product_A_is_100()
        {
            subject.Scan("A");
            subject.Scan("A");

            Assert.Equal(100, subject.GetTotalPrice());
        }

        [Fact]
        public void price_for_3_product_A_is_130()
        {
            subject.Scan("A");
            subject.Scan("A");
            subject.Scan("A");

            Assert.Equal(130, subject.GetTotalPrice());
        }

        [Fact]
        public void price_for_6_product_A_is_260()
        {
            subject.Scan("A");
            subject.Scan("A");
            subject.Scan("A");
            subject.Scan("A");
            subject.Scan("A");
            subject.Scan("A");

            Assert.Equal(260, subject.GetTotalPrice());
        }

        [Fact]
        public void price_for_1_product_B_is_30()
        {
            subject.Scan("B");

            Assert.Equal(30, subject.GetTotalPrice());
        }

        [Fact]
        public void price_for_2_product_B_is_45()
        {
            subject.Scan("B");
            subject.Scan("B");

            Assert.Equal(45, subject.GetTotalPrice());
        }

        [Fact]
        public void price_for_3_product_B_is_75()
        {
            subject.Scan("B");
            subject.Scan("B");
            subject.Scan("B");

            Assert.Equal(75, subject.GetTotalPrice());
        }

        [Fact]
        public void price_for_4_product_B_is_90()
        {
            subject.Scan("B");
            subject.Scan("B");
            subject.Scan("B");
            subject.Scan("B");

            Assert.Equal(90, subject.GetTotalPrice());
        }

        [Fact]
        public void price_for_1_product_C_is_20()
        {
            subject.Scan("C");

            Assert.Equal(20, subject.GetTotalPrice());
        }

        [Fact]
        public void price_for_1_product_D_is_15()
        {
            subject.Scan("D");

            Assert.Equal(15, subject.GetTotalPrice());
        }

        [Fact]
        public void can_use_independent_pricing()
        {
            subject.Scan("A");

            Assert.Equal(50, subject.GetTotalPrice());
        }

        [Fact]
        public void does_not_include_unknown_products()
        {
            subject.Scan("Z");

            Assert.Equal(0, subject.GetTotalPrice());
        }
    }

    class Checkout
    {
        private int total;
        private IDictionary<string, int> productCount = new Dictionary<string, int>();
        private IEnumerable<ProductPricing> pricing;

        public Checkout(params ProductPricing[] pricing)
        {
            this.pricing = pricing;
        }

        public void Scan(string product)
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

        public int GetTotalPrice()
        {
            return total;
        }
    }

    class ProductPricing
    {
        private string product;
        private int price;
        private int discountThreshold;
        private int discountAmount;

        public ProductPricing(string product, int price)
        {
            this.product = product;
            this.price = price;
        }

        public ProductPricing(string product, int price, int discountThreshold, int discountAmount)
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

        public int GetTotal(int quantity)
        {
            if (discountThreshold != 0)
                if (quantity % discountThreshold == 0)
                    return price - discountAmount;

            return price;
        }
    }
    }
}
