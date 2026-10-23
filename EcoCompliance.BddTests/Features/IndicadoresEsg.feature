# language: pt
@indicadores
Funcionalidade: Registro de indicadores ESG
  Como analista de sustentabilidade
  Quero registrar indicadores ambientais medidos em cada empresa
  Para acompanhar emissões, consumo de energia e reciclagem de resíduos

  Contexto:
    Dado que estou autenticado como administrador
    E que existe a empresa "Cooperativa Recicla Mais" cadastrada

  Esquema do Cenário: Registrar indicadores ambientais
    Quando registro o indicador "<indicador>" com valor <valor> "<unidade>" para a empresa "Cooperativa Recicla Mais"
    Então o status da resposta deve ser 201
    E a resposta deve seguir o contrato "esg-indicator"
    E o campo "indicatorType" da resposta deve ser "<indicador>"
    E o campo "value" da resposta deve ser <valor>
    E o campo "companyName" da resposta deve ser "Cooperativa Recicla Mais"

    Exemplos:
      | indicador               | valor  | unidade |
      | EMISSOES_CO2            | 120.5  | tCO2e   |
      | CONSUMO_ENERGIA         | 3400   | kWh     |
      | RESIDUOS_RECICLADOS     | 850.75 | kg      |

  Cenário: Indicador com valor negativo é rejeitado (integridade dos dados ESG)
    Quando registro o indicador "EMISSOES_CO2" com valor -50 "tCO2e" para a empresa "Cooperativa Recicla Mais"
    Então o status da resposta deve ser 400
    E a resposta deve seguir o contrato "validation-problem"
    E o erro de validação deve apontar o campo "Value"

  Cenário: Indicador para uma empresa inexistente
    Quando registro o indicador "CONSUMO_AGUA" com valor 1500 "m3" para a empresa de id 999
    Então o status da resposta deve ser 404
    E a resposta deve seguir o contrato "error"
    E o campo "message" da resposta deve ser "Company not found with id: 999"

  Cenário: Listar os indicadores ESG registrados
    Dado que existe o indicador "EMISSOES_CO2" com valor 98.2 "tCO2e" para a empresa "Cooperativa Recicla Mais"
    E que existe o indicador "RESIDUOS_RECICLADOS" com valor 430 "kg" para a empresa "Cooperativa Recicla Mais"
    Quando listo os indicadores ESG
    Então o status da resposta deve ser 200
    E a resposta deve seguir o contrato "esg-indicator-page"
    E a resposta deve conter 2 itens

  Cenário: Registrar indicador sem autenticação é bloqueado
    Dado que não estou autenticado
    Quando registro o indicador "EMISSOES_CO2" com valor 10 "tCO2e" para a empresa "Cooperativa Recicla Mais"
    Então o status da resposta deve ser 401
