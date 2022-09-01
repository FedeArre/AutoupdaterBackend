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
        private IGuild _server;
        private IGuildChannel _playerCountChannel;
        
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
            _server = _client.GetGuild(ulong.Parse(Configuration["modUtils_serverId"]));
            _playerCountChannel = await _server.GetChannelAsync(ulong.Parse(Configuration["currentPlayersChannelId"]));

            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(30);

            _timer = new System.Threading.Timer((e) =>
            {
                Task.Run(UpdatePlayerCountChannel);
            }, null, startTimeSpan, periodTimeSpan);
        }

        public async Task UpdatePlayerCountChannel()
        {
            if(_server == null)
            {
                _server = _client.GetGuild(ulong.Parse(Configuration["modUtils_serverId"]));
            }

            if(_playerCountChannel == null)
            {
                _playerCountChannel = await _server.GetChannelAsync(ulong.Parse(Configuration["currentPlayersChannelId"]));
            }

            await _playerCountChannel.ModifyAsync(x => x.Name = $"Current players: {TelemetryHandler.GetInstance().GetCurrentPlayerCount("ModUtils")}");
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
