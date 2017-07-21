using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace SQL_Deneme
{
    public class Book
    {

        public string AuthorId;
        public string ISBN;
        public int PageNumber;
        public int Stock;
        public string Name;
        public List<double> Ratings = new List<double>();
        public List<string> Genres = new List<string>();
        //public double type;
        public double Rating;
        public double Price;
        public int Sold;


        public void ReStock(IDbConnection db)
        {
            db.Query<Book>($"Update Books Set Stock ={Stock  +25} Where ISBN = {ISBN}");

            var updated = db.Query<Book>($"Select * From Books Where ISBN = {ISBN}").SingleOrDefault();
            this.Stock = updated.Stock;
            return;
        }


        public void SellBook(IDbConnection db, int amount)
        {
            db.Query<Book>($"Update Books Set Sold ={Sold + amount}, Stock ={Stock - amount} Where ISBN = {ISBN}");

            var updated = db.Query<Book>($"Select * From Books Where ISBN = {ISBN}").SingleOrDefault();
            this.Stock = updated.Stock;
            this.Sold = updated.Sold;
            return;
        }



    }
}
