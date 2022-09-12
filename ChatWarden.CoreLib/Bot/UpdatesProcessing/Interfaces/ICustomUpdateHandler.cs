using Telegram.Bot.Extensions.Polling;

namespace ChatWarden.CoreLib.Bot.UpdatesProcessing.Interfaces
{
    public interface ICustomUpdateHandler : IUpdateHandler
    {
        public ReceiverOptions ReceiverOptions { get; }
    }
}
