using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Homework_5.Dapper.Model;
using Microsoft.Extensions.Configuration;

namespace Homework_5.Dapper.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ProductsController : ControllerBase
    {
        
        private readonly ILogger<ProductsController> _logger;
        private readonly IConfiguration _configuration;

        public ProductsController(ILogger<ProductsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }


        [HttpGet]
        public ActionResult<IEnumerable<ProductsModel>> DapperSelectInQuery()
        {
            IEnumerable<ProductsModel> products = new List<ProductsModel>();
            using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                if (db.State != ConnectionState.Open)
                    db.Open();

                // standart bir SQL sorgusu çalistiriyorum
                // ProductID si 5 olan urunu aliyorum
                products = db.Query<ProductsModel>("SELECT * FROM Products WHERE ProductID = 5 ");

            }
            return new ActionResult<IEnumerable<ProductsModel>>(products);
        }

        [HttpPost]
        public IActionResult DapperInsert([FromBody] ProductsModel productsModel, int abc)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    if (db.State != ConnectionState.Open)
                        db.Open();

                    // Insert islemi yapiyorum
                    //API'ye requestin body'si ile gelen içeriği kullaniyorum.

                    db.Execute(@"INSERT INTO products (ProductID, ProductName)
                         VALUES(@ProductID,@ProductName)",productsModel);
                    return Ok(productsModel);
                }

            }
            catch (Exception e)
            {
                _logger.LogInformation("Insert is failed");
            }

            return Ok(productsModel);

        }

        [HttpPut]
        public IActionResult DapperUpdate([FromBody] ProductsModel productsModel)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    if (db.State != ConnectionState.Open)
                        db.Open();
                    //Update islemi yapiyorum
                    //API'ye requestin body'si ile gelen içeriği kullaniyorum.
                    var result = db.Execute(@"UPDATE products SET ProductName = @ProductName, 
                                                     WHERE ProductId = @ProductId",
                        new
                        {
                            ProductName = productsModel.ProductName
                        });
                    //boyle bir ID yoksa hata atiyorum. 
                    if (result == 1)
                        return Ok(productsModel);
                    else
                        return NotFound();

                }

            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult DapperSpInsert( ProductsModel productsModel)
        {
            try
            {
                //Stored procedures kullanarak insert yapiyorum.
                //Dinamik type kullaniyor.
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    DynamicParameters parm = new DynamicParameters();

                    parm.Add("@ProductName", productsModel.ProductName);
                    parm.Add("@ProductID", productsModel.ProductID);

                    if (db.State != ConnectionState.Open)
                        db.Open();
                    db.Execute("AddNewProduct", parm, commandType: CommandType.StoredProcedure);
                    db.Close();
                    

                }

            }
            catch (Exception e)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DapperDelete(int id)
        {
            using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                if (db.State != ConnectionState.Open)
                    db.Open();
                //Kullanicidan alinacak olan id ye gore silme islemi yapacagim.
                var result = db.Execute(@"DELETE FROM products WHERE ProductID=@id", new {id = id});
                //Silme isleminin basarili olup olmadigini kontrol ediyorum
                if (result == 1)
                    return Ok();
                else
                    return NotFound();

            }
        }

        [HttpPost()]
        public IActionResult DapperTransactionalInsert([FromBody] ProductsModel productsModel, int insert)
        {
            using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                if (db.State != ConnectionState.Open)
                    db.Open();

                //Transaction islemi yapiyorum Products ve Order Detail tablosunu kullanarak.
                using (var transaction = db.BeginTransaction())
                {
                    string sql = @"INSERT INTO dbo.products (ProductName, ProductID)
                                                  Values (@ProductName, @ProductID);";
                    var result = db.Execute(sql, new
                    {
                        ProductName = "Deneme",
                        ProductID = 80,
                        
                    }, transaction);
                    throw new ArgumentNullException();
                    OrdersDetailModel ordersDetailModel= new OrdersDetailModel()
                    {
                        OrderId = 80,
                        ProductId = 80,
                        Quantity = 1500
                    };
                    sql = @"Insert into [products].[Order Details] (OrderId, ProductId, Quantity)
                                Values (@OrderId, @ProductId, @Quantity)";
                    result = db.Execute(sql, ordersDetailModel, transaction);
                    transaction.Commit();
                }
            }
            return Ok();
        }

        [HttpPost()]
        public IActionResult MultipleQueryMapping(ProductsModel productsModel, int mapping)
        {
            // Products ve Order Detail tablosunu ProductID uzerinden birlestirdim. 
            string sql = "SELECT * FROM products AS A INNER JOIN Order Details AS B ON A.ProductID = B.ProductID;";

            using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                if (db.State != ConnectionState.Open)
                    db.Open();

                var productsDictionary = new Dictionary<int, ProductsModel>();

                //Aldigim result i one-to-many mapping yaparak donuyorum.
                var result = db.Query<ProductsModel, OrdersDetailModel, ProductsModel > (
                            sql,
                            (productsModel, ordersDetailModel) =>
                            {
                                ProductsModel products;
                                if (!productsDictionary.TryGetValue(productsModel.ProductID, out products))
                                {
                                    products = productsModel;
                                    products.item = new List<OrdersDetailModel>();
                                    productsDictionary.Add(products.ProductID, products);

                                }

                                products.item.Add(ordersDetailModel);
                                return products;

                            },
                            splitOn: "ProductID")
                        .Distinct()
                        .ToList();

            }
            return Ok();
        }

       
    }
}
