using Common.Requests.Identity;
using Common.Responses.Wrappers;

namespace Application.Services.Identity;

public interface IRoleService
{
    Task<IResponseWrapper> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken);
    Task<IResponseWrapper> GetRolesAsync(CancellationToken cancellationToken);
    Task<IResponseWrapper> UpdateRoleAsync(UpdateRoleRequest request,CancellationToken cancellationToken);
    Task<IResponseWrapper> GetRoleByIdAsync(string roleId,CancellationToken cancellationToken);
    Task<IResponseWrapper> DeleteRoleAsync(string roleId,CancellationToken cancellationToken);
}