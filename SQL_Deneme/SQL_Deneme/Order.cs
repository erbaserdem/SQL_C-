using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace SQL_Deneme
{
    public class Order
    {
        public string CustomerId;
        public string OrderNo { get; set; }
        public List<Item> OrderedItems = new List<Item>();
        public bool Processed;
        public double Cost;
        public double Discount;
        public Customer Customer;
        public DateTime DateOfOrder;



        public void AddOrder(IDbConnection db)
        {
            db.Query($"INSERT INTO BookStore.dbo.Order(CustomerId, Cost, Discount, DateOfOrder) "
                     + $"Values ('{CustomerId}','{Cost}','{Discount}','{DateOfOrder}')");
            return;
        }


        public void UpdateDataBase(IDbConnection db)
        {

            string Query = "UPDATE BookStore.dbo.Orders " +
                           $" SET CustomerId = @CustomerId, Cost = @Cost, Discount = @Discount, DateOfOrder = @DateOfOrder, Processed = @Processed" +
                           $" WHERE OrderNo = @OrderNo";

            db.Query(Query, new { CustomerId = CustomerId, Cost = Cost, Discount = Discount, DateOfOrder = DateOfOrder, Processed= Processed, OrderNo = OrderNo });

        }







    }

}
