using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechMoveSystem.Models
{
    public class ServiceRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Cost (USD)")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CostInUsd { get; set; }

        [Display(Name = "Cost (ZAR)")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CostInZar { get; set; }

        public string Status { get; set; } = "Pending";

        public int ContractId { get; set; }
        public Contract? Contract { get; set; }
    }
}