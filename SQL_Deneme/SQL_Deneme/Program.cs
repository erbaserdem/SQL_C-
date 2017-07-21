using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace SQL_Deneme
{
    class Program
    {

        public static List<Book> SomeMethod(string fName, SqlConnection conn)
        {
            List<Book> booksToAdd = new List<Book>();

            string oString = "Select * from Books";
            SqlCommand oCmd = new SqlCommand(oString, conn);
            using (SqlDataReader oReader = oCmd.ExecuteReader())
            {
                while (oReader.Read())
                {
                    Book bookToAdd = new Book
                    {
                        AuthorId = oReader["AuthorId"].ToString(),
                        ISBN = (oReader["ISBN"].ToString()),
                        Name = oReader["Name"].ToString(),
                        PageNumber = Convert.ToInt32(oReader["PageNumber"].ToString()),
                        Sold = Convert.ToInt32(oReader["Sold"].ToString()),
                        Price = Double.Parse(oReader["Price"].ToString(),System.Globalization.CultureInfo.InvariantCulture)
                    };
                    booksToAdd.Add(bookToAdd);

                }
            }
            return booksToAdd;
        }

        public static Author SelectWithIdAuthor(string Id, IDbConnection db)
        {
            return db.Query<Author>($"Select * From Authors Where Id = {Id}").SingleOrDefault();
        }

        public static List<Author> GetAllAuthors(IDbConnection db)
        {
            return db.Query<Author>($"Select * From Authors").ToList();
        }

        public static Book SelectWithIsbnBook(string isbn, IDbConnection db)
        {
            return db.Query<Book>($"Select * From BookStore.dbo.Books Where ISBN = {isbn}").SingleOrDefault();
        }

        public static List<Book> GetAllBooks(IDbConnection db)
        {
            return db.Query<Book>($"Select * From Books").ToList();
        }

        static void Main(string[] args)
        {

            const string connection =
                @"Data Source=HB-IK-61\SQLEXPRESS;Initial Catalog=BookStore;Integrated Security=True";
            IDbConnection db = new SqlConnection(connection);
            try
            {
                db.Open();
            }
            catch
            {
                Console.WriteLine("Can not establish connection");
                return;
            }
            //DataContext olayini kullanarak class lari ve connectionlari database e gore ayarlarsan direk alabiliyorsun
            //LINQ to SQL bak iyice incee
            /*  List<Author> authorFromSqlAuthors = new List<Author>();
              DataContext db = new DataContext(conn);
              db.DatabaseExists();
              authorFromSqlAuthors= db.GetTable<Author>().ToList();*/

            var authorsFromSql =GetAllAuthors(db);
            var booksFromSql = GetAllBooks(db);


            var qu = db.Query(
                "SELECT Books.ISBN, Books.Name, Authors.FirstName, Authors.LastName FROM Authors INNER JOIN Books ON Books.AuthorId = Authors.ID; ");



            foreach (var book in booksFromSql)
            {
                book.UpdateDataBase(db);
            }



            db.Close();
            //Console.ReadKey();
        }
    }
}
