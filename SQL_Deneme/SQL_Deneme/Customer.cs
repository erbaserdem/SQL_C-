using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL_Deneme
{
    public class Customer
    {
        public string ID;
        public string Name;
        public List<Order> Orders = new List<Order>();
        public double MoneySpent;
    }
}
