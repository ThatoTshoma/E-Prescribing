using E_Prescribing.Models;

namespace E_Prescribing.CollectionModel
{
    public class PatientConditionCollection
    {
        public PatientCondition PatientCondition { get; set; }

        public IEnumerable<Patient> Patients { get; set; }

        public IEnumerable<ConditionDiagnosis> ConditionDiagnoses { get; set; }
    }
}
