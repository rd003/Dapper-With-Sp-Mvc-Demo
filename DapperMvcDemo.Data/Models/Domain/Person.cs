

using System.ComponentModel.DataAnnotations;

namespace DapperMvcDemo.Data.Models.Domain;

public class Person
{
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }
    [Required]
    public string? Email { get; set; }
    public string? Address { get; set; }
}
