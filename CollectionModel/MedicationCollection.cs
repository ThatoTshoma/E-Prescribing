using E_Prescribing.Models;

namespace E_Prescribing.CollectionModel
{
    public class MedicationCollection
    {
        public MedicationIngredient MedicationIngredient { get; set; }
        public Medication Medication { get; set; }
        public IEnumerable<Medication> Medications { get; set; }
        public Dictionary<int, string> Strengths { get; set; }

        public MedicationCollection()
        {
            MedicationIngredient = new MedicationIngredient(); 
        }


    }
}
