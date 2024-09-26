using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Prescribing.Models
{
    public class PatientCondition
    {
        [Key]
        public int PatientConditionId { get; set; }
        public Patient Patient { get; set; }
        public int PatientId { get; set; }
        [Display(Name = "Patient Name")]
        public ConditionDiagnosis Condition { get; set; }
        [Display(Name = "Condition Dignosis")]
        public int ConditionId { get; set; }

        [NotMapped]
        public List<int> SelectedCondition { get; set; }
    }

}
