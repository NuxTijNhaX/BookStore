using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            var obj = _db.Products.FirstOrDefault(c => c.Id == product.Id);

            if(obj != null)
            {
                if(product.ImgUrl != null)
                {
                    obj.ImgUrl = product.ImgUrl;
                }

                obj.Name = product.Name;
                obj.Description = product.Description;
                obj.Author = product.Author;
                obj.ISBN = product.ISBN;
                obj.Price = product.Price;
                obj.Price50 = product.Price50;
                obj.Price100 = product.Price100;
                obj.ListPrice = product.ListPrice;
                obj.CategoryId = product.CategoryId;
                obj.CoverTypeId = product.CoverTypeId;
            }
        }
    }
}
