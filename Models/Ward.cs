using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class Ward
    {
        [Key]
        public int WardId { get; set; }
        [Display(Name = "Ward Number")]
        public string WardNumber { get; set; }
        [ValidateNever]
        public Hospital Hospital { get; set; }
        public int? HospitalId { get; set; }

        public List<Theatre> Theatres { get; set; }
    }
}
