using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BookListsMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookListMVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public Book Book { get; set; }
        //dependancy injection below
        public BooksController(ApplicationDbContext db)
        {
            _db = db;
        }
        //below is action that results to index.cshml view
        public IActionResult Index()
        {
            return View();
        }
        //below is action that results to upsert.cshml view

        public IActionResult Upsert(int? id)//creates a nullable id due to create book idea 
        {
            Book = new Book();
            if (id == null)
            {
                //create
                return View(Book);
                //or you can write return View(Book Book=new Book());
            }
            //update(retrieving the bbok to be updated)
            Book = _db.Books.FirstOrDefault(u => u.Id == id);


            if (Book == null)
            {
                return NotFound();
            }
            return View(Book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                if (Book.Id == 0)
                {
                    //create a book
                    _db.Books.Add(Book);
                }
                else
                {
                    _db.Books.Update(Book);
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Book);
        }

        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _db.Books.ToListAsync() });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var bookFromDb = await _db.Books.FirstOrDefaultAsync(u => u.Id == id);
            if (bookFromDb == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }
            _db.Books.Remove(bookFromDb);
            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Delete successful" });
        }
        #endregion
    }
}