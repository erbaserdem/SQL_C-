using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
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

        //bunu ve validationlari duzenle
        public static void ReadOrdersFromCSV(IDbConnection db, List<Order> OrderList, List<Book> Books, string file_name, List<Customer> CustomerList)
        {
            //Read Values From the file



            string[] allLines = File.ReadAllLines(file_name);
            foreach (var line in allLines)
            {
                string ID = string.Empty; string isbn = string.Empty; string amount = string.Empty; string discount_rate = string.Empty;
                var data = line.Split(',').ToList();
                ID = data[0];
                isbn = data[1];
                amount = data[2];
                if (data.Count() == 4)
                    discount_rate = data[3];
                if (CustomerValidation(ID, CustomerList) & ISBNValidation(isbn, Books) & AmountValidation(amount) & DiscountRateValidation(ref discount_rate)) // After this line it places the order according to the infoprovided
                {
                    var discount = 100 - Double.Parse(discount_rate, System.Globalization.CultureInfo.InvariantCulture);
                    discount = discount / 100;
                    List<Customer> Precious_Customers = CustomerList.Where(mny => mny.MoneySpent > (CustomerList.Average(mn => mn.MoneySpent) * 10)).ToList(); // Customers whom have spent 10 times more than the average customer gets 10% extra discount
                    // only if their current discount is less than 50 %
                    if (discount > 0.5 && Precious_Customers.Find(id => id.ID == ID) != null)
                    {
                        discount -= 0.1;
                    }
                    Item item = new Item
                    {
                        Amount = Convert.ToInt32(amount),
                        OrderedBook = Books.Find(isb=>isb.ISBN == isbn),
                        BookId = isbn,
                    };
                    Order New_Order = new Order
                    {
                        Customer = CustomerList.Find(find => find.ID == ID),
                        CustomerId = ID,
                        //OrderNo = (OrderList.Count() + 1).ToString(),
                        DateOfOrder = DateTime.Now,
                        Discount = discount
                    };
                    New_Order.OrderedItems.Add(item);
                    New_Order.Cost = New_Order.OrderedItems.Sum(pr => pr.OrderedBook.Price * pr.Amount );
                    // New_Order.customer.Money_Spent += New_Order.cost;
                    New_Order.AddOrder(db);

                    item.OrderNo = db.Query<Order>("Select * from BookStore.dbo.Orders").Last().OrderNo;
                    item.Order = OrderList.Find(no=>no.OrderNo == item.OrderNo);
                    item.AddItem(db);

                    Console.WriteLine($"Your Order for the book {item.OrderedBook.Name} has been issued with the orderno: {item.OrderNo}");
                }
                else
                    Console.WriteLine("Not a valid input");

            }


        }

        public static bool CustomerValidation(string ID, List<Customer> CustomerList)
        {
            if (string.IsNullOrWhiteSpace(ID) & string.IsNullOrEmpty(ID))
            {
                Console.WriteLine("White space, Empty and NULL ID's are not allowed");
                return false;
            }
            if (!ID.All(str => str >= '0' & str <= '9'))
            {
                Console.WriteLine("Customer ID consists only of numerical values");
                return false;
            }
            if (ID.Length > 13)
            {
                Console.WriteLine("Customer ID must be a 13 digitnumber. If entered less it will bepadded with 0's.");
                return false;
            }
            if (CustomerList.Find(find => find.ID == ID) == null)
            {
                Console.WriteLine("Not a valid CustomerID");
                return false;
            }



            ID = ID.PadLeft(13, '0');
            return true;
        }

        public static bool ISBNValidation(string ID, List<Book> Books)
        {

            if (string.IsNullOrWhiteSpace(ID) & string.IsNullOrEmpty(ID))
            {
                Console.WriteLine("White space, Empty and NULL ISBN's are not allowed");
                return false;
            }
            if (!ID.All(str => str >= '0' & str <= '9'))
            {
                Console.WriteLine("ISBN consists only of numerical values");
                return false;
            }
            if (ID.Length > 13)
            {
                Console.WriteLine("ISBN must be a 13 digitnumber. If entered less it will bepadded with 0's.");
                return false;
            }
            if ((Books.Find(id=>id.ISBN==ID)) == null)
            {
                Console.WriteLine("Not a valid ISBN");
                return false;
            }
            return true;
        }

        public static bool AmountValidation(string ID)
        {
            if (string.IsNullOrWhiteSpace(ID) & string.IsNullOrEmpty(ID))
            {
                Console.WriteLine("White space, Empty and NULL Amounts are not allowed");
                return false;
            }
            if (!ID.All(str => str >= '0' & str <= '9'))
            {
                Console.WriteLine("Amount of book consists only of numerical values");
                return false;
            }
            if (Convert.ToInt32(ID) > 99 && Convert.ToInt32(ID) < 0)
            {
                Console.WriteLine("Amount of book to be bought must bebetween 0 an 100");
                return false;
            }


            return true;
        }

        public static bool DiscountRateValidation(ref string ID)
        {
            if (string.IsNullOrWhiteSpace(ID) & string.IsNullOrEmpty(ID))
            {
                ID = "100";
                return true;
            }

            if (!ID.All(str => (str >= '0' & str <= '9') || str == '.'))
            {
                Console.WriteLine("Discount rate consists only of numerical values and '.' for decimal ones");
                return false;
            }
            if (0 > Double.Parse(ID, System.Globalization.CultureInfo.InvariantCulture) && 100 < Double.Parse(ID, System.Globalization.CultureInfo.InvariantCulture))
            {
                Console.WriteLine("Discount rate must be between 0 and 100");
                return false;
            }

            return true;
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

        public static void ManageConnections(IDbConnection db, List<Book> booksFromSql, List<Author> authorsFromSql, List<Order> ordersFromSql, List<Item> itemsFromSql, List<Customer> customersFromSql)
        {
            foreach (var book in booksFromSql)
            {
                book.AuthorOfBook = authorsFromSql.Find(id=>id.ID == book.AuthorId);
                authorsFromSql.Find(authorId => authorId.ID == book.AuthorId).Books.Add(book);
                authorsFromSql.Find(authorId => authorId.ID == book.AuthorId).NumberOfBooks++;
                authorsFromSql.Find(authorId => authorId.ID == book.AuthorId).UpdateDataBase(db);
            }

            foreach (var item in itemsFromSql)
            {
                item.Cost = booksFromSql.Find(id => id.ISBN == item.BookId).Price * item.Amount;
                item.UpdateDataBase(db);
                item.OrderedBook = booksFromSql.Find(id => id.ISBN == item.BookId);
                if (item.Amount>0)
                {
                    ordersFromSql.Find(orderid => orderid.OrderNo == item.OrderNo).Processed = false;
                }
                ordersFromSql.Find(orderid => orderid.OrderNo == item.OrderNo).OrderedItems.Add(item);
                ordersFromSql.Find(orderid => orderid.OrderNo == item.OrderNo).UpdateDataBase(db);
            }

            foreach (var order in ordersFromSql)
            {
                order.Cost = order.OrderedItems.Sum(mny => mny.Cost);
                order.Customer = customersFromSql.Find(customerid => customerid.ID == order.CustomerId);
                customersFromSql.Find(customerid => customerid.ID == order.CustomerId).Orders.Add(order);
                customersFromSql.Find(customerid => customerid.ID == order.CustomerId).UpdateDataBase(db);
            }
        }

        public static void ProcessOrders(IDbConnection db, List<Order> ordersFromSql, List<Book> booksFromSql)
        {
            var orderList = ordersFromSql.FindAll((done=>! done.Processed));
            foreach (var order in orderList)
            {
                var completed = 0;
                foreach (var item in order.OrderedItems)
                {
                    if (item.Amount <= 0) continue;
                    order.Customer.MoneySpent += item.Cost * (100 - order.Discount) / 100;
                    item.Amount = item.OrderedBook.SellBook(db, item.Amount);
                    order.Customer.MoneySpent -= item.OrderedBook.Price * item.Amount * (100 - order.Discount) / 100;
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

        public static void RestockBooks(IDbConnection db, List<Book> booksFromSql)
        {
            foreach (var book in booksFromSql)
            {
                if (book.AwaitingOrders != 0 && book.Stock==0 )
                {
                    book.ReStock(db, book.AwaitingOrders);
                }
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

            //ReadOrdersFromCSV(db, ordersFromSql, booksFromSql, "csv.txt", customersFromSql);
            //ordersFromSql = GetAllOrders(db);

            ManageConnections(db,booksFromSql,authorsFromSql,ordersFromSql,itemsFromSql,customersFromSql);

            ProcessOrders(db,ordersFromSql,booksFromSql);
            RestockBooks(db,booksFromSql);

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
