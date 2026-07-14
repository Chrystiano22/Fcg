# Tech Challenge Fase 1 - Checklist e Execucao

Este arquivo e a fonte de verdade do projeto para a Fase 1. Ele define o que deve ser entregue, a ordem de execucao e o padrao minimo que o time deve seguir durante o desenvolvimento.

## Como usar este arquivo

- Atualize o status de cada item conforme o projeto avanca.
- Nao inicie uma etapa nova sem revisar os criterios de aceite da etapa atual.
- Registre decisoes tecnicas e links importantes neste mesmo arquivo ou nos documentos referenciados por ele.
- Use este arquivo como base para o `README.md`, video de demonstracao e relatorio final.

## Legenda de status

- `[ ]` Nao iniciado
- `[-]` Em andamento
- `[x]` Concluido

---

## 1. Checklist de Entrega

### 1.1 API e Requisitos Funcionais

- [x] Cadastro de usuarios com `nome`, `email` e `senha`
- [x] Validacao de formato de email
- [x] Validacao de senha forte com minimo de 8 caracteres, letras, numeros e caractere especial
- [x] Autenticacao com JWT
- [x] Autorizacao com perfis `Usuario` e `Administrador`
- [x] Administrador pode cadastrar jogos
- [x] Administrador pode administrar usuarios
- [x] Administrador pode criar promocoes
- [x] Usuario pode acessar a plataforma e sua biblioteca de jogos
- [x] Biblioteca de jogos adquiridos implementada

### 1.2 Requisitos Tecnicos

- [x] API em `.NET 8`
- [x] Arquitetura monolitica
- [x] `Entity Framework Core` configurado
- [x] `Migrations` criadas e aplicadas
- [x] Middleware global para tratamento de erros
- [x] Logs estruturados
- [x] Swagger configurado
- [x] Testes unitarios escritos
- [x] TDD ou BDD aplicado em pelo menos um modulo
- [x] DDD refletido na organizacao do codigo

### 1.3 Entregaveis Finais

- [ ] Video de ate 15 minutos demonstrando os requisitos
- [x] Documentacao DDD em `Miro` ou equivalente
- [x] Event Storming de criacao de jogos
- [x] Event Storming de criacao de usuarios
- [x] Diagramas conforme disciplina de DDD
- [x] Repositorio com codigo-fonte
- [x] Repositorio com projeto de testes
- [x] `README.md` completo
- [-] Relatorio final em `PDF` ou `TXT`
- [ ] Link do video publicado
- [-] Link da documentacao
- [-] Link do repositorio

---

## 2. Ordem Recomendada de Execucao

### Etapa 1 - Descoberta e Dominio

Objetivo: transformar o enunciado em escopo executavel, modelo de dominio inicial e fluxos principais.

Checklist:
- [x] Ler e consolidar o escopo obrigatorio
- [x] Definir o MVP da Fase 1
- [x] Identificar atores do sistema
- [x] Identificar entidades principais do dominio
- [x] Rascunhar fluxos obrigatorios
- [x] Validar Event Storming em ferramenta visual
- [x] Converter fluxos em historias ou casos de uso

Criterio de aceite:
- O time sabe exatamente o que entra no MVP e o que fica como opcional.
- Existe um modelo inicial de dominio consistente com os requisitos.
- Existe um rascunho claro dos fluxos de criacao de usuario e criacao de jogo.

### Etapa 2 - Estrutura da Solucao

Status atual: `[x] Concluida`

Objetivo: criar a base tecnica do projeto em .NET 8.

Checklist:
- [x] Criar solucao `.sln`
- [x] Criar projeto da API
- [x] Criar projeto de testes
- [x] Definir estrutura por camadas
- [x] Configurar injecao de dependencia
- [x] Configurar arquivo de ambiente e settings

Criterio de aceite:
- O projeto compila.
- A estrutura suporta evolucao com DDD sem overengineering.

Observacao:
- Estrutura da solucao, projetos, configuracoes-base e composition root foram criados.
- Na conferencia de `2026-05-03`, a base foi estabilizada removendo referencias de projeto desnecessarias neste momento.
- Validacoes executadas com sucesso:
  `dotnet build src/Fcg.Domain/Fcg.Domain.csproj --no-restore`
  `dotnet build src/Fcg.Application/Fcg.Application.csproj --no-restore`
  `dotnet build src/Fcg.Infrastructure/Fcg.Infrastructure.csproj --no-restore`
  `dotnet build src/Fcg.Api/Fcg.Api.csproj --no-restore`
  `dotnet build tests/Fcg.UnitTests/Fcg.UnitTests.csproj --no-restore`
  `dotnet build Fcg.sln --no-restore /m:1`
  `dotnet test Fcg.sln --no-build --no-restore -m:1`
