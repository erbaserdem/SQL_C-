using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace SQL_Deneme
{
    public class Item
    {
        public int Amount { get; set; }
        public Book OrderedBook;
        public string BookId;
        public string ItemId;
        public bool Competed;
        public string OrderNo;
        public Order Order;
        public double Cost;



        public void AddItem(IDbConnection db)
        {
            if (Program.SelectWithIsbnBook(BookId, db) != null) // if author does not exist do not add
            {
                db.Query($"INSERT INTO BookStore.dbo.Item(BookId, Amount, OrderNo, Cost) "
                         + $"Values ('{BookId}','{Amount}','{OrderNo}','{Cost}')");

                return;
            }
            else
            {
                Console.WriteLine("Book Does Not Exist");
            }
        }



        public void UpdateDataBase(IDbConnection db)
        {

            string Query = "UPDATE BookStore.dbo.Item " +
                           $" SET BookId = @BookId, Amount = @Amount " +
                           $" WHERE ItemId = @ItemId";

            db.Query(Query, new { BookId = BookId, Amount = Amount, ItemId = ItemId });

        }







    }
}
