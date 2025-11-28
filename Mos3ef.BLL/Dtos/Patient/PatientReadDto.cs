namespace Mos3ef.BLL.Dtos.Patient
{
    public class PatientReadDto
    {
        public int PatientId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? ImageUrl { get; set; }
        public string? Location { get; set; }
        public string? Address { get; set; }
    }
}