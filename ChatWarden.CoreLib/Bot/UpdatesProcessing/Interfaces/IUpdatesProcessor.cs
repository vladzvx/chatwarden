using Telegram.Bot.Types;

namespace ChatWarden.CoreLib.Bot.UpdatesProcessing.Interfaces
{
    public interface IUpdatesProcessor
    {
        public Task Process(Update update);
    }
}
