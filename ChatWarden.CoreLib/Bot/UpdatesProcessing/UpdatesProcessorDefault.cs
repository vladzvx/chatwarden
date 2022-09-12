using ChatWarden.CoreLib.Bot.UpdatesProcessing.Interfaces;
using Telegram.Bot.Types;

namespace ChatWarden.CoreLib.Bot.UpdatesProcessing
{
    public class UpdatesProcessorDefault : IUpdatesProcessor
    {
        public async Task Process(Update update)
        {
            await Task.Delay(0);
        }
    }
}
