namespace Common.Requests.Identity;

public class ChangePasswordRequest
{
    public string UserId { get; set; }
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmedNewPassword { get; set; }
}