namespace DefaultNamespace;

[Table("Prescription")]
public class Prescription
{
    [Key] 
    public int IdPrescription { get; set; }
    
    public Date Date { get; set; }
    
    public DueDate Date { get; set; }
    
    [ForeignKey(nameof(Doctor))]
    public int IdDoctor { get; set; }
    
    [ForeignKey(nameof(Patient))]
    public int IdPatient { get; set; }
    
    public Doctor Doctor { get; set; }
    public Patient Patient { get; set; }
}