using System;
using System.ComponentModel.DataAnnotations;

namespace A2.Dtos
{
    public class EventInput
    {
        [Required]
        
        public string Start { get; set; }
        [Required]
        
        public string End { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        [Required]
        public string Location { get; set; }
    }
}