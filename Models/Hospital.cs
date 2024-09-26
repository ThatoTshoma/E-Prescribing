using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class Hospital
    {
        [Key]
        public int HospitalId { get; set; }
        public string Name { get; set; }
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }
        [Display(Name = "Hospital Practice Number")]
        public string HospitalNumber { get; set; }
        [EmailAddress]
        [Display(Name = "Puchase Manager Email Address")]
        public string PurchaseManagerEmailAddress { get; set; }
        [ValidateNever]
        public Suburb Suburb { get; set; }
        [Display(Name = "Suburb Name")]
        public int SuburbId { get; set; }

    }
}
