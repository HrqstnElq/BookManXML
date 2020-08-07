using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookMan.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required, DisplayName("Tên sách")]
        public string Name { get; set; }
        [Required, DisplayName("Tác giả")]
        public string Authors { get; set; }
        [Required, DisplayName("Nhà xuất bản")]
        public string Publisher { get; set; }
        [Required, Range(1990, int.MaxValue), DisplayName("Năm xuất bản")]
        public int Year { get; set; } = DateTime.Now.Year;
        public string Description { get; set; } = "";
        [DisplayName("File")]
        public string DataFile { get; set; }
    }
}
