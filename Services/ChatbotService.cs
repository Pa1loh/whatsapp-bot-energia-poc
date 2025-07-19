using WhatsAppBot.Models;
using WhatsAppBot.Helpers;
using WhatsAppBot.Services.Interfaces;

namespace WhatsAppBot.Services;

public class ChatbotService(
    IWhatsAppService whatsAppService,
    ILogger<ChatbotService> logger,
    GerenciadorConversa gerenciadorConversa) : IChatbotService
{
    public async Task ProcessarMensagemAsync(string numeroTelefone, string tipoMensagem, string? textoMensagem = null, string? idBotaoClicado = null, string? idMensagem = null)
    {
        try
        {
            var numeroFormatado = FormatadorTelefone.FormatarNumeroBrasileiro(numeroTelefone);
            var estadoAtual = gerenciadorConversa.ObterEstadoConversa(numeroFormatado);

            if (tipoMensagem == "interactive" && !string.IsNullOrEmpty(idBotaoClicado))
            {
                await ProcessarInteracaoBotaoAsync(numeroFormatado, idBotaoClicado, estadoAtual, idMensagem);
                return;
            }

            if (tipoMensagem == "text" && !string.IsNullOrEmpty(textoMensagem))
            {
                await ProcessarRespostaTextoAsync(numeroFormatado, textoMensagem, estadoAtual, idMensagem);
                return;
            }

            await EnviarMensagemPadraoAsync(numeroFormatado, idMensagem);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao processar mensagem para {NumeroTelefone}", numeroTelefone);
            throw;
        }
    }
  
    private async Task ProcessarInteracaoBotaoAsync(string numeroTelefone, string idBotaoClicado, EstadoFluxoProposta estadoAtual, string? idMensagem = null)
    {
        switch (idBotaoClicado)
        {
            case "iniciar_proposta":
                gerenciadorConversa.DefinirEstadoConversa(numeroTelefone, EstadoFluxoProposta.AguardandoNomeCliente);
                await SolicitarNomeClienteAsync(numeroTelefone, idMensagem);
                break;
            
            case "cancelar":
                gerenciadorConversa.LimparDadosConversa(numeroTelefone);
                await EnviarMensagemCancelamentoAsync(numeroTelefone, idMensagem);
                break;

            case "colonial":
            case "fibrocimento":
            case "laje":
            case "ondulada":
            case "solo":
                if (estadoAtual == EstadoFluxoProposta.AguardandoTipoTelhado)
                {
                    var dadosProposta = gerenciadorConversa.ObterDadosProposta(numeroTelefone);
                    dadosProposta.TipoTelhado = idBotaoClicado;
                    gerenciadorConversa.DefinirEstadoConversa(numeroTelefone, EstadoFluxoProposta.AguardandoConfirmacao);
                    await EnviarConfirmacaoPropostaAsync(numeroTelefone, idMensagem);
                }
                break;
                
            case "confirmar":
                if (estadoAtual == EstadoFluxoProposta.AguardandoConfirmacao)
                {
                    gerenciadorConversa.DefinirEstadoConversa(numeroTelefone, EstadoFluxoProposta.PropostaFinalizada);
                    await EnviarMensagemPropostaProcessadaAsync(numeroTelefone, idMensagem);
                }
                break;
            
            default:
                await EnviarOpcoesIniciaisAsync(numeroTelefone, idMensagem);
                break;
        }
    }

    private async Task ProcessarRespostaTextoAsync(string numeroTelefone, string textoMensagem, EstadoFluxoProposta estadoAtual, string? idMensagem = null)
    {
        var dadosProposta = gerenciadorConversa.ObterDadosProposta(numeroTelefone);

        switch (estadoAtual)
        {
            case EstadoFluxoProposta.AguardandoNomeCliente:
                dadosProposta.NomeCliente = textoMensagem;
                gerenciadorConversa.DefinirEstadoConversa(numeroTelefone, EstadoFluxoProposta.AguardandoGeracaoMensalKwh);
                await SolicitarGeracaoMensalAsync(numeroTelefone, idMensagem);
                break;

            case EstadoFluxoProposta.AguardandoGeracaoMensalKwh:
                dadosProposta.GeracaoMensalKwh = textoMensagem;
                gerenciadorConversa.DefinirEstadoConversa(numeroTelefone, EstadoFluxoProposta.AguardandoEstado);
                await SolicitarEstadoAsync(numeroTelefone, idMensagem);
                break;

            case EstadoFluxoProposta.AguardandoEstado:
                dadosProposta.Estado = textoMensagem;
                gerenciadorConversa.DefinirEstadoConversa(numeroTelefone, EstadoFluxoProposta.AguardandoCidade);
                await SolicitarCidadeAsync(numeroTelefone, idMensagem);
                break;

            case EstadoFluxoProposta.AguardandoCidade:
                dadosProposta.Cidade = textoMensagem;
                gerenciadorConversa.DefinirEstadoConversa(numeroTelefone, EstadoFluxoProposta.AguardandoTipoTelhado);
                await SolicitarTipoTelhadoAsync(numeroTelefone, idMensagem);
                break;

            case EstadoFluxoProposta.Inicio:
                await EnviarOpcoesIniciaisAsync(numeroTelefone, idMensagem);
                break;

            default:
                await EnviarOpcoesIniciaisAsync(numeroTelefone, idMensagem);
                break;
        }
    }

    private async Task EnviarOpcoesIniciaisAsync(string numeroTelefone, string? idMensagem = null)
    {
        await whatsAppService.EnviarMensagemComBotoesAsync(
            numeroTelefone, 
            "Olá! O que você deseja fazer?",
            new[] {
                new BotaoResposta("iniciar_proposta", "Iniciar Proposta"),
                new BotaoResposta("cancelar", "Cancelar")
            },
            idMensagem);
    }

    private async Task EnviarMensagemCancelamentoAsync(string numeroTelefone, string? idMensagem = null)
    {
        await whatsAppService.EnviarMensagemTextoAsync(numeroTelefone, "Volte sempre! 😊", idMensagem);
    }

    private async Task SolicitarNomeClienteAsync(string numeroTelefone, string? idMensagem = null)
    {
        await whatsAppService.EnviarMensagemTextoAsync(numeroTelefone, "Digite o nome do cliente:", idMensagem);
    }

    private async Task SolicitarGeracaoMensalAsync(string numeroTelefone, string? idMensagem = null)
    {
        await whatsAppService.EnviarMensagemTextoAsync(numeroTelefone, "Quanto deseja gerar por mês em kW/h?", idMensagem);
    }

    private async Task SolicitarEstadoAsync(string numeroTelefone, string? idMensagem = null)
    {
        await whatsAppService.EnviarMensagemTextoAsync(numeroTelefone, "Digite o estado do cliente:", idMensagem);
    }

    private async Task SolicitarCidadeAsync(string numeroTelefone, string? idMensagem = null)
    {
        await whatsAppService.EnviarMensagemTextoAsync(numeroTelefone, "Digite a cidade do cliente:", idMensagem);
    }

    private async Task SolicitarTipoTelhadoAsync(string numeroTelefone, string? idMensagem = null)
    {
        await whatsAppService.EnviarMensagemComBotoesAsync(
            numeroTelefone,
            "Selecione a estrutura do telhado (parte 1/2):",
            new[] {
                new BotaoResposta("colonial", "Colonial"),
                new BotaoResposta("fibrocimento", "Fibrocimento"),
                new BotaoResposta("laje", "Laje")
            },
            idMensagem);

        await whatsAppService.EnviarMensagemComBotoesAsync(
            numeroTelefone,
            "Selecione a estrutura do telhado (parte 2/2):",
            new[] {
                new BotaoResposta("ondulada", "Ondulada"),
                new BotaoResposta("solo", "Solo")
            });
    }

    private async Task EnviarConfirmacaoPropostaAsync(string numeroTelefone, string? idMensagem = null)
    {
        var dadosProposta = gerenciadorConversa.ObterDadosProposta(numeroTelefone);
        
        await whatsAppService.EnviarMensagemComBotoesAsync(
            numeroTelefone,
            $"Deseja confirmar proposta?\n\n{dadosProposta.ObterResumo()}",
            new[] {
                new BotaoResposta("confirmar", "Confirmar"),
                new BotaoResposta("cancelar", "Cancelar")
            },
            idMensagem);
    }

    private async Task EnviarMensagemPropostaProcessadaAsync(string numeroTelefone, string? idMensagem = null)
    {
        await whatsAppService.EnviarMensagemTextoAsync(numeroTelefone, "Processando proposta...", idMensagem);
    }

    private async Task EnviarMensagemPadraoAsync(string numeroTelefone, string? idMensagem = null)
    {
        await EnviarOpcoesIniciaisAsync(numeroTelefone, idMensagem);
    }
}