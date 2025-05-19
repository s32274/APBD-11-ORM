using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DefaultNamespace;

namespace Tutorial5.Models;

[Table("Doctor")]
public class Doctor
{
    [Key]
    public int IdDoctor { get; set; }
    
    [MaxLength(100)]
    public string FirstName { get; set; }
    
    [MaxLength(100)]
    public string LastName { get; set; }
    
    [MaxLength(100)]
    public string Email { get; set; }
    
    public ICollection<Prescription> prescriptions { get; set; }
}