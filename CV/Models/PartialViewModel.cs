using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CV.Models
{
    public class PartialViewModel<T>
    {
        public bool IsForSearch { get; set; }
        public IEnumerable<T> ItemList { get; set; }

    }
}
