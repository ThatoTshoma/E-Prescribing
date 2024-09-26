using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Prescribing.Models
{
    public class PatientAllergy
    {
        [Key]
        public int AllergyId { get; set; }
        public Patient Patient { get; set; }
        [Display(Name = "Patient")]
        public int PatientId { get; set; }
        public ActiveIngredient ActiveIngredient { get; set;}
        [Display(Name = "Active Ingredient Name")]
        public int ActiveIngredientId { get; set; }
        [NotMapped]
        public List<int> SelectedActiveIngredient { get; set; }


    }
}
