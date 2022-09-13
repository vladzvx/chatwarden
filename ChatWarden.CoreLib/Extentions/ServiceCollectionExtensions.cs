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
            services.AddBotClientIfNeed(token);
            services.AddBoxIfNeed(tarantoolConnectionString);

            services.AddSingleton<BotState>();
            services.AddSingleton<ICustomUpdateHandler, CustomUpdateHandler>();

            services.AddSingleton<Publisher>();
            services.AddHostedService<Consumer>();
            services.AddHostedService<BotStarter>();
            return services;
        }

        public static IServiceCollection AddWorker(this IServiceCollection services, string tarantoolConnectionString, string token)
        {
            services.AddBotClientIfNeed(token);
            services.AddBoxIfNeed(tarantoolConnectionString);

            services.AddSingleton<BotState>();
            services.AddSingleton<Consumer>();
            return services;
        }
        internal static IServiceCollection AddBoxIfNeed(this IServiceCollection services, string tarantoolConnectionString)
        {
            services.AddSingleton<Box>(pr =>
            {
                var box = pr.GetRequiredService<Box>();
                if (box == null)
                {
                    var _box = new Box(new ClientOptions(tarantoolConnectionString));
                    _box.Connect().Wait();
                    return _box;
                }
                else
                {
                    return box;
                }
            });
            return services;
        }

        internal static IServiceCollection AddBotClientIfNeed(this IServiceCollection services, string token)
        {
            var id = token.GetBotId();
            if (id.HasValue)
            {
                services.AddSingleton<ITelegramBotClient>(pr =>
                {
                    var client = pr.GetRequiredService<ITelegramBotClient>();
                    if (client.BotId == id)
                    {
                        return client;
                    }
                    else
                    {
                        return new TelegramBotClient(token);
                    }
                });
                return services;
            }
            else
            {
                throw new ArgumentException("Uncorrect bot token!");
            }
        }
    }
}
