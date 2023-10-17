namespace Application.Services.Identity;

public interface ICurrentUserService
{
    public string UserId { get; set; }
}