- Observacao tecnica:
  neste ambiente, a validacao da solucao ficou estavel em modo sequencial (`/m:1`).

### Etapa 3 - Dominio e Casos de Uso

Status atual: `[x] Concluida`

Objetivo: implementar entidades, regras e servicos do nucleo.

Checklist:
- [x] Criar entidade `Usuario`
- [x] Criar entidade `Jogo`
- [x] Criar entidade `BibliotecaItem` ou equivalente
- [x] Criar regras de validacao de email
- [x] Criar regras de validacao de senha
- [x] Criar regras de permissao por perfil
- [x] Definir contratos de repositorio

Criterio de aceite:
- As regras de negocio principais existem sem depender da camada web.

Observacao:
- Modulo `Usuario` criado no dominio com `Email`, `UserRole`, politica de senha e excecao de validacao.
- Testes unitarios criados para cadastro de usuario, validacao de email, senha, nome e permissao administrativa.
- Contrato `IUserRepository` criado no dominio e caso de uso `RegisterUserUseCase` implementado na camada `Application`.
- Contrato `IPasswordHasher` criado para preparar a integracao com infraestrutura de autenticacao.
- `AddApplication()` foi religado na API para registrar o caso de uso no container.
- Catalogo modelado com a entidade `Game` e contrato `IGameRepository`.
- Biblioteca modelada com a entidade `LibraryItem` e contrato `ILibraryItemRepository`.
- A nomenclatura de codigo segue o padrao em ingles ja adotado pelo projeto: `User`, `Game` e `LibraryItem`.
- Validacoes executadas com sucesso:
  `dotnet build Fcg.sln --no-restore /m:1`
  `dotnet test tests/Fcg.UnitTests/Fcg.UnitTests.csproj --no-restore -m:1`

### Etapa 4 - Persistencia

Status atual: `[x] Concluida`

Objetivo: persistir corretamente usuarios, jogos e biblioteca.

Checklist:
- [x] Configurar `DbContext`
- [x] Mapear entidades no EF Core
- [x] Definir relacionamentos
- [x] Criar primeira migration
- [x] Validar criacao e atualizacao do banco

Criterio de aceite:
- Banco sobe por migration e suporta o fluxo principal.

Observacao:
- Persistencia implementada com `Entity Framework Core` + `SQLite` para manter o MVP reproduzivel localmente sem dependencia de servidor externo.
- `FcgDbContext`, mapeamentos e repositorios de `User`, `Game` e `LibraryItem` foram criados no `Infrastructure`.
- `Pbkdf2PasswordHasher` foi registrado no `Infrastructure` para sustentar o fluxo de cadastro de usuario.
- Validacoes executadas com sucesso:
  `dotnet restore Fcg.sln`
  `dotnet build Fcg.sln --no-restore /m:1`
  `dotnet test tests/Fcg.UnitTests/Fcg.UnitTests.csproj --no-restore -m:1`
  `dotnet ef migrations add InitialCreate --project src/Fcg.Infrastructure --output-dir Persistence/Migrations --no-build`
  `dotnet ef database update --project src/Fcg.Infrastructure --no-build`

### Etapa 5 - Autenticacao e Autorizacao

Status atual: `[x] Concluida`

Objetivo: proteger os recursos da API.

Checklist:
- [x] Implementar endpoint de login
- [x] Gerar token JWT
- [x] Configurar autenticacao no pipeline
- [x] Configurar autorizacao por role
- [x] Restringir endpoints administrativos

Criterio de aceite:
- Usuario autenticado acessa apenas seus recursos.
- Administrador acessa endpoints administrativos.

Observacao:
- Endpoint `POST /auth/login` implementado com validacao de credenciais e retorno de token `JWT`.
- `JWT Bearer` configurado no pipeline da API com validacao de issuer, audience, assinatura e expiracao.
- Claims de `NameIdentifier`, `Name`, `Email` e `Role` passaram a ser emitidas no token.
- `Swagger` configurado com esquema `Bearer` para facilitar a demonstracao manual da autenticacao.
- O endpoint `GET /usuarios/me` foi exposto para validar autenticacao e leitura dos dados do usuario logado.
- Testes de unidade e integracao adicionados para login, falha de credenciais e consumo de endpoint protegido.
- O endpoint `POST /jogos` foi protegido com role `Administrator`.
- Um administrador padrao passou a ser semeado na base local para facilitar demonstracao e testes do fluxo administrativo.

