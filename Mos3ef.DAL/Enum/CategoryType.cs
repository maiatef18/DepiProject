using System.ComponentModel.DataAnnotations;

namespace Mos3ef.DAL.Enum
{
    public enum CategoryType
    {
        [Display(Name = "غرفة الطوارئ")]
        EmergencyRoom = 1,

        [Display(Name = "العناية المركزة")]
        ICU = 2,

        [Display(Name = "قسم الأطفال حديثي الولادة (الحضانة)")]
        NICU = 17,

        [Display(Name = "بنك الدم")]
        BloodBank = 18,

        [Display(Name = "غرفة العمليات")]
        OperationTheater = 3,

        [Display(Name = "الجناح العام")]
        GeneralWard = 4,

        [Display(Name = "غرفة خاصة")]
        PrivateRoom = 5,

        [Display(Name = "قسم الولادة")]
        MaternityWard = 6,

        [Display(Name = "قسم الأطفال")]
        PediatricWard = 7,

        [Display(Name = "الأشعة")]
        Radiology = 8,

        [Display(Name = "المعمل")]
        Laboratory = 9,

        [Display(Name = "الصيدلية")]
        Pharmacy = 10,

        [Display(Name = "العيادات الخارجية")]
        OutpatientClinic = 11,

        [Display(Name = "خدمة الإسعاف")]
        AmbulanceService = 12,

        [Display(Name = "التأهيل")]
        Rehabilitation = 13,

        [Display(Name = "عيادة الأسنان")]
        DentalClinic = 14,

        [Display(Name = "وحدة القلب")]
        CardiologyUnit = 15,

        [Display(Name = "وحدة الغسيل الكلوي")]
        DialysisUnit = 16
    }
}
