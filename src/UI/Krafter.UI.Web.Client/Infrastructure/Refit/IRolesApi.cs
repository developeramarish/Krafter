using Krafter.Shared.Common;
using Krafter.Shared.Common.Models;
using Krafter.Shared.Contracts.Roles;
using Refit;

namespace Krafter.UI.Web.Client.Infrastructure.Refit;

public interface IRolesApi
{
    [Get($"/{KrafterRoute.Roles}/get")]
    public Task<Response<PaginationResponse<RoleDto>>> GetRolesAsync(
        [Query] GetRequestInput request,
        CancellationToken cancellationToken = default);

    [Post($"/{KrafterRoute.Roles}/create-or-update")]
    public Task<Response> CreateOrUpdateRoleAsync([Body] CreateOrUpdateRoleRequest request,
        CancellationToken cancellationToken = default);

    [Post($"/{KrafterRoute.Roles}/delete")]
    public Task<Response> DeleteRoleAsync([Body] DeleteRequestInput request,
        CancellationToken cancellationToken = default);

    [Get($"/{KrafterRoute.Roles}/get-by-id-with-permissions/{{roleId}}")]
    public Task<Response<RoleDto>> GetRolePermissionsAsync(string roleId,
        CancellationToken cancellationToken = default);

    [Put($"/{KrafterRoute.Roles}/update-permissions")]
    public Task<Response> UpdateRolePermissionsAsync([Body] UpdateRolePermissionsRequest request,
        CancellationToken cancellationToken = default);
}
