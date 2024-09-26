using E_Prescribing.Models;

namespace E_Prescribing.CollectionModel
{
    public class PatientTreatmentCollection
    {
        public PatientTreatment PatientTreatment { get; set; }

        public IEnumerable<Patient> Patients { get; set; }

        public IEnumerable<Treatment> Treatments { get; set; }
    }
}
