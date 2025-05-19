namespace DefaultNamespace;

[Table("Patient")]
public class Patient
{
    [Key]
    public int IdPatient { get; set; }
    
    [MaxLength(100)]
    public string FirstName { get; set; }
    
    [MaxLength(100)]
    public string LastName { get; set; }
    
    public date BirthDate { get; set; }
}