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
        
        private Timer _timer;
        private Timer _timer24;
        
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
            
            var startTimeSpan = TimeSpan.Zero;
            var period24TimeSpan = TimeSpan.FromHours(24);
            
            if(_timer24 == null)
            {
                _timer24 = new Timer((e) =>
                {
                    Task.Run(CheckRecords);
                }, null, startTimeSpan, period24TimeSpan);
            }
        }
        
        public async Task CheckRecords()
        {
            Dictionary<string, int> records = new Dictionary<string, int>();

            IModRepository repo = Services.ServiceProvider.GetRequiredService<IModRepository>();
            IEnumerable<Mod> allMods = repo.FindAll();
            foreach (Mod m in allMods)
            {
                if (!TelemetryHandler.GetInstance().Peak24.ContainsKey(m.ModId))
                    continue;

                int record = TelemetryHandler.GetInstance().Peak24[m.ModId];
                if (record > m.PeakMax)
                {
                    records.Add(m.ModId, record);
                    m.PeakMax = record;
                    repo.Update(m);
                }
            }

            if(records.Count > 0)
            {
                string s = "New player count record on the following mods:";
                foreach (KeyValuePair<string, int> kvp in records)
                {
                    s += $"\n{kvp.Key} - {kvp.Value} players";
                }
                
                _client.GetGuild(873306861594640384).GetTextChannel(1030967349127413770).SendMessageAsync(s);
            }

            TelemetryHandler.GetInstance().Peak24.Clear();
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
