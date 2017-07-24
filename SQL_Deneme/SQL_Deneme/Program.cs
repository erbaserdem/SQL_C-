using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Linq;
using Dapper;

namespace SQL_Deneme
{

    

    class Program
    {








        public static void ReadBooksFromFile(List<Book> booksFromSql, string file_name,IDbConnection db)
        {
            List<double> ratings = new List<double>();
            //XML read from file
            var xml = new XDocument();
            try
            {
                xml = XDocument.Load("Books.xml");
            }
            catch
            {
                Console.WriteLine("Books File is empty");
                return;
            }


            var el = xml.Descendants("Book");

            foreach (var bkk in el)
            {

                var rates = (bkk.Descendants("Rating"));
                var genrs = (bkk.Descendants("Genre"));

                string author = bkk.Element("Author").Value.ToString();
                string AuthorId = AuthorIdFromName(db, author); 

                Book a = new Book
                {
                    AuthorId =  AuthorId,
                    PageNumber = Convert.ToInt32(bkk.Element("Page_number").Value.ToString()),
                    Name = bkk.Element("Name").Value.ToString(),
                    ISBN = (bkk.Element("ISBN").Value.ToString()),
                    Stock = Convert.ToInt32(bkk.Element("Stock").Value.ToString()),
                    Sold = Convert.ToInt32(bkk.Element("Sold").Value.ToString()),
                    Price = Double.Parse(bkk.Element("Price").Value, CultureInfo.InvariantCulture)
                };
                foreach (var rts in rates)
                {
                    ratings.Add(Double.Parse(rts.Value, CultureInfo.InvariantCulture));
                }
                if (ratings.Count() != 0)
                    a.Rating = ratings.Average();
                a.AddBook(db,booksFromSql);

            }

        }

        public static string AuthorIdFromName(IDbConnection db,string getId)
        {
            var aa = getId.Split(' ');
            Author getAuthorId = db.Query<Author>($"Select * From Authors Where (FirstName = '{aa[0]}' AND LastName = '{aa[1]}')").SingleOrDefault();
            return getAuthorId.ID;
        }

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
                        Price = Double.Parse(oReader["Price"].ToString(),CultureInfo.InvariantCulture)
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

        public static Customer SelectWithIdCustomer(string Id, IDbConnection db)
        {
            return db.Query<Customer>($"Select * From Customer Where Id = {Id}").SingleOrDefault();
        }

        public static Customer BestCustomerWithinDates(DateTime start, DateTime end, List<Customer> customersFromSql)
        {
            var orders_date = new List<double>();
            foreach (var customer in customersFromSql)
            {
                orders_date.Add(customer.Orders.FindAll(date => date.DateOfOrder > start && date.DateOfOrder < end).Sum(mny=>mny.Cost));
            }

            if (orders_date.Max() > 0)
            {
                return customersFromSql[orders_date.IndexOf(orders_date.Max())];
            }
            else
            {
                Console.WriteLine("No customer has given an order between given dates");
                return null;
            }
        }

