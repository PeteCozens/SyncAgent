using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Models
{
    public class DemoFormVM
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Range(1, 100, ErrorMessage = "Please enter your age")]
        public int Age { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        public bool IsSubscribed { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string FavoriteColor { get; set; } = string.Empty;


        public List<SelectListItem> CheckboxOptions { get; set; } = new();
    }
}
