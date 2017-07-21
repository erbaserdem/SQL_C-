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

        public string Genre;
        //public double type;
        public float Rating;
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

        public bool AddBook(IDbConnection db, List<Book> booksFromSql)
        {
            if (Program.SelectWithIdAuthor(AuthorId ,db)!=null)// if author does not exist do not add
            {
                db.Query($"INSERT INTO BookStore.dbo.Books(AuthorId, Name, PageNumber, Stock, Price) "
                         + $"Values ('{AuthorId}','{Name}','{PageNumber}','{Stock}','{Price}')");
                booksFromSql = Program.GetAllBooks(db);
                return true;
            }
            else
            {
                Console.WriteLine("Author Does not Exist");
                return false;
            }
        }

        public bool DeleteBook(IDbConnection db, List<Book> booksFromSql)
        {
            var toBeDeleted = Program.SelectWithIsbnBook(ISBN, db);
            if (toBeDeleted != null) // if author does not exist do not add
            {
                db.Query($"Delete From Books Where ISBN = {ISBN}");
                booksFromSql = Program.GetAllBooks(db);
                return true;
            }
            else
            {

                return false;
            }
        }

        public void UpdateDataBase(IDbConnection db)
        {

            string Query = "UPDATE BookStore.dbo.Books " +
                     $" SET AuthorId = @AuthorId,Price = @Price, Name = @Nam, PageNumber = @PageNumbe, Rating = @Ratin, Sold = @Sol, Genre =@Genre  " +
                     $" WHERE ISBN = @ISBN";



            db.Query(Query, new {AuthorId = AuthorId, Nam = Name, Ratin = Rating, Genre = Genre, PageNumbe = PageNumber, ISBN = ISBN , Price = Price, Sol = Sold});





        }


    }
}
