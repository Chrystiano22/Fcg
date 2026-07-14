# Fase 1 - Documentacao de Andamento

Este documento consolida o que foi implementado ate agora no projeto da Fase 1, quais decisoes tecnicas foram tomadas e qual e o plano de execucao para fechar o escopo obrigatorio.

## 1. Estado atual

- Escopo considerado: apenas itens obrigatorios da Fase 1
- Progresso estimado da entrega obrigatoria: `99%`
- Branch de trabalho: `fase1-fcg-mvp`
- Arquitetura adotada: `monolito modular em .NET 8`

## 2. O que ja foi feito

### 2.1 Descoberta e definicao do MVP

- O enunciado foi consolidado em um escopo executavel.
- Os itens opcionais foram explicitamente deixados fora do ciclo atual.
- Os principais atores, entidades e fluxos foram mapeados.
- O arquivo `FASE1_CHECKLIST_EXECUCAO.md` foi definido como fonte de verdade do projeto.
- A documentacao DDD visual equivalente foi consolidada em `docs/ddd/DDD_FASE1_FCG.md`.
- O relatorio final base foi consolidado em `docs/entrega/RELATORIO_ENTREGA_FASE1.txt`.

### 2.2 Estrutura da solucao

- Solucao `.sln` criada.
- Projetos criados:
  - `Fcg.Api`
  - `Fcg.Application`
  - `Fcg.Domain`
  - `Fcg.Infrastructure`
  - `Fcg.UnitTests`
- A API foi configurada com `Controllers`, `Swagger` e composition root inicial.
- A estrutura por camadas foi estabilizada para evolucao com DDD.

### 2.3 Dominio e regras de negocio

- Dominio de `User` implementado com:
  - validacao de nome obrigatorio
  - validacao de email
  - politica de senha forte
  - perfil `User` e `Administrator`
- Dominio de `Game` implementado com:
  - titulo, descricao, categoria e preco
  - ativacao e desativacao
  - atualizacao de dados
- Dominio de `LibraryItem` implementado com:
  - `UserId`
  - `GameId`
  - `AcquiredAt`
- Contratos de repositorio definidos para:
  - `User`
  - `Game`
  - `LibraryItem`

### 2.4 Casos de uso na camada Application

- Caso de uso de cadastro de usuario:
  - `RegisterUserUseCase`
- Caso de uso de autenticacao:
  - `AuthenticateUserUseCase`
- Contratos de seguranca definidos:
  - `IPasswordHasher`
  - `IAccessTokenGenerator`

### 2.5 Persistencia

- `Entity Framework Core` configurado.
- `DbContext` criado:
  - `FcgDbContext`
- Mapeamentos criados para:
  - `User`
  - `Game`
  - `LibraryItem`
- Repositorios implementados no `Infrastructure`.
- `SQLite` adotado para o MVP local.
- Primeira migration criada e aplicada com sucesso.

### 2.6 Autenticacao e endpoints ja expostos

- Endpoint `POST /usuarios` implementado.
- Endpoint `POST /auth/login` implementado.
- Endpoint `GET /usuarios/me` implementado e protegido por autenticacao.
- Endpoint `POST /jogos` implementado e protegido por role `Administrator`.
- Endpoint `GET /jogos` implementado para usuario autenticado.
- Endpoint `PUT /jogos/{id}` implementado para administracao do catalogo.
- Endpoint `DELETE /jogos/{id}` implementado como desativacao logica.
- Endpoint `GET /biblioteca` implementado para o usuario autenticado.
- Endpoint `POST /usuarios/{userId}/biblioteca/{gameId}` implementado para vinculo de jogo adquirido.
- Endpoint `GET /usuarios` implementado para administracao.
- Endpoint `PATCH /usuarios/{id}/perfil` implementado para alteracao de papel por administrador.
- Endpoint `POST /promocoes` implementado para criacao de promocao por administrador.
- Endpoint `GET /promocoes` implementado para consulta de promocoes por usuario autenticado.
- `JWT Bearer` configurado no pipeline.
- Claims emitidas no token:
  - `NameIdentifier`
  - `Name`
  - `Email`
  - `Role`
