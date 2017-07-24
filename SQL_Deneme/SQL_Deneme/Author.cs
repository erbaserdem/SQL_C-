using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace SQL_Deneme
{
    //[Table(Name = "Authors")]
    public class Author
    {
        //[Column(IsPrimaryKey = true, Name = "ID")]
        public string ID;
       // [Column(Name = "FirstName")]
        public string FirstName;
        //[Column(Name = "LastName")]
        public string LastName;

        public int NumberOfBooks;

        public List<Book> Books = new List<Book>();




        public bool AddAuthor(IDbConnection db)
        {
                db.Query($"INSERT INTO BookStore.dbo.Authors( FirstName, LastName) "
                         + $"Values ('{FirstName}','{LastName}')");

                return true;
        }

        public void UpdateDataBase(IDbConnection db)
        {

            string Query = "UPDATE BookStore.dbo.Authors " +
                           $" SET FirstName = @FirstNam, LastName = @LastName, NumberOfBooks = @NumberOfBooks " +
                           $" WHERE ID = @ID";

            db.Query(Query, new { FirstNam = FirstName, Nam = LastName, LastName = LastName, NumberOfBooks = NumberOfBooks, ID = ID});

        }















    }
}
