namespace WhatsAppBot.Models;

public class DadosProposta
{
    public string? NomeCliente { get; set; }
    public string? GeracaoMensalKwh { get; set; }
    public string? Estado { get; set; }
    public string? Cidade { get; set; }
    public string? TipoTelhado { get; set; }
    public string ObterResumo()
    {
        return $"*Resumo da Proposta*\n\n" +
               $"*Nome:* {NomeCliente}\n" +
               $"*Geração Mensal:* {GeracaoMensalKwh} kWh\n" +
               $"*Estado:* {Estado}\n" +
               $"*Cidade:* {Cidade}\n" +
               $"*Tipo de Telhado:* {TipoTelhado}";
    }
}