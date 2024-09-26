using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class Bed
    {
        [Key]
        public int BedId { get; set; }
        [DisplayName("Bed Number")]
        public string BedNumber { get; set; }
        [ValidateNever]
        public Ward Ward { get; set; }
        [Display(Name = "Ward Number")]

        public int WardId { get; set; }

    }
}
