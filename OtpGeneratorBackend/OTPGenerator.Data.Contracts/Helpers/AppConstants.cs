namespace OTPGenerator.Data.Contracts.Helpers;
public static class AppConstants
{
    public const string VALID_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    public const int OTP_LENGTH = 6;
    public const int OTP_VALIDITY_IN_SECONDS = 60;
    public const int MAX_TRY_COUNT = 3;
}
