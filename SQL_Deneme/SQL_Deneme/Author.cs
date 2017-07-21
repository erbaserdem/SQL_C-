using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL_Deneme
{
    [Table(Name = "Authors")]
    class Author
    {
        //[Column(IsPrimaryKey = true, Name = "ID")]
        public int Id;
       // [Column(Name = "FirstName")]
        public string FirstName;
        //[Column(Name = "LastName")]
        public string LastName;
    }
}
