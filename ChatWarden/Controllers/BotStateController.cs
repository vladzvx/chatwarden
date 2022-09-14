using ChatWarden.CoreLib.Bot;
using Microsoft.AspNetCore.Mvc;

namespace ChatWarden.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class BotStateController : ControllerBase
    {
        private readonly BotState _state;
        private readonly string? _pwd;
        public BotStateController(BotState state)
        {
            _state = state;
            _pwd = Environment.GetEnvironmentVariable("STATE_PASSWORD");
        }

        [HttpPost]
        public async Task AddBanReplic([FromQuery] string text, [FromQuery] string pwd)
        {
            if (pwd == _pwd)
            {
                await _state.AddBanReplic(text);
            }
        }

        [HttpPost]
        public async Task AddMediaReplic([FromQuery] string text, [FromQuery] string pwd)
        {
            if (pwd == _pwd)
            {
                await _state.AddMediaReplic(text);
            }
        }

        [HttpPost]
        public async Task AddRestrictReplic([FromQuery] string text, [FromQuery] string pwd)
        {
            if (pwd == _pwd)
            {
                await _state.AddRestrictReplic(text);
            }
        }

    }
}
