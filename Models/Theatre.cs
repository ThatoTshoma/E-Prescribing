
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class Theatre
    {
        [Key]
        public int TheatreId { get; set; }
        public string Name { get; set; }
        [ValidateNever] 
        public Ward Ward { get; set; }
        [Display(Name = "Ward Number")]
        public int WardId { get;set; }
    }
}