- `Swagger` configurado com esquema `Bearer`.
- Tratamento global de excecao implementado com `ProblemDetails`.
- Logs estruturados configurados com saida JSON no console.
- Middleware de request logging implementado com `correlationId`, status HTTP e tempo de execucao.
- Respostas de erro passam a incluir `correlationId` para rastreamento.
- Seed local de administrador configurado para facilitar testes e demonstracao.
- A listagem publica do catalogo retorna apenas jogos ativos.
- Promocoes foram modeladas como descontos percentuais por jogo, com periodo de vigencia.

### 2.7 Testes e validacao

- Testes unitarios do dominio implementados para:
  - `User`
  - `Game`
  - `LibraryItem`
- Testes unitarios de casos de uso implementados para:
  - cadastro de usuario
  - autenticacao
  - criacao de jogo
  - atualizacao de jogo
  - desativacao de jogo
  - aquisicao de jogo para a biblioteca
- Testes de integracao implementados para:
  - `POST /usuarios`
  - `POST /auth/login`
  - `GET /usuarios/me`
  - `POST /jogos`
  - `GET /jogos`
  - `PUT /jogos/{id}`
  - `DELETE /jogos/{id}`
  - `GET /biblioteca`
  - `POST /usuarios/{userId}/biblioteca/{gameId}`
  - `GET /usuarios`
  - `PATCH /usuarios/{id}/perfil`
- Estado atual da suite:
  - `66` testes aprovados

## 3. Decisoes tecnicas importantes

- O codigo segue nomenclatura em ingles:
  - `User`
  - `Game`
  - `LibraryItem`
- Os contratos HTTP foram mantidos em portugues para ficar aderente ao contexto academico:
  - `nome`
  - `email`
  - `senha`
- O banco do MVP foi padronizado em `SQLite` para simplificar reproducao local.
- O hash de senha usa `PBKDF2`.
- O token de autenticacao usa `JWT`.
- Existe um administrador local padrao para demonstracao:
  - `admin@fcg.local`
  - `Admin@123`
- A administracao de catalogo usa desativacao logica, e nao remocao fisica.
- Neste ambiente, `build` e `test` funcionam melhor em modo sequencial:
  - `dotnet build Fcg.sln --no-restore /m:1`
  - `dotnet test tests/Fcg.UnitTests/Fcg.UnitTests.csproj --no-build --no-restore -m:1`

## 4. O que falta fazer

### 4.1 API e regras obrigatorias

- Nenhuma pendencia tecnica obrigatoria aberta.

### 4.2 Qualidade e observabilidade

- Expandir cobertura de testes para fluxos administrativos e de catalogo.

### 4.3 Entregaveis academicos

- Gravar video de demonstracao.
- Preencher os dados finais do relatorio.

## 5. Proximos passos recomendados

1. Gravar o video de demonstracao.
2. Preencher os dados finais do relatorio.
3. Revisar os links finais de entrega.
4. Fechar o checklist final da fase.
5. Confirmar a versao final que sera submetida.

## 6. Como validar localmente

### Build

```powershell
dotnet restore Fcg.sln
dotnet build Fcg.sln --no-restore /m:1
```

### Testes

```powershell
dotnet test tests/Fcg.UnitTests/Fcg.UnitTests.csproj --no-build --no-restore -m:1
```

### Banco

```powershell
dotnet ef database update --project src/Fcg.Infrastructure --no-build
```

### API

```powershell
dotnet run --project src/Fcg.Api
```

Depois disso, acesse o `Swagger` e valide:

1. `POST /usuarios`
2. `POST /auth/login` com usuario comum
3. `POST /auth/login` com administrador local
4. `Authorize` com o token JWT
5. `GET /usuarios/me`
6. `POST /jogos`
7. `GET /jogos`
8. `PUT /jogos/{id}`
9. `DELETE /jogos/{id}`
10. `POST /usuarios/{userId}/biblioteca/{gameId}`
11. `GET /biblioteca`
12. `GET /usuarios`
13. `PATCH /usuarios/{id}/perfil`
14. `POST /promocoes`
15. `GET /promocoes`
