using Microsoft.EntityFrameworkCore;
using Tutorial5.Data;
using Tutorial5.DTOs;

namespace Tutorial5.Services;

public class DbService : IDbService
{
    private readonly PatientWardContext _context;
    public DbService(PatientWardContext context)
    {
        _context = context;
    }
    
    public async Task<List<PatientWithPrescriptonsDto>> GetPatientByIdAsync()
    {
        var books = await _context.Books.Select(e =>
        new PatientWithPrescriptonsDto {
            Name = e.Name,
            Price = e.Price,
            Authors = e.BookAuthors.Select(a =>
            new AuthorDto {
                FirstName = a.Author.FirstName,
                LastName = a.Author.LastName
            }).ToList()
        }).ToListAsync();
        return books;
    }
}