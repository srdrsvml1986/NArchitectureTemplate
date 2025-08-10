namespace Application.Features.Auth.Constants;

public static class AuthMessages
{
    public const string SectionName = "Auth";

    public const string EmailAuthenticatorDontExists = "EmailAuthenticatorDontExists";
    public const string OtpAuthenticatorDontExists = "OtpAuthenticatorDontExists";
    public const string AlreadyVerifiedOtpAuthenticatorIsExists = "AlreadyVerifiedOtpAuthenticatorIsExists";
    public const string EmailActivationKeyDontExists = "EmailActivationKeyDontExists";
    public const string UserDontExists = "UserDontExists";
    public const string UserHaveAlreadyAuthenticator = "UserHaveAlreadyAuthenticator";
    public const string RefreshDontExists = "RefreshDontExists";
    public const string InvalidRefreshToken = "InvalidRefreshToken";
    public const string UserMailAlreadyExists = "UserMailAlreadyExists";
    public const string PasswordDontMatch = "PasswordDontMatch";
    public const string UserStatusUnverified = "UserStatusUnverified";
    public const string UserStatusInactive = "UserStatusInactive";
    public const string UserStatusSuspended = "UserStatusSuspended";
    public const string UserStatusDeleted = "UserStatusDeleted";
    public const string UserStatusOther = "UserStatusOther";
}
