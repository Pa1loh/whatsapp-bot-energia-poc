# Energia WhatsApp Bot 🤖

Um serviço de chatbot para WhatsApp utilizando a API do WhatsApp Business, desenvolvido com .NET 9.0 e C#.

## 📋 Descrição do Projeto

Este projeto implementa um bot para WhatsApp capaz de interagir com usuários, coletar informações para geração de propostas e processar essas informações. O bot segue um fluxo de diálogo estruturado, solicitando informações como:

- Nome do cliente
- Geração mensal desejada em kW/h
- Estado do cliente
- Cidade do cliente
- Estrutura de telhado (com opções: Colonial, Fibrocimento, Laje, Ondulada, Solo)

Após coletar essas informações, o bot apresenta um resumo da proposta para confirmação do usuário, paraf uturamente comunicar com alguma api e automatizar o processo de geração de propostas.

## 🔧 Tecnologias Utilizadas

- .NET 9.0
- ASP.NET Core
- API WhatsApp Business (Meta)
- C#

## ⚙️ Configurações

O projeto utiliza um arquivo `appsettings.json` para armazenar configurações. Algumas configurações sensíveis são gerenciadas com o Secret Manager do .NET.

### Configurações do WhatsApp

```json
"WhatsApp": {
  "VerifyToken": "xpto",
  "AccessToken": "xpto",
  "PhoneNumberId": "xpto"
}
```

- **VerifyToken**: Token utilizado para validação do webhook pelo WhatsApp. Não é um dado sensível e pode ser mantido no arquivo `appsettings.json`.
- **AccessToken**: Token de acesso à API do WhatsApp Business. Por questões de segurança, este token não é armazenado no arquivo de configuração, mas sim gerenciado pelo Secret Manager do .NET.
- **PhoneNumberId**: ID do número de telefone registrado na API do WhatsApp Business. Este é um valor padrão fornecido pela plataforma.

## 🔒 Gerenciamento de Segredos

Para proteger o AccessToken da API do WhatsApp, o projeto utiliza o Secret Manager do .NET. Para configurar:

```bash
# Inicializar o Secret Manager (necessário apenas na primeira vez)
dotnet user-secrets init --project WhatsAppBot

# Adicionar o AccessToken
dotnet user-secrets set "WhatsApp:AccessToken" "seu_token_aqui" --project WhatsAppBot
```

## 🚀 Como Executar

### Pré-requisitos

- .NET 9.0 SDK ou superior
- Conta de desenvolvedor na plataforma WhatsApp Business
- Número de telefone registrado na plataforma WhatsApp Business

### Passos para Executar

1. Restaure as dependências:
```bash
dotnet restore
```

2. Configure o Secret Manager com o AccessToken:
```bash
dotnet user-secrets set "WhatsApp:AccessToken" "seu_token_aqui" --project WhatsAppBot
```

3. Execute o projeto:
```bash
dotnet run --project WhatsAppBot
```

4. Configure a URL do webhook na plataforma WhatsApp Business:
   - A URL deve apontar para `/api/webhook` da sua aplicação
   - Use o mesmo `VerifyToken` definido no arquivo de configuração (como atualmente não esta em deploy, para configurar localmente, precisamos utilizar o ngrok para deixar a api e termos um acesso publico)

## 🌐 Integração com WhatsApp Business API

### Configuração do Webhook

Para receber mensagens do WhatsApp, você precisa configurar um webhook:

1. Acesse o [Meta for Developers](https://developers.facebook.com/)
2. Configure o webhook para apontar para sua URL: `https://seu-dominio.com/api/webhook`
3. Use o `VerifyToken` definido nas configurações
4. Selecione os eventos necessários (mensagens, entregas, etc.)

## 📦 Estrutura do Projeto

- **Controllers**: Controladores da API, incluindo o `WebhookController`
- **Models**: Definição de modelos de dados como `DadosProposta` e `EstadoFluxoProposta`
- **Services**: Implementação dos serviços de negócio
  - **Interfaces**: Contratos para os serviços
  - `ChatbotService.cs`: Lógica de processamento das mensagens e gestão do fluxo de conversa
  - `WhatsAppService.cs`: Integração com a API do WhatsApp
  - `GerenciadorConversa.cs`: Gerencia os estados da conversa e dados temporários

## 🔄 Fluxo da Conversa

1. **Início**: Bot apresenta opções iniciais
2. **Iniciar Proposta**: Bot solicita o nome do cliente
3. **Nome do Cliente**: Bot solicita a geração mensal em kW/h
4. **Geração Mensal**: Bot solicita o estado do cliente
5. **Estado**: Bot solicita a cidade do cliente
6. **Cidade**: Bot apresenta opções de estrutura de telhado
7. **Estrutura de Telhado**: Bot apresenta resumo da proposta para confirmação
8. **Confirmação**: Bot processa a proposta ou cancela conforme escolha do usuário
