namespace WhatsAppBot.Services.Interfaces;

public interface IChatbotService
{
    Task ProcessarMensagemAsync(string numeroTelefone, string tipoMensagem, string? textoMensagem = null, string? idBotaoClicado = null, string? idMensagem = null);
}