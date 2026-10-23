# language: pt
@monitoramento
Funcionalidade: Monitoramento da API
  Como equipe de operações responsável pela governança do sistema ESG
  Quero saber qual versão está no ar e se a aplicação está saudável
  Para garantir rastreabilidade dos deploys e disponibilidade do serviço

  Cenário: Consultar a identificação da API em execução
    Quando consulto a identificação da API
    Então o status da resposta deve ser 200
    E a resposta deve seguir o contrato "app-info"
    E o campo "environment" da resposta deve ser "Testing"
    E o campo "application" da resposta deve conter "Cidades ESG Inteligentes"

  Cenário: Verificar a saúde da aplicação
    Quando consulto o health check
    Então o status da resposta deve ser 200
    E o corpo da resposta deve ser "Healthy"
