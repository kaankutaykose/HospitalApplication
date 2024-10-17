using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace HospitalApplication.Models
{
    public class BookAppointmentViewModel
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime? AppointmentDate { get; set; }
        public List<SelectListItem> Doctors { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Professions { get; set; } = new List<SelectListItem>();
    }

}
