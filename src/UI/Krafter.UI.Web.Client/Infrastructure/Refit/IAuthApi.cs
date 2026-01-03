using Krafter.Shared.Common;
using Krafter.Shared.Common.Models;
using Krafter.Shared.Contracts.Auth;
using Refit;

namespace Krafter.UI.Web.Client.Infrastructure.Refit;

public interface IAuthApi
{
    [Post($"/{KrafterRoute.Tokens}/create")]
    public Task<Response<TokenResponse>> CreateTokenAsync([Body] TokenRequest request,
        CancellationToken cancellationToken = default);

    [Post($"/{KrafterRoute.Tokens}/refresh")]
    public Task<Response<TokenResponse>> RefreshTokenAsync([Body] RefreshTokenRequest request,
        CancellationToken cancellationToken = default);

    [Post($"/{KrafterRoute.ExternalAuth}/google")]
    public Task<Response<TokenResponse>> GoogleAuthAsync([Body] GoogleAuthRequest request,
        CancellationToken cancellationToken = default);

    [Post($"/{KrafterRoute.Tokens}/logout")]
    public Task LogoutAsync(CancellationToken cancellationToken = default);
}
