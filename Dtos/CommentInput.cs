using System;
using System.ComponentModel.DataAnnotations;

namespace A2.Dtos
{
    public class CommentInput
    {
        [Required]
        public string UserComment { get; set; }
        [Required]
        public string Name { get; set; }
    }
}