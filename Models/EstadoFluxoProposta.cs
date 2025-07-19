namespace WhatsAppBot.Models;

public enum EstadoFluxoProposta
{
    Inicio,
    AguardandoNomeCliente,
    AguardandoGeracaoMensalKwh,
    AguardandoEstado,
    AguardandoCidade,
    AguardandoTipoTelhado,
    AguardandoConfirmacao,
    PropostaFinalizada,
    PropostaCancelada
}