using Application.Services.Identity;
using Common.Requests;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Queries;

public class GetRefreshTokenQuery : IRequest<IResponseWrapper>
{
    public RefreshTokenRequest RefreshTokenRequest { get; set; }
}

public class GetRefreshTokenQueryHandler : IRequestHandler<GetRefreshTokenQuery, IResponseWrapper>
{
    private readonly ITokenService _tokenService;

    public GetRefreshTokenQueryHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }
    
    public async Task<IResponseWrapper> Handle(GetRefreshTokenQuery request, CancellationToken cancellationToken)
    {
        return await _tokenService.GetRefreshTokenAsync(request.RefreshTokenRequest);
    }
}