using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace E_Prescribing.Models
{
    public class Suburb
    {
        [Key]
        public int SuburbId { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Postal Code")]
        public int PostalCode { get; set; }
        [ValidateNever]
        public City City { get; set; }
        [Display(Name = "City Name")]
        public int CityId { get; set; }
    }
}
