using DefaultNamespace;

namespace Tutorial5.DTOs;

public class PatientWithPrescriptonsDto
{
    public int IdPatient { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public List<Prescription> PrescriptionWithMedicamentsDto { get; set; }
}

public class PrescriptionWithMedicamentsDto
{
    public int IdPrescription { get; set; }
    
    public DateTime Date { get; set; }
    
    public DateTime DueDate { get; set; }
    
    public DoctorDto Doctor { get; set; }
    
    public List<MedicamentDto> Medicaments { get; set; }
}

public class DoctorDto
{
    public int IdDoctor { get; set; }
    
    public string FirstName { get; set; }
}

public class MedicamentDto
{
    public int IdMedicament { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
}