using Common.Requests.Identity;
using Common.Responses.Wrappers;

namespace Application.Services.Identity;

public interface IUserService
{
    Task<IResponseWrapper> RegisterUserAsync(UserRegistrationRequest request, CancellationToken cancellationToken);
    Task<IResponseWrapper> GetUserByIdAsync(string userId);
}