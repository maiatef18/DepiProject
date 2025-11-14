using System.ComponentModel.DataAnnotations;

namespace Mos3ef.BLL.Dtos.Compare
{
    public class CompareRequestDto
    {
        [Required(ErrorMessage = "Service1Id is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Service1Id must be greater than 0.")]
        public int Service1Id { get; set; }

        [Required(ErrorMessage = "Service2Id is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Service2Id must be greater than 0.")]
        public int Service2Id { get; set; }
    }
}
