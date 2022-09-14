using ChatWarden.CoreLib.Bot;
using ChatWarden.CoreLib.Bot.Queue;
using ChatWarden.CoreLib.Bot.UpdatesProcessing;
using ChatWarden.CoreLib.Bot.UpdatesProcessing.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;
using Telegram.Bot;

namespace ChatWarden.CoreLib.Extentions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHandler(this IServiceCollection services, string tarantoolConnectionString, string token)
        {
            services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token));
            var _box = new Box(new ClientOptions(tarantoolConnectionString));
            _box.Connect().Wait();
            services.AddSingleton(_box);
            services.AddSingleton<BotState>();
            services.AddSingleton<UsersRepository>();
            services.AddSingleton<MessagesRepository>();
            services.AddSingleton<ICustomUpdateHandler, CustomUpdateHandler>();

            services.AddSingleton<Publisher>();
            services.AddHostedService<Consumer>();
            services.AddHostedService<BotStarter>();
            return services;
        }
    }
}
