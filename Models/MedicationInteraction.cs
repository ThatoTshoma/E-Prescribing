using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class MedicationInteraction
    {
        [Key]
        public int InteractionId { get; set; }
        [ValidateNever]
        public ActiveIngredient ActiveIngredient1 { get; set; }
        [Display(Name = "Active Ingredient 1")]
        public int ActiveIngredient1Id { get; set; }
        [ValidateNever]

        public ActiveIngredient ActiveIngredient2 { get; set; }
        [Display(Name = "Active Ingredient 2")]
        public int ActiveIngredient2Id { get; set; }
        [ValidateNever]
        public string Description { get; set; }
    }
}
