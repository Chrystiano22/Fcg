# FIAP Cloud Games - Tech Challenge Fase 1

## Visao geral

Este repositorio contem a implementacao da **Fase 1** do Tech Challenge da plataforma **FIAP Cloud Games (FCG)**.

O objetivo desta fase e entregar um **MVP em .NET 8** para:

- cadastro de usuarios
- autenticacao com JWT
- autorizacao por perfis `User` e `Administrator`
- administracao de jogos
- biblioteca de jogos adquiridos
- criacao de promocoes por administrador

O projeto foi construido como um **monolito modular**, com separacao por camadas para manter a base pronta para evolucoes futuras.

## Objetivo academico da fase

Esta entrega atende o escopo obrigatorio de:

- API REST em `.NET 8`
- persistencia com `Entity Framework Core`
- migrations
- Swagger
- middleware global de erros
- logs estruturados
- testes unitarios e de integracao
- organizacao aderente a `DDD`

Itens opcionais como `MongoDB`, `Dapper`, `GraphQL` e `Domain Storytelling` ficaram fora deste ciclo.

## Arquitetura

O projeto segue a estrutura abaixo:

```text
/src
  /Fcg.Api
  /Fcg.Application
  /Fcg.Domain
  /Fcg.Infrastructure
/tests
  /Fcg.UnitTests
/docs
  /ddd
  /entrega
README.md
FASE1_CHECKLIST_EXECUCAO.md
DOCUMENTACAO_ANDAMENTO_FASE1.md
```

### Camadas

- `Fcg.Api`
  Exposicao HTTP, controllers, contratos de request/response, autenticacao, Swagger e middleware.
- `Fcg.Application`
  Casos de uso da aplicacao e contratos de servicos.
- `Fcg.Domain`
  Entidades, regras de negocio e contratos de repositorio.
- `Fcg.Infrastructure`
  EF Core, repositorios, seguranca, inicializacao e persistencia.
- `Fcg.UnitTests`
  Testes de dominio, casos de uso e integracao da API.

## Tecnologias adotadas

- `.NET 8`
- `ASP.NET Core Web API`
- `Entity Framework Core`
- `SQLite`
- `JWT Bearer`
- `Swagger / OpenAPI`
- `xUnit`

## Funcionalidades implementadas

### Usuarios

- `POST /usuarios`
  Cadastra usuario com `nome`, `email` e `senha`.
- validacao de email
- validacao de senha forte
- consulta do proprio usuario autenticado em `GET /usuarios/me`
- listagem administrativa em `GET /usuarios`
- alteracao de perfil em `PATCH /usuarios/{id}/perfil`

### Autenticacao e autorizacao

- `POST /auth/login`
- emissao de token `JWT`
- autorizacao por perfil:
  - `User`
  - `Administrator`

### Catalogo de jogos

- `GET /jogos`
- `POST /jogos`
- `PUT /jogos/{id}`
- `DELETE /jogos/{id}`

### Biblioteca

- `GET /biblioteca`
- `POST /usuarios/{userId}/biblioteca/{gameId}`

### Promocoes

- `GET /promocoes`
- `POST /promocoes`

## Regras de negocio principais

- o email do usuario deve ser valido e unico
- a senha deve ter no minimo 8 caracteres, com letras, numeros e caractere especial
- apenas administradores podem:
  - cadastrar jogos
  - alterar jogos
  - desativar jogos
  - administrar usuarios
  - criar promocoes
- um jogo desativado nao aparece na listagem autenticada do catalogo
- um jogo nao pode ser vinculado duas vezes para o mesmo usuario na biblioteca
- uma promocao so pode ser criada para jogo ativo
- a promocao tem percentual de desconto e janela de vigencia

## Persistencia

O projeto utiliza `SQLite` para manter o MVP simples e reproduzivel localmente.

Arquivos de banco:

- `src/Fcg.Api/fcg.db`
- `src/Fcg.Api/fcg.dev.db`

Migrations existentes:

- `InitialCreate`
- `AddPromotions`

## Observabilidade

O projeto possui:

- middleware global de tratamento de excecao
- `ProblemDetails` padronizado
- logs estruturados em JSON no console
- `X-Correlation-Id` por requisicao
- `correlationId` retornado em respostas de erro

## Credenciais de demonstracao

Ao subir a aplicacao em ambiente local, um administrador padrao e semeado automaticamente:

- email: `admin@fcg.local`
- senha: `Admin@123`

## Como executar localmente

### Pre-requisitos

- `.NET SDK 8`

### 1. Restaurar dependencias

```powershell
dotnet restore Fcg.sln
```

### 2. Aplicar migrations

```powershell
dotnet ef database update --project src/Fcg.Infrastructure
```

### 3. Executar a API

```powershell
dotnet run --project src/Fcg.Api
```

Em ambiente de desenvolvimento, a API sobe com Swagger disponivel em:

- `http://localhost:5020/swagger`
- `https://localhost:7195/swagger`

## Como rodar os testes

```powershell
dotnet build Fcg.sln --no-restore /m:1
dotnet test tests/Fcg.UnitTests/Fcg.UnitTests.csproj --no-build --no-restore -m:1
```

Observacao:

- neste ambiente, `build` e `test` ficaram mais estaveis em modo sequencial com `/m:1`

## Fluxo recomendado para demonstracao

1. Acessar o Swagger.
2. Criar um usuario em `POST /usuarios`.
3. Fazer login desse usuario em `POST /auth/login`.
4. Fazer login do administrador padrao.
5. Autorizar o token no Swagger.
6. Cadastrar um jogo em `POST /jogos`.
7. Listar jogos em `GET /jogos`.
8. Criar uma promocao em `POST /promocoes`.
9. Listar promocoes em `GET /promocoes`.
10. Vincular um jogo ao usuario em `POST /usuarios/{userId}/biblioteca/{gameId}`.
11. Consultar a biblioteca em `GET /biblioteca`.
12. Demonstrar `GET /usuarios` e `PATCH /usuarios/{id}/perfil`.

## Documentos de apoio do projeto

- checklist principal da execucao:
  - [FASE1_CHECKLIST_EXECUCAO.md](./FASE1_CHECKLIST_EXECUCAO.md)
- consolidado tecnico do andamento:
  - [DOCUMENTACAO_ANDAMENTO_FASE1.md](./DOCUMENTACAO_ANDAMENTO_FASE1.md)
- documentacao DDD visual equivalente ao Miro:
  - [docs/ddd/DDD_FASE1_FCG.md](./docs/ddd/DDD_FASE1_FCG.md)
- relatorio final de entrega em base TXT:
  - [docs/entrega/RELATORIO_ENTREGA_FASE1.txt](./docs/entrega/RELATORIO_ENTREGA_FASE1.txt)
- pasta reservada para artefatos finais de entrega:
  - [docs/entrega](./docs/entrega)

## Estado atual da fase

No estado atual deste repositorio, a Fase 1 possui:

- escopo obrigatorio de API implementado
- persistencia e migrations implementadas
- autenticacao e autorizacao implementadas
- promocoes implementadas
- testes automatizados cobrindo dominio, aplicacao e endpoints principais

O que ainda falta para a entrega academica final:

- gravar o video
- preencher os dados finais do relatorio
