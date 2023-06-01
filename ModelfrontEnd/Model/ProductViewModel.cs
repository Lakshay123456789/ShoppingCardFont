using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelfrontEnd.Model
{
  public  class ProductViewModel
    {
        public IQueryable<Product> Products { get; set;}

        public string NameSortOrder { get; set; }

        public int PageSize { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public string OrderBy { get; set; }
    }
}
