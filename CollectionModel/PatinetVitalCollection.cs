using E_Prescribing.Models;

namespace E_Prescribing.CollectionModel
{
    public class PatinetVitalCollection
    {
        public PatientVital PatientVital { get; set; }

        public IEnumerable<Patient> Patients { get; set; }

        public IEnumerable<VitalRange> Vitals { get; set; }
    }
}
