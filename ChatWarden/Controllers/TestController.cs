using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProGaudi.Tarantool.Client;

namespace ChatWarden.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly Box _box;
        public TestController(Box box)
        {
            _box = box;
        }
        [HttpGet]
        public string Web()
        {
            return "Ok!";
        }

        [HttpGet]
        public async Task<string> Tarantool()
        {
            var tmp = await _box.Call<string>("test");
            return tmp.Data[0];
        }
    }
}
