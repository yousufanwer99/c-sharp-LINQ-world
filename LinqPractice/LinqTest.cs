using System.Security.Cryptography.X509Certificates;

namespace LinqPractice
{
    public class LinqTest
    {


        public void main()
        {

            // Dataset 2: Customers
            var customers = new List<Customer>
            {
                new Customer { CustomerId = 1, Name = "Alice", City = "New York" },
                new Customer { CustomerId = 2, Name = "Bob", City = "Los Angeles" },
                new Customer { CustomerId = 3, Name = "Charlie", City = "Chicago" }
            };

            // Dataset 1: Products
            var products = new List<Product>
            {
                new Product { ProductId = 1, Name = "Laptop", Price = 1200, Category = "Electronics" },
                new Product { ProductId = 2, Name = "Smartphone", Price = 800, Category = "Electronics" },
                new Product { ProductId = 3, Name = "Headphones", Price = 150, Category = "Accessories" },
                new Product { ProductId = 4, Name = "Desk", Price = 300, Category = "Furniture" },
                new Product { ProductId = 5, Name = "Chair", Price = 200, Category = "Furniture" }
            };

            // Dataset 3: Orders
            var orders = new List<Order>
            {
                new Order { OrderId = 1, CustomerId = 1, ProductId = 1, Quantity = 1 },
                new Order { OrderId = 2, CustomerId = 2, ProductId = 3, Quantity = 2 },
                new Order { OrderId = 3, CustomerId = 1, ProductId = 5, Quantity = 4 },
                new Order { OrderId = 4, CustomerId = 3, ProductId = 2, Quantity = 1 },
                new Order { OrderId = 5, CustomerId = 3, ProductId = 4, Quantity = 1 }
            };


            var q1 = orders
                .Join
                (
                    products,
                    o => o.ProductId,
                    p => p.ProductId,
                    (o, p) => new { p.Name, o.Quantity, price = (o.Quantity * p.Price) }
                );


            var q2 = orders
                    .Join
                    (
                        products,
                        o => o.ProductId,
                        p => p.ProductId,
                        (o, p) => new { p.Category, o.Quantity }
                    )
                    .GroupBy(tbl => tbl.Category)
                    .Select
                    (
                        x => new
                        {
                            x.Key,
                            _orderCount = x.Sum(k => k.Quantity)
                        }
                    );
            foreach (var item in q2)
            {
                //Console.WriteLine($"{item.Key}-{item._orderCount}");
            }

            var q3 = orders
                .Join
                (
                    customers,
                    o => o.CustomerId,
                    c => c.CustomerId,
                    (o, c) => new { o.OrderId, o.ProductId, c.CustomerId, c.City }
                )
                .Where(x => x.City == "New York")
                .Join
                (
                    products,
                    tblX => tblX.ProductId,
                    p => p.ProductId,
                    (tblX, p) => new { p.Name }

                );
            foreach (var item in q3)
            {
                //Console.WriteLine(item.Name);
            }

            var q4 = orders
                .Join
                (
                    products,
                    o => o.ProductId,
                    p => p.ProductId,
                    (o, p) => new { o.CustomerId, _spentAmount = (o.Quantity * p.Price) }
                )
                .GroupBy(x => x.CustomerId)
                .Select
                (
                    x => new { _customerId = x.Key, _totalSpent = x.Sum(l => l._spentAmount) }
                )
                .Join
                (
                    customers,
                    tblX => tblX._customerId,
                    c => c.CustomerId,
                    (tblX, c) => new { c.Name, tblX._totalSpent }
                );
            foreach (var item in q4)
            {
                //Console.WriteLine($"{item.Name}-{item._totalSpent}");
            }

        }
    }
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Category { get; set; }
    }

    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
    }

    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
