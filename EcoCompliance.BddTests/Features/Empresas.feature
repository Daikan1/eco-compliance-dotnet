# language: pt
@empresas
Funcionalidade: Cadastro de empresas monitoradas
  Como gestor de compliance ambiental
  Quero cadastrar e consultar as empresas acompanhadas pela plataforma
  Para vincular a elas os indicadores ESG e os registros de conformidade

  Contexto:
    Dado que estou autenticado como administrador

  Cenário: Cadastrar uma empresa com sucesso
    Quando cadastro a empresa "Prefeitura de São Paulo" com CNPJ "46395000000139" do setor "Governo"
    Então o status da resposta deve ser 201
    E a resposta deve seguir o contrato "company"
    E o campo "name" da resposta deve ser "Prefeitura de São Paulo"
    E o campo "sector" da resposta deve ser "Governo"
    E a resposta deve indicar a localização do recurso criado

  Cenário: Cadastrar uma empresa sem nome é rejeitado
    Quando envio uma requisição POST para "/api/companies" com o corpo:
      """
      { "cnpj": "46395000000139", "sector": "Governo" }
      """
    Então o status da resposta deve ser 400
    E a resposta deve seguir o contrato "validation-problem"
    E o erro de validação deve apontar o campo "Name"

  Cenário: Consultar uma empresa cadastrada pelo id
    Dado que existe a empresa "Secretaria do Verde e Meio Ambiente" cadastrada
    Quando consulto a empresa "Secretaria do Verde e Meio Ambiente"
    Então o status da resposta deve ser 200
    E a resposta deve seguir o contrato "company"
    E o campo "name" da resposta deve ser "Secretaria do Verde e Meio Ambiente"

  Cenário: Consultar uma empresa inexistente retorna 404
    Quando consulto a empresa de id 999
    Então o status da resposta deve ser 404
    E a resposta deve seguir o contrato "error"
    E o campo "message" da resposta deve ser "Company not found with id: 999"

  Cenário: Listar empresas com paginação
    Dado que existem as empresas:
      | nome                     |
      | Cooperativa Recicla Mais |
      | Usina Solar Horizonte    |
      | Transportes Verdes S.A.  |
    Quando listo as empresas com page 1 e pageSize 2
    Então o status da resposta deve ser 200
    E a resposta deve seguir o contrato "company-page"
    E a resposta deve conter 2 itens
    E o campo "totalCount" da resposta deve ser 3
    E o campo "totalPages" da resposta deve ser 2

  Esquema do Cenário: Paginação inválida é recusada
    Quando listo as empresas com page <page> e pageSize <pageSize>
    Então o status da resposta deve ser 400
    E a resposta deve seguir o contrato "error"
    E o campo "message" da resposta deve conter "<campo>"

    Exemplos:
      | page | pageSize | campo    |
      | 0    | 10       | page     |
      | 1    | 0        | pageSize |
      | 1    | 101      | pageSize |
