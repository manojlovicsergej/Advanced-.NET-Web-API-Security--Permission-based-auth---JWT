using Application.Services.Identity;
using Common.Requests;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Queries;

public class GetTokenQuery : IRequest<IResponseWrapper>
{
    public TokenRequest TokenRequest { get; set; }
}

public class GetTokenQueryHandler : IRequestHandler<GetTokenQuery, IResponseWrapper>
{
    private readonly ITokenService _tokenService;

    public GetTokenQueryHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }
    
    public async Task<IResponseWrapper> Handle(GetTokenQuery request, CancellationToken cancellationToken)
    {
        return await _tokenService.GetTokenAsync(request.TokenRequest);
    }
}