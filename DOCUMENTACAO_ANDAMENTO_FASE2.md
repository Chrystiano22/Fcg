# Fase 2 - Documentacao de Andamento

Este documento consolida o estado atual da Fase 2, o que ja foi feito, o que ainda falta e qual e a proxima frente de trabalho. Ele deve ser atualizado sempre que uma etapa do `FASE2_CHECKLIST_EXECUCAO.md` avancar.

## 1. Estado atual

- Escopo considerado: requisitos obrigatorios da Fase 2.
- Progresso estimado da entrega obrigatoria: `98%`
- Percentual faltante estimado da entrega obrigatoria: `2%`
- Arquitetura alvo: microsservicos orientados a eventos.
- Stack definida: `.NET 8`, `RabbitMQ`, `MassTransit`, `Docker`, `docker-compose` e Kubernetes local.
- Repositorios locais criados:
  - `C:\Users\Chrys\source\repos\fcg-users-api`
  - `C:\Users\Chrys\source\repos\fcg-catalog-api`
  - `C:\Users\Chrys\source\repos\fcg-payments-api`
  - `C:\Users\Chrys\source\repos\fcg-notifications-api`
  - `C:\Users\Chrys\source\repos\fcg-orchestration`

## 2. O que ja foi feito

### 2.1 Planejamento da decomposicao

- O monolito da Fase 1 foi mapeado por responsabilidade.
- Foram definidos quatro microsservicos:
  - `UsersAPI`
  - `CatalogAPI`
  - `PaymentsAPI`
  - `NotificationsAPI`
- Foi definido um repositorio de orquestracao:
  - `fcg-orchestration`
- Foram definidos os donos dos dados por servico.
- Foram definidos os eventos principais:
  - `UserCreatedEvent`
  - `OrderPlacedEvent`
  - `PaymentProcessedEvent`
- Foi criado o documento:
  - `docs/fase2/PLANEJAMENTO_DECOMPOSICAO.md`

Percentual faltante desta etapa: `0%`

### 2.2 Bases dos repositorios

- Os cinco repositorios locais foram criados.
- Cada microsservico recebeu uma solution/projeto `.NET 8`.
- Cada API recebeu:
  - Swagger
  - endpoint `/health`
  - logs em console
  - README inicial
  - porta local padronizada
- Portas definidas:
  - `UsersAPI`: `5101`
  - `CatalogAPI`: `5102`
  - `PaymentsAPI`: `5103`
  - `NotificationsAPI`: `5104`
- Validacoes realizadas:
  - `dotnet restore`
  - `dotnet build --no-restore`
  - `GET /health` nos quatro servicos

Percentual faltante desta etapa: `0%`

### 2.3 UsersAPI

- O `UsersAPI` foi migrado do monolito para o repositorio `fcg-users-api`.
- Camadas criadas:
  - `Fcg.Users.Api`
  - `Fcg.Users.Application`
  - `Fcg.Users.Domain`
  - `Fcg.Users.Infrastructure`
  - `Fcg.Users.UnitTests`
- Funcionalidades migradas:
  - cadastro de usuario
  - login
  - geracao de JWT
  - autorizacao por role
  - consulta do usuario autenticado
  - listagem administrativa de usuarios
  - alteracao administrativa de perfil
  - hash de senha com PBKDF2
  - persistencia propria com `UsersDbContext`
- Endpoints migrados:
  - `POST /usuarios`
  - `POST /auth/login`
  - `GET /usuarios/me`
  - `GET /usuarios`
  - `PATCH /usuarios/{userId}/perfil`
- Administrador local semeado automaticamente:
  - `admin@fcg.local`
  - `Admin@123`
- `UserCreatedEvent` foi criado.
- A publicacao do evento foi abstraida por `IEventPublisher`.
- Ate a entrada da mensageria real, o evento e registrado por `LoggingEventPublisher`.
- Validacoes realizadas:
  - `dotnet restore`
  - `dotnet build --no-restore`
  - `dotnet test --no-build --no-restore`
  - `29` testes aprovados
  - cadastro, login e `GET /usuarios/me` validados via HTTP em `http://localhost:5101`

