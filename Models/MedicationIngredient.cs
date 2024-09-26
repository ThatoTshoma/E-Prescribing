using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Prescribing.Models
{
    public class MedicationIngredient
    {
        [Key]
        public int MedicationIngredientId { get; set; }
        public ActiveIngredient ActiveIngredient { get; set;}
        [Display(Name = "Active Ingredient Name")]
        public int ActiveIngredientId { get;set; }
        public Medication Medication { get; set; }
        [Display(Name = "Medication Name")]
        public int MedicationId { get; set; }
        public string ActiveIngredientStrength { get; set; }
        [NotMapped]
        public List<int> SelectedIngredient { get; set; }






    }
}
