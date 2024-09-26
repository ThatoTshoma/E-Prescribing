using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class ActiveIngredient
    {
        [Key]
        public int IngredientId { get; set; }
        public string Name { get; set; }
    }
}