Percentual faltante desta etapa: `0%`

### 2.4 CatalogAPI

- O `CatalogAPI` foi migrado do monolito para o repositorio `fcg-catalog-api`.
- Camadas criadas:
  - `Fcg.Catalog.Api`
  - `Fcg.Catalog.Application`
  - `Fcg.Catalog.Domain`
  - `Fcg.Catalog.Infrastructure`
  - `Fcg.Catalog.UnitTests`
- Dominios migrados:
  - `Game`
  - `Promotion`
  - `LibraryItem`
- Funcionalidades migradas:
  - CRUD de jogos
  - consulta de promocoes
  - criacao de promocoes
  - consulta da biblioteca do usuario
  - inicio do fluxo de compra
- Endpoints migrados/criados:
  - `GET /jogos`
  - `POST /jogos`
  - `PUT /jogos/{gameId}`
  - `DELETE /jogos/{gameId}`
  - `GET /promocoes`
  - `POST /promocoes`
  - `GET /biblioteca`
  - `POST /compras`
- Persistencia propria criada com `CatalogDbContext` e SQLite.
- `OrderPlacedEvent` foi criado.
- A publicacao do evento foi abstraida por `IEventPublisher`.
- Ate a entrada da mensageria real, o evento e registrado por `LoggingEventPublisher`.
- `PaymentProcessedEvent` foi criado.
- `ProcessPaymentUseCase` prepara a regra de adicionar jogo a biblioteca somente quando o pagamento for aprovado.
- Validacoes realizadas:
  - `dotnet restore`
  - `dotnet build --no-restore`
  - `dotnet test --no-build --no-restore`
  - `37` testes aprovados
  - `GET /health`, `POST /jogos` e `POST /compras` validados via HTTP em `http://localhost:5102`

Percentual faltante desta etapa: `0%`

### 2.5 PaymentsAPI

- O `PaymentsAPI` foi criado no repositorio `fcg-payments-api`.
- Camadas criadas:
  - `Fcg.Payments.Api`
  - `Fcg.Payments.Application`
  - `Fcg.Payments.Infrastructure`
  - `Fcg.Payments.UnitTests`
- Funcionalidades implementadas:
  - processamento logico do contrato `OrderPlacedEvent`
  - simulacao de pagamento aprovado/rejeitado
  - publicacao logica de `PaymentProcessedEvent`
  - logs claros de processamento e publicacao
- Endpoint criado para validacao isolada:
  - `POST /pagamentos/processar`
- Regra de simulacao:
  - preco maior que zero aprova pagamento por padrao
  - preco menor ou igual a zero rejeita pagamento
  - `PaymentSimulation__DefaultStatus=Rejected` forca rejeicao
- Ate a entrada da mensageria real, o evento e registrado por `LoggingEventPublisher`.
- Validacoes realizadas:
  - `dotnet restore`
  - `dotnet build --no-restore`
  - `dotnet test --no-build --no-restore`
  - `3` testes aprovados
  - `GET /health` e `POST /pagamentos/processar` validados via HTTP em `http://localhost:5103`

Percentual faltante desta etapa: `0%`

### 2.6 NotificationsAPI

- O `NotificationsAPI` foi criado no repositorio `fcg-notifications-api`.
- Camadas criadas:
  - `Fcg.Notifications.Api`
  - `Fcg.Notifications.Application`
  - `Fcg.Notifications.Infrastructure`
  - `Fcg.Notifications.UnitTests`
- Funcionalidades implementadas:
  - processamento logico do contrato `UserCreatedEvent`
  - processamento logico do contrato `PaymentProcessedEvent`
  - log de e-mail de boas-vindas para usuario criado
  - log de confirmacao de compra para pagamento aprovado
  - ignorar pagamento rejeitado com resposta clara
- Endpoints criados para validacao isolada:
  - `POST /notificacoes/usuario-criado`
  - `POST /notificacoes/pagamento-processado`