### Etapa 6 - Endpoints do MVP

Status atual: `[x] Concluida`

Objetivo: expor os fluxos obrigatorios da fase.

Checklist:
- [x] `POST /usuarios`
- [x] `POST /auth/login`
- [x] `GET /usuarios/me`
- [x] `POST /jogos`
- [x] `GET /jogos`
- [x] `PUT /jogos/{id}`
- [x] `DELETE /jogos/{id}`
- [x] `GET /biblioteca`
- [x] Endpoint para vincular jogo adquirido ao usuario
- [x] Endpoints administrativos de usuarios
- [x] Endpoints de promocoes

Criterio de aceite:
- Todos os fluxos obrigatorios funcionam via API documentada.

Observacao:
- Endpoint `POST /usuarios` implementado com contrato HTTP em portugues (`nome`, `email`, `senha`).
- Fluxo conectado ao caso de uso `RegisterUserUseCase` e persistencia real via EF Core.
- Testes de integracao cobrem cadastro com sucesso e bloqueio de email duplicado.
- Endpoints `POST /jogos` e `GET /jogos` implementados com casos de uso da camada `Application`.
- `POST /jogos` exige role `Administrator`; `GET /jogos` exige usuario autenticado.
- Um administrador padrao (`admin@fcg.local` / `Admin@123`) foi configurado para demonstracao local do fluxo administrativo.
- Endpoints `PUT /jogos/{id}` e `DELETE /jogos/{id}` implementados para administracao do catalogo.
- `DELETE /jogos/{id}` realiza desativacao logica e a listagem publica do catalogo retorna apenas jogos ativos.
- `GET /biblioteca` implementado para o usuario autenticado visualizar sua propria biblioteca.
- Endpoint `POST /usuarios/{userId}/biblioteca/{gameId}` implementado para vincular um jogo adquirido via perfil administrador.
- Endpoints administrativos de usuarios implementados com listagem protegida e alteracao de perfil por administrador.
- Endpoints `POST /promocoes` e `GET /promocoes` implementados para fechar o requisito obrigatorio de promocoes.
- A promocao foi modelada como um desconto percentual associado a um jogo ativo, com inicio e fim de vigencia.

### Etapa 7 - Qualidade e Observabilidade

Objetivo: elevar a qualidade do software antes da entrega.

Checklist:
- [x] Middleware de excecao implementado
- [x] Logs estruturados configurados
- [x] Testes unitarios do dominio
- [x] TDD ou BDD aplicado em um modulo
- [-] Cobrir autenticacao e validacoes com testes

Criterio de aceite:
- Regras criticas possuem testes e falhas retornam resposta consistente.

Observacao:
- Logs estruturados foram configurados com saida JSON no console para facilitar rastreio e demonstracao da API.
- Cada requisicao passa a receber `X-Correlation-Id` e esse identificador tambem e retornado em respostas de erro via `ProblemDetails`.
- O middleware de request logging registra metodo, rota, status HTTP, tempo de execucao, endpoint resolvido e contexto de autenticacao.

### Etapa 8 - Documentacao e Entrega

Objetivo: fechar os entregaveis academicos.

Checklist:
- [x] Criar `README.md`
- [x] Finalizar documentacao DDD
- [ ] Gravar video
- [-] Montar relatorio final
- [ ] Revisar checklist completo

Criterio de aceite:
- Tudo o que sera demonstrado esta documentado e reproduzivel.

Observacao:
- `README.md` criado com objetivo da fase, arquitetura, stack, instrucoes de execucao, comandos de banco, testes, Swagger, credenciais de demonstracao e resumo das funcionalidades implementadas.
- Documentacao DDD visual criada em `docs/ddd/DDD_FASE1_FCG.md`, com context map, modelo de dominio, Event Storming e diagramas em Mermaid.
- Relatorio final base criado em `docs/entrega/RELATORIO_ENTREGA_FASE1.txt`, faltando apenas dados finais do grupo e link do video.

---

## 3. Definicao do MVP da Fase 1

### O que entra obrigatoriamente

- Cadastro de usuarios
- Login com JWT
- Perfis `Usuario` e `Administrador`
- Cadastro e administracao de jogos
- Biblioteca de jogos adquiridos
- Persistencia relacional com EF Core
- Migrations
- Swagger
- Middleware de erros
- Logs estruturados
- Testes unitarios
- DDD com Event Storming

### O que fica como opcional

- MongoDB
- Dapper
- GraphQL
- Domain Storytelling

Decisao recomendada:
- Implementar apenas o obrigatorio primeiro.
- So considerar opcionais depois de fechar o MVP com testes e documentacao.

