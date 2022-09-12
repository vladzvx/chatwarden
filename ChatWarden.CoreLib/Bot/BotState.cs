using ProGaudi.Tarantool.Client;
using System.Collections.Concurrent;
using static ChatWarden.CoreLib.Bot.ChatState;

namespace ChatWarden.CoreLib.Bot
{
    public class BotState
    {
        private readonly Box _box;
        private readonly ConcurrentDictionary<StateId, ChatState> _chatsStates = new();
        public BotState(Box box)
        {
            _box = box;
        }

        public async Task<ChatState> GetChatState(long botId, long chatId)
        {
            var id = new StateId(botId, chatId);
            ChatState state;
            if (_chatsStates.TryGetValue(id, out var tmp))
            {
                state = tmp;
            }
            else
            {
                state = new ChatState(_box, botId, chatId);
                try
                {
                    _chatsStates.TryAdd(state.Id, state);
                    state = _chatsStates[state.Id];
                    await state.AddChat(Constants.Defaults.DefaultHelp);
                }
                catch { }
            }
            return state;
        }
    }
}
