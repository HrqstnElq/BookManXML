using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BookMan.Models
{
    public class Service
    {
        private readonly string _dataFile = @"./Data/data.xml";
        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(HashSet<Book>));
        public string GetDataPath(string file) => $@".\Data\{file}";
        public HashSet<Book> Books { get; set; }
        public Service()
        {
            if(!File.Exists(_dataFile))
            {
                Books = new HashSet<Book>() {
                    new Book{Id = 1, Name = "ASP.NET Core for dummy", Authors = "Trump D.", Publisher = "Washington", Year = 2020},
                    new Book{Id = 2, Name = "Pro ASP.NET Core", Authors = "Putin V.", Publisher = "Moscow", Year = 2020},
                    new Book{Id = 3, Name = "ASP.NET Core Video course", Authors = "Obama B.", Publisher = "Washington", Year = 2020},
                    new Book{Id = 4, Name = "Programming ASP.NET Core MVC", Authors = "Clinton B.", Publisher = "Washington", Year = 2020},
                    new Book{Id = 5, Name = "ASP.NET Core Razor Pages", Authors = "Yelstin B.", Publisher = "Moscow", Year = 2020},
                };
            }
            else
            {
                using var stream = File.OpenRead(_dataFile);
                Books = _serializer.Deserialize(stream) as HashSet<Book>;
            }
        }

        public Book[] Get() => Books.ToArray();
        public Book[] Get(string search)
        {
            var s = search.ToLower();
            return Books.Where(b =>
               b.Name.ToLower().Contains(s) ||
               b.Authors.ToLower().Contains(s) ||
               b.Publisher.ToLower().Contains(s) ||
               b.Description.ToLower().Contains(s) ||
               b.Year.ToString() == s
                ).ToArray();
        }
        public (Book [] books, int pages, int page) Paging(int page, string orderBy = "Name", bool dsc = false)
        {
            int size = 5;
            int pages = (int)Math.Ceiling((double)Books.Count / size);
            var books = Books.Skip((page - 1) * size).Take(size).AsQueryable().OrderBy($"{orderBy} {(dsc ? "descending" : "")}").ToArray();
            return (books, pages, page);
        }
        public Book Get(int id) => Books.FirstOrDefault(b => b.Id == id);
        public Book Create()
        {
            var max = Books.Max(b => b.Id);
            var b = new Book() { Id = ++max };
            return b;
        }
        public bool Add(Book book) => Books.Add(book);
        public bool Delete(int id)
        {
            var b = Get(id);
            return b != null ? Books.Remove(b) : false;
        }
        public bool Update(Book book)
        {
            var b = Get(book.Id);
            return b != null ? Books.Remove(b) && Books.Add(book) : false;
        }
        public void SaveChange()
        {
            using var stream = File.Create(_dataFile);
            _serializer.Serialize(stream, Books);
        }
        public void Upload(Book book, IFormFile file)
        {
            if(file != null)
            {
                string path = GetDataPath(file.FileName);
                using var stream = new FileStream(path, FileMode.Create);
                file.CopyTo(stream);
                book.DataFile = file.FileName;
            }
        }
        public (Stream, string) Dowload(Book book)
        {
            var memory = new MemoryStream();
            using var stream = new FileStream(GetDataPath(book.DataFile), FileMode.Open);
            stream.CopyTo(memory);
            memory.Position = 0;
            String type = Path.GetExtension(book.DataFile) switch
            {
                "pdf" => "application/pdf",
                "docx" => "application/vnd.ms-word",
                "doc" => "application/vnd.ms-word",
                "txt" => "text/plain",
                _ => "application/pdf"
            };
            return (memory, type);
        }

    }
}
