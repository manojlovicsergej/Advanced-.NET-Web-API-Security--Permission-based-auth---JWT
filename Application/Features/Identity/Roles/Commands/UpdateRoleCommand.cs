using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Roles.Commands;

public class UpdateRoleCommand : IRequest<IResponseWrapper>
{
    public UpdateRoleRequest UpdateRole { get; set; }
}

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, IResponseWrapper>
{
    private readonly IRoleService _roleService;

    public UpdateRoleCommandHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<IResponseWrapper> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        return await _roleService.UpdateRoleAsync(request.UpdateRole, cancellationToken);
    }
}