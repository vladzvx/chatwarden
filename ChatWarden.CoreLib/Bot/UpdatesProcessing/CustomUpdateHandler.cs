using ChatWarden.CoreLib.Bot.UpdatesProcessing.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ChatWarden.CoreLib.Bot.UpdatesProcessing
{
    public class CustomUpdateHandler : ICustomUpdateHandler
    {
        private readonly IUpdatesProcessor _updatesProcessor;
        public CustomUpdateHandler(IUpdatesProcessor updatesProcessor)
        {
            _updatesProcessor = updatesProcessor;
        }

        public ReceiverOptions ReceiverOptions { get; private set; } = new ReceiverOptions()
        {
            AllowedUpdates = new UpdateType[] { UpdateType.Message }
        };

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                await _updatesProcessor.Process(update);
            }
            catch
            {

            }
        }
    }
}
