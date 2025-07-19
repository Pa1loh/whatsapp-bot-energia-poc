using Microsoft.AspNetCore.Mvc;
using WhatsAppBot.Models;
using WhatsAppBot.Services.Interfaces;

namespace WhatsAppBot.Controllers;

[ApiController]
[Route("api/webhook")]
public class WebhookController(
    IChatbotService chatbotService,
    IConfiguration configuration,
    ILogger<WebhookController> logger) : ControllerBase
{
    [HttpGet]
    public IActionResult VerifyWebhook([FromQuery(Name = "hub.mode")] string mode,
        [FromQuery(Name = "hub.challenge")] string challenge,
        [FromQuery(Name = "hub.verify_token")] string token)
    {
        var verifyToken = configuration["WhatsApp:VerifyToken"];
        if (mode == "subscribe" && token == verifyToken)
        {
            logger.LogInformation("Webhook verificado com sucesso.");
            return Ok(challenge);
        }

        logger.LogWarning("Falha na verificação do webhook.");
        return Forbid();
    }

    [HttpPost]
    public async Task<IActionResult> ProcessWebhook([FromBody] WhatsAppWebhookPayload payload)
    {
        try
        {
            var change = payload.Entry?.FirstOrDefault()?.Changes?.FirstOrDefault();
            
            if (change == null)
                return Ok();

            var message = change.Value?.Messages?.FirstOrDefault();
            
            if (message == null)
                return Ok();

            var numeroTelefone = message.From;
            var tipoMensagem = message.Type;
            var idMensagem = message.Id;
            
            string? textoMensagem = null;
            string? idBotaoClicado = null;

            if (tipoMensagem == "text" && message.Text != null)
                textoMensagem = message.Text.Body;
            
            if (tipoMensagem == "interactive" && message.Interactive?.ButtonReply != null)
                idBotaoClicado = message.Interactive.ButtonReply.Id;

            await chatbotService.ProcessarMensagemAsync(numeroTelefone, tipoMensagem, textoMensagem, idBotaoClicado, idMensagem);
            
            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao processar webhook");
            return StatusCode(500);
        }
    }
}