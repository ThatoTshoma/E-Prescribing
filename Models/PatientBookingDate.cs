namespace E_Prescribing.Models
{
    public class PatientBookingDate
    {
        public Patient Patient { get; set; }
        public Anaesthesiologist Anaesthesiologist { get; set; }
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; }

        public string Status { get; set; }
    }
}
