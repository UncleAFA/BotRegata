using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using BotRegata.Models;

namespace BotRegata.Controllers
{
    [Route("api/message/update")]
    public class MessageController : Controller
    {
        // GET api/valuesd
        [HttpGet]
        public string Get()
        {
            return "Method GET unuvalable";
        }

        // POST api/values
        [HttpPost]
        public async Task<OkResult> Post([FromBody] object update)
        {
            var upd = JsonConvert.DeserializeObject<Update>(update.ToString());
            if (upd == null) return Ok();
            if (upd.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var commands = Bot.Commands;
                var message = upd.Message;
                var botClient = await Bot.GetBotClientAsync();

                foreach (var command in commands)
                {
                    if (command.Contains(message))
                    {
                        await command.Execute(message, botClient);
                        break;
                    }
                }
            }

            return Ok();
        }
    }
}