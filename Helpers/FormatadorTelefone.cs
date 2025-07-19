namespace WhatsAppBot.Helpers;

public static class FormatadorTelefone
{
    public static string FormatarNumeroBrasileiro(string numeroTelefone)
    {
        var apenasNumeros = new string(numeroTelefone.Where(char.IsDigit).ToArray());
        
        if (apenasNumeros.Length == 13 && apenasNumeros.StartsWith("55"))
            return apenasNumeros;
        
        if (apenasNumeros.Length == 12 && apenasNumeros.StartsWith("55"))
            return apenasNumeros.Substring(0, 4) + "9" + apenasNumeros.Substring(4);
        
        return apenasNumeros;
    }
}