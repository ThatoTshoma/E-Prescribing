using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class ConditionDiagnosis
    {
        [Key]
        public int ConditionId { get; set; }
        [Display(Name = "ICD-10 Code")]

        public string Icd10Code { get; set; }
        public string Name { get; set; }
    }
}
