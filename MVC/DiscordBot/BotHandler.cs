using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Objects;
using Objects.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MVC.DiscordBot
{
    // 
    public class BotHandler
    {   
        public IConfiguration Configuration;
        public IServiceScope Services;
        
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
            SendDiscordMessage("Bot logged in");
        }
        
        public void SendDiscordMessage(string msg)
        {
            _client.GetGuild(873306861594640384).GetTextChannel(1030967349127413770).SendMessageAsync(msg);
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
