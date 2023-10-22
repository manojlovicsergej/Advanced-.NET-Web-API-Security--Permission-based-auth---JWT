using Application.Services.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Roles.Queries;

public class GetRolesQuery : IRequest<IResponseWrapper>
{
    
}

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery,IResponseWrapper>
{
    private readonly IRoleService _roleService;

    public GetRolesQueryHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }
    
    public async Task<IResponseWrapper> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        return await _roleService.GetRolesAsync(cancellationToken);
    }
}