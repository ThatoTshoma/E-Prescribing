using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace E_Prescribing.Models
{
    public class City
    {
        [Key]
        public int CityId { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        [Display(Name = "City Name")]
        public string Name { get; set; }
        [ValidateNever]
        public Province Province { get; set; }
        [Display(Name = "Province Name")]
        public int ProvinceId { get; set; }
    }
}
