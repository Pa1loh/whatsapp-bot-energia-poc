using System.Text;
using System.Text.Json;
using WhatsAppBot.Models;
using WhatsAppBot.Services.Interfaces;

namespace WhatsAppBot.Services;

public class WhatsAppService(HttpClient httpClient, IConfiguration configuration) : IWhatsAppService
{
    public async Task EnviarMensagemTextoAsync(string para, string mensagem, string? idMensagemRecebida = null)
    {
        object payload;
        
        if (!string.IsNullOrEmpty(idMensagemRecebida))
        {
            payload = new
            {
                messaging_product = "whatsapp",
                context = new { message_id = idMensagemRecebida },
                to = para,
                type = "text",
                text = new { body = mensagem }
            };
        }
        else
        {
            payload = new
            {
                messaging_product = "whatsapp",
                to = para,
                type = "text",
                text = new { body = mensagem }
            };
        }
        
        await EnviarMensagemAsync(payload);
    }

    public async Task EnviarMensagemComBotoesAsync(string para, string texto, BotaoResposta[] botoes, string? idMensagemRecebida = null)
    {
       var botoesFormatados = botoes.Select(b => new
        {
            type = "reply",
            reply = new { id = b.Id, title = b.Titulo }
        }).ToArray();

        object payload;
        
        if (!string.IsNullOrEmpty(idMensagemRecebida))
        {
            payload = new
            {
                messaging_product = "whatsapp",
                context = new { message_id = idMensagemRecebida },
                to = para,
                type = "interactive",
                interactive = new
                {
                    type = "button",
                    body = new { text = texto },
                    action = new
                    {
                        buttons = botoesFormatados
                    }
                }
            };
        }
        else
        {
            payload = new
            {
                messaging_product = "whatsapp",
                to = para,
                type = "interactive",
                interactive = new
                {
                    type = "button",
                    body = new { text = texto },
                    action = new
                    {
                        buttons = botoesFormatados
                    }
                }
            };
        }
        
        await EnviarMensagemAsync(payload);
    }
  
    private async Task EnviarMensagemAsync(object payload)
    {
        var idNumeroTelefone = configuration["WhatsApp:PhoneNumberId"];
        var url = $"https://graph.facebook.com/v19.0/{idNumeroTelefone}/messages";

        var jsonPayload = JsonSerializer.Serialize(payload);
        var conteudo = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var tokenAcesso = configuration["WhatsApp:AccessToken"];
        if (string.IsNullOrWhiteSpace(tokenAcesso))
        {
            throw new InvalidOperationException("O AccessToken do WhatsApp não está configurado. Configure o segredo corretamente.");
        }
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenAcesso}");

        var resposta = await httpClient.PostAsync(url, conteudo);
        if (!resposta.IsSuccessStatusCode)
        {
            var corpoErro = await resposta.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Erro ao enviar mensagem para a API do WhatsApp. Código: {resposta.StatusCode}. Detalhes: {corpoErro}");
        }
    }
}