using System.Collections.Generic;

namespace CV.Models
{
    public class PartialViewModel<T>
    {
        public bool IsForSearch { get; set; }
        public IEnumerable<T> ItemList { get; set; }

    }
}
