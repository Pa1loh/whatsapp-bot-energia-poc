using WhatsAppBot.Models;

namespace WhatsAppBot.Services;

public class GerenciadorConversa
{
    private readonly Dictionary<string, EstadoFluxoProposta> _estadosConversa = new();
    
    private readonly Dictionary<string, DadosProposta> _dadosPropostas = new();
    public EstadoFluxoProposta ObterEstadoConversa(string numeroTelefone)
    {
        if (!_estadosConversa.ContainsKey(numeroTelefone))
        {
            _estadosConversa[numeroTelefone] = EstadoFluxoProposta.Inicio;
        }
        
        return _estadosConversa[numeroTelefone];
    }
  
    public void DefinirEstadoConversa(string numeroTelefone, EstadoFluxoProposta estado)
    {
        _estadosConversa[numeroTelefone] = estado;
    }
    
    public DadosProposta ObterDadosProposta(string numeroTelefone)
    {
        if (!_dadosPropostas.ContainsKey(numeroTelefone))
        {
            _dadosPropostas[numeroTelefone] = new DadosProposta();
        }
        
        return _dadosPropostas[numeroTelefone];
    }
    
    public void LimparDadosConversa(string numeroTelefone)
    {
        _estadosConversa.Remove(numeroTelefone);
        _dadosPropostas.Remove(numeroTelefone);
    }
}