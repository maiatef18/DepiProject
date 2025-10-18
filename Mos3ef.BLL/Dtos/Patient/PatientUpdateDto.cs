using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.BLL.Dtos.Patient
{
    public class PatientUpdateDto
    {
        // Add properties you want to allow the patient to update.
        // Based on PatientRegisterDto, 'Name' is a good candidate.
        // You can add more fields from your Patient.cs model here.
        public string Name { get; set; }

        // Example: public string PhoneNumber { get; set; }
    }
}
