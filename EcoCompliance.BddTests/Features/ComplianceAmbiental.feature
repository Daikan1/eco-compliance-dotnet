# language: pt
@compliance
Funcionalidade: Rastreabilidade do compliance ambiental
  Como auditor de conformidade regulatória
  Quero registrar, atualizar e remover registros de compliance de cada empresa
  Para manter um histórico íntegro e rastreável das obrigações ambientais

  Contexto:
    Dado que estou autenticado como administrador
    E que existe a empresa "Prefeitura de São Paulo" cadastrada

  Cenário: Registrar uma obrigação de compliance para uma empresa
    Quando registro o compliance "Licença Ambiental de Operação" com status "EM_ANALISE" para a empresa "Prefeitura de São Paulo"
    Então o status da resposta deve ser 201
    E a resposta deve seguir o contrato "compliance"
    E o campo "companyName" da resposta deve ser "Prefeitura de São Paulo"
    E o campo "status" da resposta deve ser "EM_ANALISE"

  Cenário: Registrar compliance para uma empresa inexistente
    Quando registro o compliance "Inventário de Emissões GEE" com status "PENDENTE" para a empresa de id 999
    Então o status da resposta deve ser 404
    E a resposta deve seguir o contrato "error"
    E o campo "message" da resposta deve ser "Company not found with id: 999"

  Cenário: Registrar compliance sem o tipo da obrigação é rejeitado
    Quando envio uma requisição POST para "/api/compliance" com o corpo:
      """
      { "companyId": 1, "status": "PENDENTE", "reportDate": "2026-10-01T00:00:00Z" }
      """
    Então o status da resposta deve ser 400
    E a resposta deve seguir o contrato "validation-problem"
    E o erro de validação deve apontar o campo "ComplianceType"

  Cenário: Atualizar o status de um registro mantém a rastreabilidade
    Dado que existe o compliance "Plano de Gerenciamento de Resíduos" com status "EM_ANALISE" para a empresa "Prefeitura de São Paulo"
    Quando atualizo o status desse registro de compliance para "CONFORME"
    Então o status da resposta deve ser 200
    E a resposta deve seguir o contrato "compliance"
    E o campo "status" da resposta deve ser "CONFORME"
    E o campo "complianceType" da resposta deve ser "Plano de Gerenciamento de Resíduos"

  Cenário: Atualizar um registro de compliance inexistente
    Quando atualizo o registro de compliance de id 999 para a empresa "Prefeitura de São Paulo"
    Então o status da resposta deve ser 404
    E a resposta deve seguir o contrato "error"
    E o campo "message" da resposta deve ser "ComplianceRecord not found with id: 999"

  Cenário: Remover um registro de compliance
    Dado que existe o compliance "Outorga de Uso da Água" com status "VENCIDO" para a empresa "Prefeitura de São Paulo"
    Quando removo esse registro de compliance
    Então o status da resposta deve ser 204
    E ao listar os registros de compliance da empresa "Prefeitura de São Paulo" a resposta deve conter 0 itens

  Cenário: Remover um registro de compliance inexistente
    Quando removo o registro de compliance de id 999
    Então o status da resposta deve ser 404
    E a resposta deve seguir o contrato "error"

  Cenário: Listar todos os registros de compliance
    Dado que existe o compliance "Licença Ambiental de Operação" com status "CONFORME" para a empresa "Prefeitura de São Paulo"
    E que existe o compliance "Inventário de Emissões GEE" com status "EM_ANALISE" para a empresa "Prefeitura de São Paulo"
    Quando listo os registros de compliance
    Então o status da resposta deve ser 200
    E a resposta deve seguir o contrato "compliance-page"
    E a resposta deve conter 2 itens

  Cenário: Listar os registros de compliance de uma empresa
    Dado que existe a empresa "Usina Solar Horizonte" cadastrada
    E que existe o compliance "Licença Ambiental de Operação" com status "CONFORME" para a empresa "Prefeitura de São Paulo"
    E que existe o compliance "Relatório de Sustentabilidade" com status "EM_ANALISE" para a empresa "Usina Solar Horizonte"
    Quando listo os registros de compliance da empresa "Usina Solar Horizonte"
    Então o status da resposta deve ser 200
    E a resposta deve seguir o contrato "compliance-page"
    E a resposta deve conter 1 item
    E o campo "items[0].companyName" da resposta deve ser "Usina Solar Horizonte"

  Cenário: Listar compliance de uma empresa inexistente
    Quando listo os registros de compliance da empresa de id 999
    Então o status da resposta deve ser 404
    E a resposta deve seguir o contrato "error"

  Esquema do Cenário: Alterações de compliance exigem autenticação
    Dado que existe o compliance "Licença Ambiental de Operação" com status "CONFORME" para a empresa "Prefeitura de São Paulo"
    E que não estou autenticado
    Quando envio uma requisição <método> para o registro de compliance criado
    Então o status da resposta deve ser 401

    Exemplos:
      | método |
      | PUT    |
      | DELETE |
