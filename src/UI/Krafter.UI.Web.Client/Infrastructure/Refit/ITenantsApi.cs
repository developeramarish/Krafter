using Krafter.Shared.Common;
using Krafter.Shared.Common.Models;
using Krafter.Shared.Contracts.Tenants;
using Refit;

namespace Krafter.UI.Web.Client.Infrastructure.Refit;

public interface ITenantsApi
{
    [Get($"/{KrafterRoute.Tenants}/get")]
    public Task<Response<PaginationResponse<TenantDto>>> GetTenantsAsync(
        [Query] GetRequestInput request,
        CancellationToken cancellationToken = default);

    [Post($"/{KrafterRoute.Tenants}/create-or-update")]
    public Task<Response> CreateOrUpdateTenantAsync([Body] CreateOrUpdateTenantRequest request,
        CancellationToken cancellationToken = default);

    [Post($"/{KrafterRoute.Tenants}/delete")]
    public Task<Response> DeleteTenantAsync([Body] DeleteRequestInput request,
        CancellationToken cancellationToken = default);

    [Post($"/{KrafterRoute.Tenants}/seed-data")]
    public Task<Response> SeedDataAsync([Body] SeedDataRequest request, CancellationToken cancellationToken = default);
}
