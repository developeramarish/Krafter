using Krafter.Shared.Common;
using Krafter.Shared.Common.Models;
using Krafter.Shared.Contracts.Users;
using Refit;

namespace Krafter.UI.Web.Client.Infrastructure.Refit;

public interface IUsersApi
{
    [Get($"/{KrafterRoute.Users}/get")]
    public Task<Response<PaginationResponse<UserDto>>> GetUsersAsync(
        [Query] GetRequestInput request,
        CancellationToken cancellationToken = default);

    [Get($"/{KrafterRoute.Users}/by-role/{{roleId}}")]
    public Task<Response<PaginationResponse<UserInfo>>> GetUsersByRoleAsync(
        string roleId,
        [Query] GetRequestInput request,
        CancellationToken cancellationToken = default);

    [Post($"/{KrafterRoute.Users}/create-or-update")]
    public Task<Response> CreateOrUpdateUserAsync([Body] CreateUserRequest request,
        CancellationToken cancellationToken = default);

    [Post($"/{KrafterRoute.Users}/delete")]
    public Task<Response> DeleteUserAsync([Body] DeleteRequestInput request,
        CancellationToken cancellationToken = default);

    [Get($"/{KrafterRoute.Users}/permissions")]
    public Task<Response<List<string>>> GetPermissionsAsync(CancellationToken cancellationToken = default);

    [Get($"/{KrafterRoute.Users}/get-roles/{{userId}}")]
    public Task<Response<List<UserRoleDto>>> GetUserRolesAsync(string userId,
        CancellationToken cancellationToken = default);

    [Post($"/{KrafterRoute.Users}/change-password")]
    public Task<Response> ChangePasswordAsync([Body] ChangePasswordRequest request,
        CancellationToken cancellationToken = default);

    [Post($"/{KrafterRoute.Users}/forgot-password")]
    public Task<Response> ForgotPasswordAsync([Body] ForgotPasswordRequest request,
        CancellationToken cancellationToken = default);

    [Post($"/{KrafterRoute.Users}/reset-password")]
    public Task<Response> ResetPasswordAsync([Body] ResetPasswordRequest request,
        CancellationToken cancellationToken = default);
}