        public static Customer BestCustomerAllTime(List<Customer> customersFromSql)
        {
            var ordersCost = new List<double>();
            foreach (var customer in customersFromSql)
            {
                ordersCost.Add(customer.Orders.Sum(mny => mny.Cost));
            }

            if (ordersCost.Max() > 0)
            {
                return customersFromSql[ordersCost.IndexOf(ordersCost.Max())];
            }
            else
            {
                Console.WriteLine("No customer has given an order between given dates");
                return null;
            }
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

        public static List<Order> GetAllOrders(IDbConnection db)
        {
            return db.Query<Order>($"Select * From Orders").ToList();
        }

        public static List<Order> GetOrderNo(IDbConnection db,string OrderNo)
        {
            return db.Query<Order>($"Select * From Orders Where OrderNo = '{OrderNo}'").ToList();
        }

        public static List<Order> GetUnProcessedOrders(IDbConnection db, string OrderNo)
        {
            return db.Query<Order>($"Select * From Orders Where Processed = '{OrderNo}'").ToList();
        }

        public static List<Item> GetAllItems(IDbConnection db)
        {
            return db.Query<Item>($"Select * From Item").ToList();
        }

        public static List<Customer> GetAllCustomers(IDbConnection db)
        {
            return db.Query<Customer>($"Select * From Customer").ToList();
        }

        public static void ProcessOrders(IDbConnection db, List<Order> ordersFromSql)
        {
            var orderList = ordersFromSql.FindAll((done=>! done.Processed));
            foreach (var order in orderList)
            {
                var completed = 0;
                foreach (var item in order.OrderedItems)
                {
                    item.Amount = item.OrderedBook.SellBook(db, item.Amount);
                    completed += item.Amount;
                    item.OrderedBook.UpdateDataBase(db);
                }
                order.Processed = completed==0;
            }


        }

        public static void UpdateDb(IDbConnection db,List<Book> booksFromSql,List<Author> authorsFromSql,List<Order> ordersFromSql,List<Item> itemsFromSql, List<Customer> customersFromSql)
        {
            foreach (var book in booksFromSql)
            {
                book.UpdateDataBase(db);
            }
            foreach (var order in ordersFromSql)
            {
                order.UpdateDataBase(db);
            }
            foreach (var author in authorsFromSql)
            {
                author.UpdateDataBase(db);
            }
            foreach (var item in itemsFromSql)
            {
                item.UpdateDataBase(db);
            }
            foreach (var customer in customersFromSql)
            {
                customer.UpdateDataBase(db);
            }
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


            var authorsFromSql =GetAllAuthors(db);
            var booksFromSql = GetAllBooks(db);
            var ordersFromSql = GetAllOrders(db);
            var itemsFromSql = GetAllItems(db);
            var customersFromSql = GetAllCustomers(db);

            foreach (var book in booksFromSql)
            {
                book.AuthorOfBook = SelectWithIdAuthor(book.AuthorId, db);
                authorsFromSql.Find(authorId=>authorId.ID == book.AuthorId).Books.Add(book);
                authorsFromSql.Find(authorId => authorId.ID == book.AuthorId).NumberOfBooks++;
                authorsFromSql.Find(authorId => authorId.ID == book.AuthorId).UpdateDataBase(db);
            }

            foreach (var item in itemsFromSql)
            {
                item.Cost = SelectWithIsbnBook(item.BookId,db).Price * item.Amount;
                item.UpdateDataBase(db);
                item.OrderedBook = SelectWithIsbnBook(item.BookId, db);
                ordersFromSql.Find(orderid => orderid.OrderNo == item.OrderNo).OrderedItems.Add(item);
                ordersFromSql.Find(orderid => orderid.OrderNo == item.OrderNo).Cost += item.Cost;
                ordersFromSql.Find(orderid => orderid.OrderNo == item.OrderNo).UpdateDataBase(db);
            }

            foreach (var order in ordersFromSql)
            {
                customersFromSql.Find(customerid => customerid.ID == order.CustomerId).Orders.Add(order);
                customersFromSql.Find(customerid => customerid.ID == order.CustomerId).MoneySpent += order.Cost;
                customersFromSql.Find(customerid => customerid.ID == order.CustomerId).UpdateDataBase(db);
            }

            ProcessOrders(db,ordersFromSql);


            //You can Read Books From XML File unrecognized authors' books will be discarded
            //ReadBooksFromFile(booksFromSql, " ", db);







          /*  var qu = db.Query(
                "SELECT Books.ISBN, Books.Name, Authors.FirstName, Authors.LastName FROM Authors INNER JOIN Books ON Books.AuthorId = Authors.ID; ");*/

            //used for updating all of the entities in DB
            UpdateDb(db,booksFromSql,authorsFromSql,ordersFromSql,itemsFromSql,customersFromSql);
            db.Close();
            Console.ReadKey();
        }




    }
}
