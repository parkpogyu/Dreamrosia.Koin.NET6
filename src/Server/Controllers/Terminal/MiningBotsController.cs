using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Shared.Constants.Permission;
using Dreamrosia.Koin.Shared.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Text;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Server.Controllers
{
    [Route("api/terminal/[controller]")]
    [ApiController]
    public class MiningBotsController : BaseApiController<MiningBotsController>
    {
        private readonly IMiningBotService _miningBotService;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public MiningBotsController(IMiningBotService miningBotService,
                                    IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _miningBotService = miningBotService;
            _localizer = localizer;
        }

        /// <summary>
        /// Get All MiningBots
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.MiningBots.View)]
        [HttpGet]
        public async Task<IActionResult> GetMiningBots()
        {
            var response = await _miningBotService.GetMiningBotsAsync();

            return Ok(response);
        }

        //[HttpGet("test")]
        //public async Task<IActionResult> GetTestMiningBots()
        //{
        //    var response = await _miningBotService.GetTestMiningBotsAsync();

        //    return Ok(response);
        //}

        [HttpGet("test")]
        public async Task<ContentResult> GetTestMiningBots()
        {
            var response = await _miningBotService.GetMiningBotsAsync();

            if (response.Succeeded)
            {
                StringBuilder builder = new StringBuilder();

                builder.AppendLine("<!DOCTYPE html>");
                builder.AppendLine("<html>");
                builder.AppendLine("<head>");
                builder.AppendLine("<meta charset='utf-8'>");
                builder.AppendLine("<meta lang = 'ko'/>");
                builder.AppendLine("<meta http-equiv='refresh' content='5'>");
                builder.AppendLine(@"
<style type ='text/css'>
    body {
    font-family:'Courier New';
    font-size:11pt;
}
</style>");
                builder.AppendLine("</head>");
                builder.AppendLine("<body>");
                builder.AppendLine(@$"
                 <table border = '1' align = 'center' width = '100%'>
                    <th>{_localizer["MiningBot.Ticket"]}</th>
                    <th>{_localizer["MiningBot.Id"]}</th>
                    <th>{_localizer["MiningBot.Version"]}</th>
                    <th>{_localizer["MiningBot.MachineName"]}</th>
                    <th>{_localizer["MiningBot.CurrentDirectory"]}</th>
                    <th>{_localizer["Users"]}</th>
                    <th>{_localizer["MiningBot.Touched"]}</th>
                    <th>{_localizer["MiningBot.Elapsed"]}</th>");

                foreach (var bot in response.Data)
                {
                    builder.AppendLine("<tr>");

                    builder.AppendLine("<td align='center'>");
                    builder.AppendLine($"{bot.Ticket}");
                    builder.AppendLine("</td>");
                    builder.AppendLine("<td align='center'>");
                    builder.AppendLine($"{bot.Id}");
                    builder.AppendLine("</td>");
                    builder.AppendLine("<td align='center'>");
                    builder.AppendLine($"{bot.Version}");
                    builder.AppendLine("</td>");
                    builder.AppendLine("<td align='center'>");
                    builder.AppendLine($"{bot.MachineName}");
                    builder.AppendLine("</td>");
                    builder.AppendLine("<td>");
                    builder.AppendLine($"{bot.CurrentDirectory}");
                    builder.AppendLine("</td>");
                    builder.AppendLine("<td>");
                    builder.AppendLine($"{bot.NickName}");
                    builder.AppendLine("</td>");
                    builder.AppendLine("<td align='center'>");
                    builder.AppendLine($"{bot.Touched:HH:mm:ss}");
                    builder.AppendLine("</td>");
                    builder.AppendLine("<td align='center'>");
                    builder.AppendLine($"{bot.Elapsed:dd\\,hh\\:mm\\:ss}");
                    builder.AppendLine("</td>");

                    builder.AppendLine("</tr>");
                }
                builder.AppendLine("</table>");
                builder.AppendLine("</body>");
                builder.AppendLine("</html>");

                return base.Content(builder.ToString(), "text/html");
            }
            else
            {
                return base.Content("");
            }
        }
    }
}