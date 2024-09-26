using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class DosageForm
    {
        [Key]
        public int DosageId { get; set; }
        public string Name { get; set; }
    }
}
