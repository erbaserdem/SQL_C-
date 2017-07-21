using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL_Deneme
{
    public class Order
    {
        public Customer CustomerId;
        public string OrderNo { get; set; }
        public List<Item> Ordered_Items = new List<Item>();
        public bool competed;
        public double cost;
        public double discount;
    }

}