---

## 4. Etapa 1 Ja Iniciada

Status atual: `[-] Em andamento`

### 4.1 Atores identificados

- `Usuario`
- `Administrador`

### 4.2 Subdominios iniciais

- `Identidade e Acesso`
  Responsavel por cadastro, autenticacao e autorizacao.
- `Catalogo de Jogos`
  Responsavel por cadastro e consulta de jogos.
- `Biblioteca`
  Responsavel pelos jogos adquiridos por cada usuario.
- `Administracao`
  Responsavel por operacoes de usuarios, jogos e promocoes.

### 4.3 Entidades iniciais

#### Usuario

- `Id`
- `Nome`
- `Email`
- `SenhaHash`
- `Role`
- `CreatedAt`
- `UpdatedAt`

#### Jogo

- `Id`
- `Titulo`
- `Descricao`
- `Preco`
- `Categoria`
- `Ativo`
- `CreatedAt`
- `UpdatedAt`

#### BibliotecaItem

- `Id`
- `UsuarioId`
- `JogoId`
- `DataAquisicao`

#### Promocao

Observacao:
- O enunciado cita criacao de promocoes para administrador.
- Se o tempo apertar, alinhar com o grupo se isso entrara com implementacao minima ou apenas estrutura inicial preparada.

### 4.4 Regras de negocio iniciais

- Um usuario deve possuir nome, email valido e senha forte.
- O email deve ser unico no sistema.
- A senha nao deve ser armazenada em texto puro.
- Apenas administradores podem cadastrar, alterar ou remover jogos.
- Um usuario autenticado pode consultar sua propria biblioteca.
- Um jogo nao deve ser vinculado mais de uma vez ao mesmo usuario, salvo regra explicita em contrario.

### 4.5 Fluxos obrigatorios mapeados

#### Fluxo 1 - Criacao de usuario

1. Cliente envia `nome`, `email` e `senha`
2. Sistema valida os campos
3. Sistema verifica se o email ja existe
4. Sistema gera hash da senha
5. Sistema salva o usuario
6. Sistema retorna sucesso

Eventos de dominio candidatos:
- `UsuarioCadastrado`

#### Fluxo 2 - Criacao de jogo

1. Administrador autentica
2. Administrador envia dados do jogo
3. Sistema valida permissao e payload
4. Sistema salva o jogo
5. Sistema retorna sucesso

Eventos de dominio candidatos:
- `JogoCadastrado`

#### Fluxo 3 - Login

1. Usuario informa email e senha
2. Sistema localiza o usuario
3. Sistema valida a senha
4. Sistema gera JWT
5. Sistema retorna token e perfil

#### Fluxo 4 - Vinculo de jogo a biblioteca

1. Administrador ou fluxo interno identifica compra/aquisicao
2. Sistema valida usuario e jogo
3. Sistema cria item na biblioteca
4. Sistema retorna sucesso

Evento de dominio candidato:
- `JogoAdicionadoNaBiblioteca`

### 4.6 Decisoes tecnicas iniciais

- Linguagem e framework: `C# + .NET 8`
- Estilo da API: `Controllers MVC`
- Persistencia: `Entity Framework Core` com banco relacional
- Autenticacao: `JWT Bearer`
- Testes: `xUnit`
- Documentacao: `Swagger / OpenAPI`
- Arquitetura: `Monolito modular`

### 4.7 Pendencias imediatas da Etapa 1

- [ ] Transformar estes fluxos em Event Storming visual no `Miro`
- [x] Definir se `Promocao` sera implementada na Fase 1 ou mantida como escopo tecnico minimo
- [ ] Fechar nomes padrao das entidades e casos de uso
- [x] Definir a estrutura de pastas da solucao

Observacao:
- `Promocao` entrou na Fase 1 com implementacao minima e objetiva: criacao administrativa e consulta autenticada.

---

## 5. Estrutura Recomendada da Solucao

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
Directory.Build.props
.gitignore
README.md
FASE1_CHECKLIST_EXECUCAO.md
```

---

## 6. Definition of Done

Um item so pode ser marcado como concluido quando:

- O codigo estiver versionado
- O comportamento estiver testado quando aplicavel
- O endpoint estiver documentado no Swagger quando aplicavel
- O impacto no `README.md` estiver refletido
- O time conseguir demonstrar o fluxo funcionando

---

## 7. Proximos Passos Imediatos

1. Gravar o video final.
2. Preencher os dados finais do relatorio.
3. Revisar checklist completo antes da entrega.
4. Publicar e registrar os links finais da entrega.
5. Confirmar a versao final que sera submetida.
