using NArchitectureTemplate.Core.Application.Responses;
using static Domain.Entities.User;

namespace Application.Features.Users.Queries.GetById;

public class GetByIdUserResponse : IResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public UserStatus Status { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? BirthDate { get; set; } // Yeni alan
    public bool? Gender { get; set; } // Yeni alan

    public GetByIdUserResponse()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        PhoneNumber = string.Empty;
    }
    public GetByIdUserResponse(Guid id, string firstName, string lastName, string email, UserStatus status, string phoneNumber, DateTime createdAt, DateTime updatedAt, DateTime? birthDate, bool? gender)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Status = status;
        PhoneNumber = phoneNumber;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        BirthDate = birthDate; // Yeni alan
        Gender = gender; // Yeni alan
    }
}
