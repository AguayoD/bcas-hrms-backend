
using System.ComponentModel.DataAnnotations;

namespace Models.Models
{
    public class tblEmployees
    {
        [Key]
        public int? EmployeeID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public int? PositionID { get; set; }
        public int? DepartmentID { get; set; }
        public string? EmploymentStatus { get; set; }
        public DateTime? HireDate { get; set; }
    }
}
