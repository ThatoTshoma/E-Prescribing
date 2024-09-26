using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class ContraIndication
    {
        [Key]
        public int ContraIndicationId { get; set; }
        public ActiveIngredient ActiveIngredient { get; set; }
        [Display(Name = "Active Ingredient Name")]
        public int ActiveIngredientId { get; set; }
        public ConditionDiagnosis ConditionDiagnosis { get; set; }
        [Display(Name = "Codintion Diagnosis")]
        public int ConditionDiagnosisId { get; set; }
    }
}
