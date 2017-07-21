using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL_Deneme
{
    public class Item
    {
        public int Amount { get; set; }
        public Book OrderedBook;
        public string Item_ID;
        public bool competed;
    }
}