- Ate a entrada da mensageria real, os eventos sao simulados pelos endpoints de validacao.
- Validacoes realizadas:
  - `dotnet restore`
  - `dotnet build --no-restore`
  - `dotnet test --no-build --no-restore`
  - `5` testes aprovados
  - `GET /health`, `POST /notificacoes/usuario-criado` e `POST /notificacoes/pagamento-processado` validados via HTTP em `http://localhost:5104`

Percentual faltante desta etapa: `0%`

### 2.7 Mensageria

- RabbitMQ foi mantido como broker escolhido.
- MassTransit/RabbitMQ foi adicionado aos projetos de infraestrutura:
  - `fcg-users-api`
  - `fcg-catalog-api`
  - `fcg-payments-api`
  - `fcg-notifications-api`
- Publicadores reais foram preparados:
  - `UsersAPI` publica `UserCreatedEvent`
  - `CatalogAPI` publica `OrderPlacedEvent`
  - `PaymentsAPI` publica `PaymentProcessedEvent`
- Consumers reais foram preparados:
  - `PaymentsAPI` consome `OrderPlacedEvent`
  - `CatalogAPI` consome `PaymentProcessedEvent`
  - `NotificationsAPI` consome `UserCreatedEvent`
  - `NotificationsAPI` consome `PaymentProcessedEvent`
- Exchanges padronizados:
  - `fcg.user-created`
  - `fcg.order-placed`
  - `fcg.payment-processed`
- Filas padronizadas:
  - `payments-order-placed`
  - `catalog-payment-processed`
  - `notifications-user-created`
  - `notifications-payment-processed`
- A configuracao `RabbitMq__Enabled=false` mantem a execucao local por log.
- A configuracao `RabbitMq__Enabled=true` ativa MassTransit/RabbitMQ.
- Serializacao raw JSON foi aplicada para permitir contratos duplicados em namespaces diferentes.
- Retry basico foi configurado nos consumers:
  - `3` tentativas
  - intervalo de `5` segundos
- Validacoes realizadas:
  - `dotnet restore`
  - `dotnet build --no-restore`
  - `dotnet test --no-build --no-restore`
  - `29` testes aprovados no `UsersAPI`
  - `37` testes aprovados no `CatalogAPI`
  - `3` testes aprovados no `PaymentsAPI`
  - `5` testes aprovados no `NotificationsAPI`
  - Docker Desktop iniciado com sucesso
  - RabbitMQ iniciado no container `fcg-rabbitmq`
  - Management UI validada em `http://localhost:15672`
  - quatro microsservicos executados com `RabbitMq__Enabled=true`
  - fluxo completo de cadastro e compra validado por eventos
  - filas validadas com `0` mensagens pendentes e `1` consumidor cada

Percentual faltante desta etapa: `0%`

## 3. O que falta fazer

### 3.1 Docker e docker-compose

- Dockerfiles multi-stage criados nos quatro microsservicos.
- `compose/docker-compose.yml` criado no `fcg-orchestration`.
- APIs, RabbitMQ e volumes de dados subiram via Docker Compose.
- Execucao validada com:

```powershell
docker compose -f compose\docker-compose.yml up --build -d
```

- Health checks validados nas quatro APIs.
- RabbitMQ Management validado em `http://localhost:15672`.
- Fluxo completo de cadastro, compra, pagamento e biblioteca validado usando apenas containers.
- Filas RabbitMQ validadas com `0` mensagens pendentes e `1` consumidor cada.

Percentual faltante desta etapa: `0%`

### 3.2 Kubernetes

- Manifests Kubernetes criados no repositorio `fcg-orchestration`.
- Recursos criados:
  - `Namespace`
  - `ConfigMap`
  - `Secret`
  - `PersistentVolumeClaim`
  - `Deployment`
  - `Service`
- RabbitMQ configurado no cluster com service interno `rabbitmq`.
- APIs configuradas com NodePort para demonstracao local:
  - `UsersAPI`: `30101`
  - `CatalogAPI`: `30102`
  - `PaymentsAPI`: `30103`
  - `NotificationsAPI`: `30104`
