using Krafter.Shared.Common;
using Krafter.Shared.Common.Models;
using Refit;

namespace Krafter.UI.Web.Client.Infrastructure.Refit;

public interface IAppInfoApi
{
    [Get($"/{KrafterRoute.AppInfo}")]
    public Task<Response<string>> GetAppInfoAsync(CancellationToken cancellationToken = default);
}
