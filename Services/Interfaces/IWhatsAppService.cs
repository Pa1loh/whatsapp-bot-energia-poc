using WhatsAppBot.Models;

namespace WhatsAppBot.Services.Interfaces;

public interface IWhatsAppService
{
    Task EnviarMensagemTextoAsync(string para, string mensagem, string? idMensagemRecebida = null);

    Task EnviarMensagemComBotoesAsync(string para, string texto, BotaoResposta[] botoes, string? idMensagemRecebida = null);
}