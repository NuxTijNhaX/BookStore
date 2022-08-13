using BookStore.DataAccess.Repository;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Category category = new Category();

            if(id == null)
            {
                // Create
                return View(category);
            }

            // Edit
            category = _unitOfWork.CategoryRepository.Get(id.GetValueOrDefault());
            if(category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        #region API Call

        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = _unitOfWork.CategoryRepository.GetAll();
            return Json(new { data = categories});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if(ModelState.IsValid)
            {
                if(category.Id != 0)
                {
                    _unitOfWork.CategoryRepository.Update(category);
                }
                else
                {
                    _unitOfWork.CategoryRepository.Add(category);
                }

                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var category = _unitOfWork.CategoryRepository.Get(id);

            if (category == null)
            {
                return Json(new { success = false, message = "Cannot delete" });
            }

            _unitOfWork.CategoryRepository.Delete(category);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Deleted" });
        }

        #endregion
    }
}
