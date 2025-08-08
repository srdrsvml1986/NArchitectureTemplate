using NArchitecture.Core.Application.Dtos;
using static Domain.Entities.User;

namespace Application.Features.Users.Queries.GetList;

public class GetListUserListItemDto : IDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public UserStatus Status { get; set; }
    public string? PhotoURL { get; set; }
    public string? Gender { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Notes { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }

    public GetListUserListItemDto()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
    }

    public GetListUserListItemDto(Guid id, string firstName, string lastName, string email, UserStatus status)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Status = status;
    }

    public GetListUserListItemDto(Guid ýd, string firstName, string lastName, string email, UserStatus status, string? photoURL, string? gender, DateTime? birthDate, DateTime createdDate, DateTime? updatedDate, DateTime? deletedDate) : this(ýd, firstName, lastName, email, status)
    {
        PhotoURL = photoURL;
        Gender = gender;
        BirthDate = birthDate;
        CreatedDate = createdDate;
        UpdatedDate = updatedDate;
        DeletedDate = deletedDate;
    }
}
