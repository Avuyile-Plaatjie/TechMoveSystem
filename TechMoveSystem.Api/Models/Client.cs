using System.ComponentModel.DataAnnotations;

namespace TechMoveSystem.Api.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Full Name / Entity is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        [Display(Name = "Full Name / Entity")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Contact Information is required.")]
        [Display(Name = "Contact Information")]
        public string ContactDetails { get; set; }

        [Required(ErrorMessage = "Please assign a region.")]
        [Display(Name = "Assigned Region")]
        public string Region { get; set; }

        
        public ICollection<Contract>? Contracts { get; set; }
    }
}