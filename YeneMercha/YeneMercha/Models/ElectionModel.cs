using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YeneMercha.Models
{
    public class ElectionModel
    {
        public IEnumerable<Catagory> catagories { get; set; }
        public IEnumerable<Election> elections { get; set; }
    }
}