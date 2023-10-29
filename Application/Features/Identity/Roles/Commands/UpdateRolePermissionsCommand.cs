using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Roles.Commands;

public class UpdateRolePermissionsCommand : IRequest<IResponseWrapper>
{
    public UpdateRolePermissionsRequest UpdateRolePermissionsRequest { get; set; }
}

public class UpdateRolePermissionsCommandHandler : IRequestHandler<UpdateRolePermissionsCommand, IResponseWrapper>
{
    private readonly IRoleService _roleService;

    public UpdateRolePermissionsCommandHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }
    
    public async Task<IResponseWrapper> Handle(UpdateRolePermissionsCommand request, CancellationToken cancellationToken)
    {
        return await _roleService.UpdateRolePermissionsAsync(request.UpdateRolePermissionsRequest, cancellationToken);
    }
}