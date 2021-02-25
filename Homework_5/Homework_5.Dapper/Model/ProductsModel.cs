using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Homework_5.Dapper.Model
{
    public class ProductsModel
    {
        public int ProductID { get; set; }

        public string ProductName { get; set; }
        public List<OrdersDetailModel> item { get; set; }

    }
}
