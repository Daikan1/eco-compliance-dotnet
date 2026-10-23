# language: pt
@autenticacao
Funcionalidade: Autenticação de gestores ESG
  Como gestor de compliance ambiental
  Quero me autenticar na API com usuário e senha
  Para que apenas pessoas autorizadas alterem dados de sustentabilidade (pilar de Governança)

  Cenário: Login com credenciais válidas retorna um token JWT
    Quando faço login com as credenciais do administrador
    Então o status da resposta deve ser 200
    E a resposta deve seguir o contrato "login"
    E o campo "token" da resposta deve estar preenchido

  Cenário: Login com senha incorreta é recusado
    Quando faço login com usuário "admin" e senha "senha-errada"
    Então o status da resposta deve ser 401
    E a resposta deve seguir o contrato "auth-error"
    E o campo "message" da resposta deve ser "Invalid credentials"

  Cenário: Login sem informar a senha é rejeitado pela validação
    Quando envio uma requisição POST para "/api/auth/login" com o corpo:
      """
      { "username": "admin" }
      """
    Então o status da resposta deve ser 400
    E a resposta deve seguir o contrato "validation-problem"
    E o erro de validação deve apontar o campo "Password"

  Cenário: Operação protegida sem token é bloqueada
    Dado que não estou autenticado
    Quando cadastro a empresa "Empresa Sem Login"
    Então o status da resposta deve ser 401
    E a resposta deve exigir autenticação do tipo "Bearer"
