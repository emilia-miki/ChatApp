using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChatApp.Shared;

public class HubConnectionFactory
{
    private readonly NavigationManager _navigationManager;
    private readonly string _token;

    public HubConnectionFactory(
        NavigationManager navigationManager, 
        string token)
    { 
        _navigationManager = navigationManager;
        _token = token;
    }

    public HubConnection GetConnection()
    {
        return new HubConnectionBuilder()
            .WithUrl(_navigationManager.ToAbsoluteUri("/hub"), 
                options =>
                {
                    options.Headers.Add("Authorization", 
                        new AuthenticationHeaderValue(
                            "Bearer", _token).ToString());
                })
            .Build();
    }
}