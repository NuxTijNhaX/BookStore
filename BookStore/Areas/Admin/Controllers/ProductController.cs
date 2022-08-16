using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Linq;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.CategoryRepository.GetAll().Select(i => new SelectListItem()
                {
                    Value = i.Id.ToString(),
                    Text = i.Name
                }),
                CoverTypeList = _unitOfWork.CoverTypeRepository.GetAll().Select(i => new SelectListItem()
                {
                    Value = i.Id.ToString(),
                    Text = i.Name
                })
            };

            if(id == null)
            {
                return View(productVM);
            }

            productVM.Product = _unitOfWork.ProductRepository.Get(id.GetValueOrDefault());
            if(productVM.Product == null)
            {
                return NotFound();
            }

            return View(productVM);
        }

        #region API

        [HttpGet]
        public IActionResult GetAll()
        {
            var objs = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category,CoverType");
            return Json(new { data = objs });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if(ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                // Have a uploading files
                if(files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString(); // Create new file name
                    var uploadLocation = Path.Combine(rootPath, "img", "products");
                    var extension = Path.GetExtension(files[0].FileName);

                    // Edit
                    if(productVM.Product.ImgUrl != null)
                    {
                        // Edit situation, need to remove old img
                        var uploadedImgPath = Path.Combine(rootPath, productVM.Product.ImgUrl.TrimStart('\\'));

                        if(System.IO.File.Exists(uploadedImgPath))
                        {
                            System.IO.File.Delete(uploadedImgPath);
                        }
                    }

                    using(var filesStream = new FileStream(Path.Combine(uploadLocation, 
                        fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filesStream);
                    }

                    productVM.Product.ImgUrl = @"\img\products\" + fileName + extension;
                }
                else
                {
                    // Edit when not change img
                    if(productVM.Product.Id != 0)
                    {
                        Product oldProduct = _unitOfWork.ProductRepository.Get(productVM.Product.Id);
                        productVM.Product.ImgUrl = oldProduct.ImgUrl;
                    }
                }

                if(productVM.Product.Id == 0)
                {
                    _unitOfWork.ProductRepository.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.ProductRepository.Update(productVM.Product);
                }

                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                productVM.CategoryList = _unitOfWork.CategoryRepository.GetAll()
                    .Select(i => new SelectListItem()
                {
                    Value = i.Id.ToString(),
                    Text = i.Name
                });
                productVM.CoverTypeList = _unitOfWork.CoverTypeRepository.GetAll()
                    .Select(i => new SelectListItem()
                {
                    Value = i.Id.ToString(),
                    Text = i.Name
                });

                if(productVM.Product.Id != 0)
                {
                    productVM.Product = _unitOfWork.ProductRepository.Get(productVM.Product.Id);
                }
            }

            return View(productVM);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var product = _unitOfWork.ProductRepository.Get(id);
            if(product == null)
            {
                return Json(new { success = false, message = "Cannot delete it" });
            }

            string rootPath = _webHostEnvironment.WebRootPath;
            var uploadedImgPath = Path.Combine(rootPath, product.ImgUrl.TrimStart('\\'));

            if (System.IO.File.Exists(uploadedImgPath))
            {
                System.IO.File.Delete(uploadedImgPath);
            }
            _unitOfWork.ProductRepository.Delete(product);
            _unitOfWork.Save();

            return Json(new { success = true, massage = "Deleted" });
        }

        #endregion
    }
}
