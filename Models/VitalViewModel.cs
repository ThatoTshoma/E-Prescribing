namespace E_Prescribing.Models
{
    public class VitalViewModel
    {
        public Patient Patient { get; set; }

        public IEnumerable<Vital> Vitals { get; set; }
    }
}
