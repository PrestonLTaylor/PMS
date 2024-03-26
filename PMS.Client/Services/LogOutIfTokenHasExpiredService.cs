using Microsoft.Extensions.Hosting;
using PMS.Client.Views;
using PMS.Lib.Services;
using Shiny.Jobs;

namespace PMS.Client.Services;

public sealed class LogOutIfTokenHasExpiredJob(TokenService _tokenService) 
    :
#if WINDOWS
    IHostedService
#else
    IJob
#endif
{
#if WINDOWS
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _hostedServiceTimer = new Timer((_) => { IfTokenHasExpiredLogOutAsync(); }, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
#else
    public async Task Run(JobInfo jobInfo, CancellationToken cancelToken)
    {
        await IfTokenHasExpiredLogOutAsync();
    }
#endif

    private void IfTokenHasExpiredLogOutAsync()
    {
        if (!_tokenService.HasToken()) return;

        if (_tokenService.HasTokenExpired())
        {
            LogOutUser();
        }
    }

    private void LogOutUser()
    {
        // NOTE: We need to run GoToAsync on the main thread as otherwise we'll cause race-conditions and exceptions
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Shell.Current.GoToAsync($"///{nameof(Login)}");
        });
    }

#if WINDOWS
    private Timer _hostedServiceTimer; 
#endif
}
