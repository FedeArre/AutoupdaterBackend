using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace MVC.DiscordBot
{
    // 
    public class BotHandler
    {   
        private IConfiguration Configuration;
        
        public BotHandler(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task MainAsync()
        {
            string token = Configuration["Token"];
            DiscordSocketClient _client = new DiscordSocketClient();
            
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }
    }
}
