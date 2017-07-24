using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace SQL_Deneme
{
    public class Customer
    {
        public string ID;
        public string Name;
        public List<Order> Orders = new List<Order>();
        public double MoneySpent;






        public void AddCustomer(IDbConnection db)
        {
            db.Query($"INSERT INTO BookStore.dbo.Customer(Name) "
                     + $"Values ('{Name}')");
            return;
        }



        public void UpdateDataBase(IDbConnection db)
        {

            string Query = "UPDATE BookStore.dbo.Customer " +
                           $" SET Name = @Name, MoneySpent = @MoneySpent " +
                           $" WHERE ID = @ID";

            db.Query(Query, new { Name = Name, MoneySpent = MoneySpent, ID = ID });

        }








    }
}
