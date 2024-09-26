using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class ActiveIngredientStrength
    {
        [Key]
        public int StrengthId { get; set; }
        public string Strength { get; set; }
    }
}
