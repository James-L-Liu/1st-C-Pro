using System;
using System.ComponentModel.DataAnnotations;
namespace A2.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserComment { get; set; }

        public string Name { get; set; }

        public string Time { get; set; }

        public string IP { get; set; }
    }
}