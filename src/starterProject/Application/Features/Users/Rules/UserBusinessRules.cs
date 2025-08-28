using Application.Features.Users.Constants;
using Application.Services.UsersService;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Rules;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.Types;
using NArchitectureTemplate.Core.Localization.Abstraction;
using NArchitectureTemplate.Core.Security.Hashing;

namespace Application.Features.Users.Rules;

public class UserBusinessRules : BaseBusinessRules
{
    private readonly IUserService _userService;
    private readonly ILocalizationService _localizationService;

    public UserBusinessRules(ILocalizationService localizationService, IUserService userService)
    {
        _localizationService = localizationService;
        _userService = userService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, UsersMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task UserShouldBeExistsWhenSelected(User? user)
    {
        if (user == null)
            await throwBusinessException(UsersMessages.UserDontExists);
    }

    public async Task UserIdShouldBeExistsWhenSelected(Guid id)
    {
        bool doesExist = await _userService.UserIdShouldBeExistsWhenSelected(id);
        if (doesExist)
            await throwBusinessException(UsersMessages.UserDontExists);
    }

    public async Task UserPasswordShouldBeMatched(User user, string password)
    {
        if (!HashingHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            await throwBusinessException(UsersMessages.PasswordDontMatch);
    }

    public async Task UserEmailShouldNotExistsWhenInsert(string email)
    {
        bool doesExists = await _userService.UserEmailShouldNotExistsWhenInsert(email);
        if (doesExists)
            await throwBusinessException(UsersMessages.UserMailAlreadyExists);
    }

    public async Task UserEmailShouldNotExistsWhenUpdate(Guid id, string email)
    {
        bool doesExists = await _userService.UserEmailShouldNotExistsWhenUpdate(id,email);
        if (doesExists)
            await throwBusinessException(UsersMessages.UserMailAlreadyExists);
    }
}
