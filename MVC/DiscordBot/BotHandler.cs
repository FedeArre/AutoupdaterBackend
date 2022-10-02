using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Objects;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MVC.DiscordBot
{
    // 
    public class BotHandler
    {   
        public IConfiguration Configuration;

        private Timer _timer;
        private DiscordSocketClient _client;
        public async Task MainAsync()
        {
            string token = Configuration["Token"];
            _client = new DiscordSocketClient();
            
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _client.Ready += OnBotLogin;

            await Task.Delay(-1);
        }

        private async Task OnBotLogin()
        {
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(20);

            _timer = new System.Threading.Timer((e) =>
            {
                Task.Run(UpdatePlayerCountChannel);
            }, null, startTimeSpan, periodTimeSpan);
        }

        public async Task UpdatePlayerCountChannel()
        {
            await _client.SetActivityAsync(new Game($"{TelemetryHandler.GetInstance().GetCurrentPlayerCount("ModUtils")} players using ModUtils", ActivityType.Watching, ActivityProperties.None));
        }

        // Singleton
        private static BotHandler Instance;
        public static BotHandler GetInstance()
        {
            if (Instance == null)
                Instance = new BotHandler();
            return Instance;
        }
        private BotHandler()
        {

        }
    }
}
