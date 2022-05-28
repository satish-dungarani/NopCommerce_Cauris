using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Transactions
{

    public class DataTableAjaxPostModel
    {
        // properties are not capital due to json mapping
        [Display(Name ="draw")]
        public int Draw { get; set; }
        [Display(Name = "start")]
        public int Start { get; set; }
        [Display(Name = "length")]
        public int Length { get; set; }
        [Display(Name = "columns")]
        public List<Column> Columns { get; set; }
        [Display(Name = "search")]
        public Search Search { get; set; }
        [Display(Name = "order")]
        public List<Order> Order { get; set; }
    }

    public class Column
    {
        public string data { get; set; }
        public string name { get; set; }
        public bool searchable { get; set; }
        public bool orderable { get; set; }
        public Search search { get; set; }
    }

    public class Search
    {
        public string value { get; set; }
        public string regex { get; set; }
    }

    public class Order
    {
        public int column { get; set; }
        public string dir { get; set; }
    }
}
