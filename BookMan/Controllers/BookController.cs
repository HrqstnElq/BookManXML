using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookMan.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookMan.Controllers
{
    public class BookController : Controller
    {
        public readonly Service _service;
        public BookController(Service service) => _service = service;
        public IActionResult Index(int page = 1, string orderBy = "Name", bool dsc =false)
        {
            var model = _service.Paging(page, orderBy, dsc);
            ViewData["Pages"] = model.pages;
            ViewData["Page"] = model.page;

            ViewData["Name"] = false;
            ViewData["Authors"] = false;
            ViewData["Publisher"] = false;
            ViewData["Year"] = false;
            ViewData[orderBy] = !dsc;
            return View(model.books);
        }
        public IActionResult Details(int id)
        {
            var b = _service.Get(id);
            return View(b);
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var b = _service.Get(id);
            return View(b);
        }
        [HttpPost]
        public IActionResult Delete(Book book)
        {
            _service.Delete(book.Id);
            _service.SaveChange();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            var b = _service.Get(id);
            return View(b);
        }
        public IActionResult Edit(Book book, IFormFile file)
        {
            _service.Upload(book, file);
            _service.Update(book);
            _service.SaveChange();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Create()
        {
            var b = _service.Create();
            return View(b);
        }
        [HttpPost]
        public IActionResult Create(Book book, IFormFile file)
        {
            if(ModelState.IsValid)
            {
                _service.Upload(book, file);
                _service.Add(book);
                _service.SaveChange();
                return RedirectToAction("Index");
            }
            return View(book);
        }
        public IActionResult Read(int id)
        {
            var b = _service.Get(id);
            if (b == null) return NotFound();
            if (!System.IO.File.Exists(_service.GetDataPath(b.DataFile))) return NotFound();

            var (stream, type) = _service.Dowload(b);
            return File(stream, type, b.DataFile);
        }
        [HttpGet]
        public IActionResult Search(string se)
        {
            ViewData["Search"] = 1;
            var model = _service.Get(se);
            return View("Index", model);
        }
    }
}
