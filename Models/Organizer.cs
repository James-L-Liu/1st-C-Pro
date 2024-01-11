using System;
using System.ComponentModel.DataAnnotations;
namespace A2.Models
{
    public class Organizor
    {
        [Key]
        public string Name { get; set; }
        public string Password { get; set; }
    }
}