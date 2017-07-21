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

        public List<Book> Books = new List<Book>();

        public bool AddAuthor(IDbConnection db)
        {
                db.Query($"INSERT INTO BookStore.dbo.Authors( FirstName, LastName) "
                         + $"Values ('{FirstName}','{LastName}')");

                return true;
        }















    }
}