- Renderizacao validada com:

```powershell
kubectl kustomize k8s
```

- Validacao realizada com:

```powershell
kubectl apply -k k8s
kubectl get pods -n fcg
kubectl get services -n fcg
```

- Cluster local criado com Kind.
- Imagens locais das quatro APIs carregadas no cluster Kind.
- Pods validados em estado `Running`.
- PVCs `users-data` e `catalog-data` validados como `Bound`.
- Health checks das quatro APIs validados via NodePort.
- Fluxo completo de cadastro, compra, pagamento e biblioteca validado dentro do Kubernetes.
- Filas RabbitMQ validadas com `0` mensagens pendentes e `1` consumidor cada.

Percentual faltante desta etapa: `0%`

### 3.3 Documentacao e entrega

- README de cada microsservico criado.
- README principal criado no `fcg-orchestration`.
- Arquitetura documentada em `docs/arquitetura.md`.
- Eventos documentados em `docs/eventos.md`.
- Roteiro do video criado em `docs/ROTEIRO_VIDEO_FASE2.md`.
- Relatorio final TXT criado em `RELATORIO_ENTREGA_FASE2.txt`.
- Links dos repositorios incluidos no relatorio final.
- Ainda falta preencher:
  - nome do grupo
  - participantes e usernames no Discord
  - link do video apos a gravacao

Percentual faltante desta etapa: `25%`

## 4. Decisoes tecnicas atuais

- `RabbitMQ + MassTransit` sera a abordagem de mensageria.
- Cada servico tera seu proprio repositorio.
- Cada servico com estado tera seu proprio banco.
- `UsersAPI` e `CatalogAPI` usam SQLite no MVP local.
- `PaymentsAPI` pode seguir sem banco inicialmente, usando log e evento como prova de processamento.
- `NotificationsAPI` nao precisa de banco para o MVP.
- Contratos HTTP seguem em portugues quando herdados da Fase 1.
- Codigo interno segue nomenclatura em ingles.

## 5. Proximo passo recomendado

1. Preencher nome do grupo no relatorio final.
2. Preencher participantes e usernames no Discord.
3. Gravar o video de demonstracao.
4. Inserir link do video no relatorio final.
5. Revisar o pacote final antes da submissao.

## 6. Como validar o estado atual

### UsersAPI

```powershell
cd C:\Users\Chrys\source\repos\fcg-users-api
dotnet restore
dotnet build --no-restore
dotnet test --no-build --no-restore
dotnet run --project src\Fcg.Users.Api --launch-profile http
```

Swagger:

- `http://localhost:5101/swagger`

Health check:

- `http://localhost:5101/health`

Fluxo minimo:

1. `POST /usuarios`
2. `POST /auth/login`
3. `GET /usuarios/me` com token JWT

### PaymentsAPI

```powershell
cd C:\Users\Chrys\source\repos\fcg-payments-api
dotnet restore
dotnet build --no-restore
dotnet test --no-build --no-restore
dotnet run --project src\Fcg.Payments.Api --launch-profile http
```

Swagger:

- `http://localhost:5103/swagger`

Health check:

- `http://localhost:5103/health`

Fluxo minimo:

1. `POST /pagamentos/processar`
2. Conferir resposta com `status` igual a `Approved` ou `Rejected`

### NotificationsAPI

```powershell
cd C:\Users\Chrys\source\repos\fcg-notifications-api
dotnet restore
dotnet build --no-restore
dotnet test --no-build --no-restore
dotnet run --project src\Fcg.Notifications.Api --launch-profile http
```

Swagger:

- `http://localhost:5104/swagger`

Health check:

- `http://localhost:5104/health`

Fluxo minimo:

1. `POST /notificacoes/usuario-criado`
2. `POST /notificacoes/pagamento-processado` com `status` igual a `Approved`
3. `POST /notificacoes/pagamento-processado` com `status` igual a `Rejected`
