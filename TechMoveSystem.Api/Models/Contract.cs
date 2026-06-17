using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechMoveSystem.Api.Models
{
    public class Contract
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a client.")]
        [Display(Name = "Client")]
        public int ClientId { get; set; }

        // Navigation property marked as optional to bypass creation validation checks
        public Client? Client { get; set; }

        [Required(ErrorMessage = "Please select a start date.")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Please select an end date.")]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Please assign an initial status.")]
        public string Status { get; set; } 

        [Required(ErrorMessage = "Please select a service level tier.")]
        [Display(Name = "Service Level")]
        public string ServiceLevel { get; set; } 

        [Display(Name = "Signed Agreement Path")]
        public string? SignedAgreementPath { get; set; }

        
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }
}