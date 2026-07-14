# Tech Challenge Fase 2 - Checklist e Execucao

Este arquivo e a fonte de verdade recomendada para a Fase 2. Ele transforma o enunciado em uma ordem de execucao objetiva, com criterios de aceite, entregaveis e decisoes tecnicas iniciais.

## Como usar este arquivo

- Atualize o status de cada item conforme o projeto avanca.
- Nao inicie uma etapa nova sem revisar os criterios de aceite da etapa atual.
- Registre decisoes tecnicas e links importantes neste mesmo arquivo ou em documentos referenciados por ele.
- Use este arquivo como base para os `README.md`, video de demonstracao e relatorio final.

## Legenda de status

- `[ ]` Nao iniciado
- `[-]` Em andamento
- `[x]` Concluido

---

## 1. Resumo do Desafio

Objetivo da Fase 2:

- Refatorar o MVP monolitico da Fase 1 para uma arquitetura de microsservicos orientada a eventos.
- Criar quatro servicos independentes:
  - `UsersAPI`
  - `CatalogAPI`
  - `PaymentsAPI`
  - `NotificationsAPI`
- Colocar cada microsservico em seu proprio repositorio Git.
- Containerizar todos os servicos com Docker.
- Orquestrar a execucao local com `docker-compose`.
- Implantar em Kubernetes local usando `Deployments`, `Services`, `ConfigMaps` e `Secrets`.
- Implementar comunicacao assincrona por mensageria com RabbitMQ ou Kafka.

Recomendacao tecnica inicial:

- Usar `.NET 8`.
- Usar `RabbitMQ + MassTransit`, por simplicidade e boa integracao com .NET.
- Criar um quinto repositorio de orquestracao, para centralizar `docker-compose.yml`, manifestos Kubernetes e README principal.

---

## 2. Checklist de Entrega

### 2.1 Microsservicos e Repositorios

- [x] Criar repositorio `fcg-users-api`
- [x] Criar repositorio `fcg-catalog-api`
- [x] Criar repositorio `fcg-payments-api`
- [x] Criar repositorio `fcg-notifications-api`
- [x] Criar repositorio opcional/recomendado `fcg-orchestration`
- [x] Cada repositorio deve ter sua propria solution `.sln` ou projeto `.csproj`
- [x] Cada repositorio deve ter README explicando finalidade e variaveis de ambiente
- [x] Remover acoplamento direto entre modulos que agora serao servicos separados

### 2.2 UsersAPI

- [x] Migrar cadastro de usuarios do monolito para `UsersAPI`
- [x] Migrar login e geracao de token JWT para `UsersAPI`
- [x] Manter autorizacao por roles quando necessario
- [x] Publicar evento `UserCreatedEvent` apos cadastro de usuario
- [x] Definir contrato do evento com campos minimos:
  - `UserId`
  - `Name`
  - `Email`
  - `CreatedAt`
- [x] Configurar conexao com broker por variavel de ambiente
- [x] Criar Dockerfile multi-stage
- [x] Criar manifestos Kubernetes em `/k8s`

Criterio de aceite:

- O usuario e cadastrado com sucesso.
- O login retorna JWT valido.
- A criacao do usuario publica `UserCreatedEvent`.
- O servico roda via Docker e Kubernetes local.

### 2.3 CatalogAPI

- [x] Migrar CRUD de jogos do monolito para `CatalogAPI`
- [x] Migrar biblioteca de jogos do usuario para `CatalogAPI`
- [x] Criar endpoint para iniciar compra/adicao de jogo a biblioteca
- [x] Ao iniciar compra, publicar `OrderPlacedEvent`
- [x] Definir contrato do evento com campos minimos:
  - `OrderId`
  - `UserId`
  - `GameId`
  - `Price`
  - `PlacedAt`
- [x] Preparar consumo de `PaymentProcessedEvent`
- [x] Se pagamento for `Approved`, adicionar jogo a biblioteca
- [x] Se pagamento for `Rejected`, nao adicionar jogo a biblioteca e registrar status
- [x] Configurar conexao com broker por variavel de ambiente
- [x] Criar Dockerfile multi-stage
- [x] Criar manifestos Kubernetes em `/k8s`

Criterio de aceite:

- O catalogo permite criar, listar, atualizar e desativar jogos.
- O endpoint de compra publica `OrderPlacedEvent`.
- O servico reage ao `PaymentProcessedEvent`.
- A biblioteca so recebe o jogo quando o pagamento for aprovado.

### 2.4 PaymentsAPI

- [x] Criar novo microsservico `PaymentsAPI`
- [x] Preparar consumo logico de `OrderPlacedEvent`
- [x] Simular processamento de pagamento
- [x] Publicar `PaymentProcessedEvent` por log ate a entrada do broker
- [x] Definir contrato do evento com campos minimos:
  - `OrderId`
  - `UserId`
  - `GameId`
  - `Price`
  - `Status`
  - `ProcessedAt`
- [x] Definir regra simples de aprovacao/rejeicao para demonstracao
- [x] Registrar logs claros do processamento no console
- [x] Configurar conexao com broker por variavel de ambiente
- [x] Criar Dockerfile multi-stage
- [x] Criar manifestos Kubernetes em `/k8s`

Criterio de aceite:

- Ao receber `OrderPlacedEvent`, o servico processa o pagamento.
- O servico publica `PaymentProcessedEvent` com status `Approved` ou `Rejected`.
- O fluxo completo funciona sem chamada HTTP direta entre CatalogAPI e PaymentsAPI.

### 2.5 NotificationsAPI

- [x] Criar novo microsservico `NotificationsAPI`
- [x] Preparar consumo logico de `UserCreatedEvent`
- [x] Simular envio de e-mail de boas-vindas por log no console
- [x] Preparar consumo logico de `PaymentProcessedEvent`
- [x] Se pagamento for `Approved`, simular envio de e-mail de confirmacao de compra
- [x] Ignorar ou logar pagamento rejeitado de forma clara
- [x] Configurar conexao com broker por variavel de ambiente
- [x] Criar Dockerfile multi-stage
- [x] Criar manifestos Kubernetes em `/k8s`

Criterio de aceite:

- O cadastro de usuario gera log de e-mail de boas-vindas.
- A compra aprovada gera log de e-mail de confirmacao.
- O servico nao depende de banco de dados para o MVP.

### 2.6 Mensageria

- [x] Escolher broker: RabbitMQ ou Kafka
- [x] Padronizar biblioteca .NET de mensageria
- [x] Criar projeto/pacote compartilhado de contratos de eventos ou duplicar contratos com versionamento claro
- [x] Definir nomes de filas/exchanges/topicos
- [x] Garantir que cada consumidor tenha sua propria fila/grupo
- [x] Configurar retry basico
- [x] Configurar logs de publicacao e consumo de eventos
- [x] Documentar os eventos no README principal

Decisao aplicada:

- RabbitMQ com MassTransit.
- Contratos duplicados entre repositorios com nomes de exchanges padronizados:
  - `fcg.user-created`
  - `fcg.order-placed`
  - `fcg.payment-processed`
- Filas configuradas:
  - `payments-order-placed`
  - `catalog-payment-processed`
  - `notifications-user-created`
  - `notifications-payment-processed`
- `RabbitMq__Enabled=false` mantem execucao local por log; `RabbitMq__Enabled=true` ativa MassTransit/RabbitMQ.
- Serializacao raw JSON aplicada para permitir contratos duplicados em namespaces diferentes.
- Retry basico configurado nos consumers: `3` tentativas com intervalo de `5` segundos.

Criterio de aceite:

- Eventos sao publicados e consumidos corretamente.
- RabbitMQ sobe junto com a aplicacao pelo `docker-compose`.
- O fluxo nao depende de chamadas sincronas entre os servicos principais.

Status atual: `[x] Concluido`

Resultado:

- RabbitMQ subiu localmente no container `fcg-rabbitmq`.
- Management UI validada em `http://localhost:15672`.
- Os quatro microsservicos rodaram com `RabbitMq__Enabled=true`.
- Fluxo validado ponta a ponta:
  - `UsersAPI` publicou `UserCreatedEvent`.
  - `NotificationsAPI` consumiu `UserCreatedEvent` e logou boas-vindas.
  - `CatalogAPI` publicou `OrderPlacedEvent`.
  - `PaymentsAPI` consumiu `OrderPlacedEvent` e publicou `PaymentProcessedEvent`.
  - `CatalogAPI` consumiu `PaymentProcessedEvent` e adicionou o jogo a biblioteca.
  - `NotificationsAPI` consumiu `PaymentProcessedEvent` e logou confirmacao de compra.
- Filas validadas com `0` mensagens pendentes e `1` consumidor cada:
  - `payments-order-placed`
  - `catalog-payment-processed`
  - `notifications-user-created`
  - `notifications-payment-processed`

Percentual faltante da etapa: `0%`

### 2.7 Docker e Docker Compose

- [x] Criar Dockerfile multi-stage em cada microsservico
- [x] Validar build individual de cada imagem
- [x] Criar `docker-compose.yml` no repositorio de orquestracao
- [x] Subir os quatro microsservicos
- [x] Subir o broker de mensageria
- [x] Subir bancos de dados quando aplicavel
- [x] Configurar variaveis de ambiente no compose
- [x] Garantir que a aplicacao completa rode com:

```powershell
docker-compose up --build
```

Criterio de aceite:

- Todos os containers sobem.
- As APIs ficam acessiveis localmente.
- O fluxo de cadastro e o fluxo de compra funcionam usando os containers.

Status atual: `[x] Concluido`

Resultado:

- Dockerfiles multi-stage criados para:
  - `fcg-users-api`
  - `fcg-catalog-api`
  - `fcg-payments-api`
  - `fcg-notifications-api`
- `compose/docker-compose.yml` criado em `fcg-orchestration`.
- `docker compose -f compose\docker-compose.yml config` validado com sucesso.
- `docker compose -f compose\docker-compose.yml up --build -d` executado com sucesso.
- Containers validados:
  - `compose-rabbitmq-1`
  - `compose-users-api-1`
  - `compose-catalog-api-1`
  - `compose-payments-api-1`
  - `compose-notifications-api-1`
- Health checks validados com HTTP `200`:
  - `http://localhost:5101/health`
  - `http://localhost:5102/health`
  - `http://localhost:5103/health`
  - `http://localhost:5104/health`
- RabbitMQ Management validado em `http://localhost:15672`.
- Fluxo completo validado dentro do Docker Compose:
  - cadastro de usuario
  - login do administrador
  - cadastro de jogo
  - login do usuario
  - inicio de compra
  - processamento de pagamento por evento
  - inclusao do jogo na biblioteca via `PaymentProcessedEvent`
- Filas validadas com `0` mensagens pendentes e `1` consumidor cada:
  - `payments-order-placed`
  - `catalog-payment-processed`
  - `notifications-user-created`
  - `notifications-payment-processed`

Percentual faltante da etapa: `0%`

### 2.8 Kubernetes

- [x] Criar pasta `/k8s` no repositorio de orquestracao
- [x] Criar `Deployment` para cada microsservico
- [x] Criar `Service` para cada microsservico
- [x] Criar `ConfigMap` para configuracoes nao sensiveis
- [x] Criar `Secret` para dados sensiveis
- [x] Criar manifests para RabbitMQ/Kafka ou usar dependencia documentada
- [x] Garantir comunicacao interna por nome de Service
- [x] Testar deploy em Kubernetes local:
  - Kind
  - Minikube
  - k3d
  - Docker Desktop Kubernetes
- [x] Validar pods:

```powershell
kubectl apply -f .
kubectl get pods
kubectl get services
```

Criterio de aceite:

- Todos os pods ficam em estado `Running`.
- Os servicos conseguem se comunicar dentro do cluster.
- O fluxo completo pode ser demonstrado em Kubernetes local.

Status atual: `[x] Concluido`

Resultado:

- Manifests Kubernetes criados em `C:\Users\Chrys\source\repos\fcg-orchestration\k8s`.
- Recursos criados:
  - `Namespace`
  - `ConfigMap`
  - `Secret`
  - `PersistentVolumeClaim`
  - `Deployment`
  - `Service`
- RabbitMQ configurado com service interno `rabbitmq` e NodePorts `30672`/`31672`.
- APIs configuradas com services NodePort:
  - `UsersAPI`: `30101`
  - `CatalogAPI`: `30102`
  - `PaymentsAPI`: `30103`
  - `NotificationsAPI`: `30104`
- `kubectl kustomize k8s` validado com sucesso.
- Cluster local criado com Kind usando `k8s/kind-config.yaml`.
- Imagens locais das quatro APIs carregadas no cluster Kind.
- Manifests aplicados com `kubectl apply -k k8s`.
- Pods validados em estado `Running` no namespace `fcg`.
- PVCs `users-data` e `catalog-data` validados como `Bound`.
- Health checks validados com HTTP `200`:
  - `http://localhost:30101/health`
  - `http://localhost:30102/health`
  - `http://localhost:30103/health`
  - `http://localhost:30104/health`
- Fluxo completo validado no Kubernetes:
  - cadastro de usuario
  - login do administrador
  - cadastro de jogo
  - login do usuario
  - inicio de compra
  - processamento de pagamento via RabbitMQ
  - inclusao do jogo na biblioteca por evento
- Filas RabbitMQ validadas com `0` mensagens pendentes e `1` consumidor cada.

Percentual faltante da etapa: `0%`

---

## 3. Ordem Recomendada de Execucao

### Etapa 1 - Planejamento da Decomposicao

Status atual: `[x] Concluido`

Objetivo:

- Definir exatamente o que sai do monolito e para qual microsservico vai.

Checklist:

- [x] Mapear funcionalidades atuais da Fase 1
- [x] Separar responsabilidades por servico
- [x] Definir dados pertencentes a cada servico
- [x] Definir contratos de eventos
- [x] Definir estrategia de repositorios
- [x] Definir estrategia de banco por servico
- [x] Definir padrao de portas locais

Sugestao de responsabilidades:

- `UsersAPI`
  - usuarios
  - autenticacao
  - JWT
  - evento de usuario criado
- `CatalogAPI`
  - jogos
  - biblioteca
  - inicio de compra
  - consumo de resultado de pagamento
- `PaymentsAPI`
  - simulacao de pagamento
  - publicacao de resultado
- `NotificationsAPI`
  - logs de e-mail de boas-vindas
  - logs de e-mail de confirmacao de compra
- `fcg-orchestration`
  - docker-compose
  - manifests Kubernetes consolidados ou referencias aos manifests dos repositorios
  - README principal da execucao

Criterio de aceite:

- Existe uma lista clara de repositorios.
- Existe um desenho dos eventos.
- O time sabe qual servico e dono de cada dado.

Resultado:

- Planejamento registrado em `docs/fase2/PLANEJAMENTO_DECOMPOSICAO.md`.
- Broker recomendado: `RabbitMQ` com `MassTransit`.
- Repositorio de orquestracao recomendado: `fcg-orchestration`.
- Bancos separados definidos para `UsersAPI` e `CatalogAPI`; banco opcional para `PaymentsAPI`; sem banco para `NotificationsAPI`.
- Portas locais padronizadas entre `5101` e `5104`.

Percentual faltante da etapa: `0%`

### Etapa 2 - Criacao dos Repositorios e Bases .NET

Status atual: `[x] Concluido`

Objetivo:

- Criar a base independente dos quatro microsservicos.

Checklist:

- [x] Criar solutions/projetos .NET 8
- [x] Configurar Swagger em cada API HTTP
- [x] Configurar logs em console
- [x] Configurar health check simples
- [x] Padronizar nomes, portas e variaveis de ambiente
- [x] Criar README inicial de cada repositorio

Criterio de aceite:

- Cada servico compila de forma isolada.
- Cada servico sobe localmente.
- Cada API possui Swagger ou endpoint de health para demonstracao.

Resultado:

- Repositorios locais criados em:
  - `C:\Users\Chrys\source\repos\fcg-users-api`
  - `C:\Users\Chrys\source\repos\fcg-catalog-api`
  - `C:\Users\Chrys\source\repos\fcg-payments-api`
  - `C:\Users\Chrys\source\repos\fcg-notifications-api`
  - `C:\Users\Chrys\source\repos\fcg-orchestration`
- Cada API possui `.sln`, projeto `net8.0`, Swagger, logs em console, endpoint `/health` e README inicial.
- Portas locais validadas:
  - `UsersAPI`: `http://localhost:5101`
  - `CatalogAPI`: `http://localhost:5102`
  - `PaymentsAPI`: `http://localhost:5103`
  - `NotificationsAPI`: `http://localhost:5104`
- Validacoes executadas com sucesso:
  - `dotnet restore`
  - `dotnet build --no-restore`
  - `GET /health` nos quatro servicos

Percentual faltante da etapa: `0%`

### Etapa 3 - Migracao do UsersAPI

Status atual: `[x] Concluido`

Objetivo:

- Extrair usuarios e autenticacao da Fase 1.

Checklist:

- [x] Migrar entidade e regras de `User`
- [x] Migrar cadastro de usuario
- [x] Migrar login
- [x] Migrar hash de senha
- [x] Migrar geracao JWT
- [x] Criar persistencia propria
- [x] Publicar `UserCreatedEvent`
- [x] Testar cadastro e login

Criterio de aceite:

- O UsersAPI funciona sem depender do monolito.
- O evento de usuario criado e publicado.

Resultado:

- `UsersAPI` migrado para `C:\Users\Chrys\source\repos\fcg-users-api`.
- Camadas criadas:
  - `Fcg.Users.Api`
  - `Fcg.Users.Application`
  - `Fcg.Users.Domain`
  - `Fcg.Users.Infrastructure`
  - `Fcg.Users.UnitTests`
- Endpoints migrados:
  - `POST /usuarios`
  - `POST /auth/login`
  - `GET /usuarios/me`
  - `GET /usuarios`
  - `PATCH /usuarios/{userId}/perfil`
- Persistencia propria criada com `UsersDbContext` e SQLite.
- Administrador local semeado automaticamente:
  - `admin@fcg.local`
  - `Admin@123`
- `UserCreatedEvent` criado e publicado via abstracao `IEventPublisher`; nesta etapa, a infraestrutura usa `LoggingEventPublisher` para logar o evento ate a entrada de RabbitMQ/MassTransit.
- Validacoes executadas com sucesso:
  - `dotnet restore`
  - `dotnet build --no-restore`
  - `dotnet test --no-build --no-restore`
  - cadastro, login e `GET /usuarios/me` via HTTP em `http://localhost:5101`

Percentual faltante da etapa: `0%`

### Etapa 4 - Migracao do CatalogAPI

Status atual: `[x] Concluido`

Objetivo:

- Extrair catalogo, jogos e biblioteca.

Checklist:

- [x] Migrar entidade e regras de `Game`
- [x] Migrar endpoints de jogos
- [x] Migrar biblioteca
- [x] Criar endpoint de compra
- [x] Publicar `OrderPlacedEvent`
- [x] Preparar consumo de `PaymentProcessedEvent`
- [x] Persistir item na biblioteca somente se pagamento aprovado
- [x] Testar fluxo isolado com evento fake ou broker local

Criterio de aceite:

- O catalogo funciona de forma independente.
- A compra inicia um fluxo por evento.
- A biblioteca e atualizada apenas apos pagamento aprovado.

Resultado:

- `CatalogAPI` migrado para `C:\Users\Chrys\source\repos\fcg-catalog-api`.
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
- `OrderPlacedEvent` criado e publicado via abstracao `IEventPublisher`; nesta etapa, a infraestrutura usa `LoggingEventPublisher` ate a entrada de RabbitMQ/MassTransit.
- `PaymentProcessedEvent` criado e tratado por `ProcessPaymentUseCase`; a ligacao real com fila fica para a etapa de mensageria.
- Validacoes executadas com sucesso:
  - `dotnet restore`
  - `dotnet build --no-restore`
  - `dotnet test --no-build --no-restore`
  - `GET /health`, `POST /jogos` e `POST /compras` via HTTP em `http://localhost:5102`

Percentual faltante da etapa: `0%`

### Etapa 5 - Criacao do PaymentsAPI

Status atual: `[x] Concluido`

Objetivo:

- Criar processamento simulado de pagamento.

Checklist:

- [x] Criar entrada logica para `OrderPlacedEvent`
- [x] Implementar regra de simulacao
- [x] Publicar `PaymentProcessedEvent` via abstracao `IEventPublisher`
- [x] Adicionar logs claros para demonstracao
- [x] Testar processamento e publicacao logica sem broker

Criterio de aceite:

- O PaymentsAPI processa pedidos usando o contrato de evento.
- O resultado do pagamento e publicado pela abstracao de eventos.
- A ligacao real com RabbitMQ/MassTransit fica para a etapa de mensageria.

Resultado:

- `PaymentsAPI` implementado em `C:\Users\Chrys\source\repos\fcg-payments-api`.
- Camadas criadas:
  - `Fcg.Payments.Api`
  - `Fcg.Payments.Application`
  - `Fcg.Payments.Infrastructure`
  - `Fcg.Payments.UnitTests`
- Contratos criados:
  - `OrderPlacedEvent`
  - `PaymentProcessedEvent`
- Endpoint criado para validacao isolada:
  - `POST /pagamentos/processar`
- Regra de simulacao:
  - preco maior que zero aprova pagamento por padrao
  - preco menor ou igual a zero rejeita pagamento
  - `PaymentSimulation__DefaultStatus=Rejected` forca rejeicao para demonstracao
- `PaymentProcessedEvent` e publicado por `LoggingEventPublisher` ate a entrada de RabbitMQ/MassTransit.
- Validacoes executadas com sucesso:
  - `dotnet restore`
  - `dotnet build --no-restore`
  - `dotnet test --no-build --no-restore`
  - `GET /health` e `POST /pagamentos/processar` via HTTP em `http://localhost:5103`

Percentual faltante da etapa: `0%`

### Etapa 6 - Criacao do NotificationsAPI

Status atual: `[x] Concluido`

Objetivo:

- Criar simulacao de notificacoes por log.

Checklist:

- [x] Criar entrada logica para `UserCreatedEvent`
- [x] Logar e-mail de boas-vindas
- [x] Criar entrada logica para `PaymentProcessedEvent`
- [x] Logar e-mail de confirmacao de compra aprovada
- [x] Testar os dois fluxos sem broker

Criterio de aceite:

- O NotificationsAPI reage aos eventos corretos.
- Os logs sao claros o suficiente para aparecerem no video.
- A ligacao real com RabbitMQ/MassTransit fica para a etapa de mensageria.

Resultado:

- `NotificationsAPI` implementado em `C:\Users\Chrys\source\repos\fcg-notifications-api`.
- Camadas criadas:
  - `Fcg.Notifications.Api`
  - `Fcg.Notifications.Application`
  - `Fcg.Notifications.Infrastructure`
  - `Fcg.Notifications.UnitTests`
- Contratos criados:
  - `UserCreatedEvent`
  - `PaymentProcessedEvent`
- Endpoints criados para validacao isolada:
  - `POST /notificacoes/usuario-criado`
  - `POST /notificacoes/pagamento-processado`
- Regras de notificacao:
  - usuario criado gera log de e-mail de boas-vindas
  - pagamento aprovado gera log de confirmacao de compra
  - pagamento rejeitado e ignorado com resposta clara
- Validacoes executadas com sucesso:
  - `dotnet restore`
  - `dotnet build --no-restore`
  - `dotnet test --no-build --no-restore`
  - `GET /health`, `POST /notificacoes/usuario-criado` e `POST /notificacoes/pagamento-processado` via HTTP em `http://localhost:5104`

Percentual faltante da etapa: `0%`

### Etapa 7 - Orquestracao com Docker Compose

Status atual: `[x] Concluido`

Objetivo:

- Permitir que toda a solucao suba com um unico comando.

Checklist:

- [x] Criar Dockerfile multi-stage em cada servico
- [x] Criar compose com APIs, broker e bancos
- [x] Configurar redes e variaveis de ambiente
- [x] Testar `docker-compose up --build`
- [x] Testar cadastro de usuario completo
- [x] Testar compra de jogo completa
- [x] Documentar comandos no README principal

Criterio de aceite:

- A aplicacao completa roda via Docker Compose.
- Os fluxos obrigatorios podem ser demonstrados localmente.

Resultado:

- Dockerfiles multi-stage criados nos quatro microsservicos.
- Docker Compose criado em `C:\Users\Chrys\source\repos\fcg-orchestration\compose\docker-compose.yml`.
- RabbitMQ, UsersAPI, CatalogAPI, PaymentsAPI e NotificationsAPI subiram via Compose.
- APIs acessiveis nas portas `5101`, `5102`, `5103` e `5104`.
- RabbitMQ acessivel em `5672` e Management UI em `15672`.
- Fluxo de cadastro e compra validado usando apenas containers.

Percentual faltante da etapa: `0%`

### Etapa 8 - Orquestracao com Kubernetes

Status atual: `[x] Concluido`

Objetivo:

- Criar deploy local em Kubernetes com boas praticas minimas.

Checklist:

- [x] Criar manifests de `Deployment`
- [x] Criar manifests de `Service`
- [x] Criar `ConfigMap`
- [x] Criar `Secret`
- [x] Criar manifests do broker ou documentar instalacao
- [x] Aplicar manifests em cluster local
- [x] Validar pods e services
- [x] Testar fluxo completo no cluster

Criterio de aceite:

- `kubectl get pods` mostra todos os pods rodando.
- Os servicos usam nomes de `Service` para comunicacao interna.
- O video consegue demonstrar os manifests e o deploy.

Resultado:

- Manifests criados no repositorio `fcg-orchestration`.
- Renderizacao validada com `kubectl kustomize k8s`.
- Cluster local criado com Kind.
- Manifests aplicados com `kubectl apply -k k8s`.
- Pods, services, PVCs, health checks, filas e fluxo completo validados no cluster.

Percentual faltante da etapa: `0%`

### Etapa 9 - Qualidade, Testes e Observabilidade

Status atual: `[x] Concluido`

Objetivo:

- Garantir confiabilidade minima dos fluxos principais.

Checklist:

- [x] Testes unitarios para regras migradas
- [x] Testes de publicacao de eventos quando aplicavel
- [x] Testes de handlers/consumers quando aplicavel
- [x] Health checks por servico
- [x] Logs padronizados com correlationId quando possivel
- [x] Tratamento global de erros nas APIs HTTP

Criterio de aceite:

- As regras criticas possuem testes.
- Falhas comuns retornam resposta consistente.
- Logs ajudam a demonstrar o fluxo no video.

Resultado:

- Testes unitarios executados nos quatro microsservicos.
- Health checks criados em todas as APIs.
- Logs em console configurados.
- Tratamento de erros e validacoes mantidos nas APIs.
- Fluxos principais validados por HTTP, Docker Compose e Kubernetes.

Percentual faltante da etapa: `0%`

### Etapa 10 - Documentacao e Entrega

Status atual: `[-] Em andamento`

Objetivo:

- Fechar os entregaveis academicos.

Checklist:

- [x] README em cada microsservico
- [x] README principal no repositorio de orquestracao
- [x] Documentar arquitetura e fluxo de eventos
- [x] Documentar como executar com Docker Compose
- [x] Documentar como fazer deploy em Kubernetes
- [ ] Gravar video de ate 20 minutos
- [x] Criar relatorio final em PDF ou TXT
- [ ] Incluir nome do grupo
- [ ] Incluir participantes e usernames no Discord
- [x] Incluir link da documentacao
- [x] Incluir links dos repositorios
- [ ] Incluir link do video

Criterio de aceite:

- Qualquer avaliador consegue executar o projeto seguindo o README.
- O video demonstra todos os requisitos obrigatorios.
- O relatorio possui todos os links de entrega.

Resultado parcial:

- README principal atualizado no `fcg-orchestration`.
- Documentos criados:
  - `docs/arquitetura.md`
  - `docs/eventos.md`
  - `docs/ROTEIRO_VIDEO_FASE2.md`
  - `RELATORIO_ENTREGA_FASE2.txt`
- Relatorio final em TXT criado com links dos repositorios.
- Pendencias externas ao codigo:
  - nome do grupo
  - participantes e usernames no Discord
  - gravacao do video
  - link do video

Percentual faltante da etapa: `25%`

---

## 4. Fluxos Obrigatorios

### 4.1 Fluxo de Cadastro de Usuario

1. Cliente chama `POST /usuarios` no `UsersAPI`.
2. `UsersAPI` valida os dados.
3. `UsersAPI` cria o usuario.
4. `UsersAPI` publica `UserCreatedEvent`.
5. `NotificationsAPI` consome `UserCreatedEvent`.
6. `NotificationsAPI` simula envio de e-mail de boas-vindas por log.

Eventos:

- `UserCreatedEvent`

Criterio de aceite:

- O cadastro aparece na resposta HTTP do `UsersAPI`.
- O evento aparece no broker/log.
- O log de boas-vindas aparece no `NotificationsAPI`.

### 4.2 Fluxo de Compra de Jogo

1. Cliente chama endpoint de compra no `CatalogAPI`.
2. `CatalogAPI` valida usuario, jogo e preco.
3. `CatalogAPI` publica `OrderPlacedEvent`.
4. `PaymentsAPI` consome `OrderPlacedEvent`.
5. `PaymentsAPI` simula pagamento.
6. `PaymentsAPI` publica `PaymentProcessedEvent`.
7. `CatalogAPI` consome `PaymentProcessedEvent`.
8. Se o status for `Approved`, `CatalogAPI` adiciona o jogo a biblioteca.
9. `NotificationsAPI` consome `PaymentProcessedEvent`.
10. Se o status for `Approved`, `NotificationsAPI` simula e-mail de confirmacao por log.

Eventos:

- `OrderPlacedEvent`
- `PaymentProcessedEvent`

Criterio de aceite:

- O pedido de compra nao adiciona o jogo imediatamente sem pagamento.
- O pagamento aprovado adiciona o jogo a biblioteca.
- O pagamento aprovado gera notificacao.
- O pagamento rejeitado nao adiciona o jogo a biblioteca.

---

## 5. Estrutura Recomendada dos Repositorios

### 5.1 Microsservico

```text
/src
  /Fcg.Users.Api
  /Fcg.Users.Application
  /Fcg.Users.Domain
  /Fcg.Users.Infrastructure
/tests
  /Fcg.Users.UnitTests
/k8s
  deployment.yaml
  service.yaml
  configmap.yaml
  secret.yaml
Dockerfile
README.md
Fcg.Users.sln
```

Observacao:

- A estrutura acima pode ser simplificada para os servicos novos `PaymentsAPI` e `NotificationsAPI`, se o escopo for pequeno.
- O importante e manter cada servico independente e facil de executar.

### 5.2 Repositorio de Orquestracao

```text
/compose
  docker-compose.yml
/k8s
  users-api
  catalog-api
  payments-api
  notifications-api
  rabbitmq
/docs
  arquitetura.md
  eventos.md
README.md
RELATORIO_ENTREGA_FASE2.txt
```

---

## 6. Variaveis de Ambiente Recomendadas

### Comuns

- `ASPNETCORE_ENVIRONMENT`
- `ASPNETCORE_URLS`
- `RabbitMq__Host`
- `RabbitMq__Username`
- `RabbitMq__Password`

### UsersAPI

- `ConnectionStrings__UsersDb`
- `Jwt__Issuer`
- `Jwt__Audience`
- `Jwt__Secret`
- `Jwt__ExpirationMinutes`

### CatalogAPI

- `ConnectionStrings__CatalogDb`
- `Jwt__Issuer`
- `Jwt__Audience`
- `Jwt__Secret`

### PaymentsAPI

- `PaymentSimulation__DefaultStatus`

### NotificationsAPI

- `Notifications__FromEmail`

---

## 7. Comandos de Validacao

### Build local por servico

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build --no-restore
```

### Docker Compose

```powershell
docker-compose up --build
docker-compose ps
docker-compose logs -f
```

### Kubernetes

```powershell
kubectl apply -f k8s
kubectl get pods
kubectl get services
kubectl logs deployment/users-api
kubectl logs deployment/catalog-api
kubectl logs deployment/payments-api
kubectl logs deployment/notifications-api
```

---

## 8. Roteiro Recomendado para o Video

1. Apresentar rapidamente o objetivo da Fase 2.
2. Mostrar os quatro repositorios dos microsservicos.
3. Mostrar o repositorio de orquestracao, se existir.
4. Explicar o fluxo de eventos:
   - `UserCreatedEvent`
   - `OrderPlacedEvent`
   - `PaymentProcessedEvent`
5. Subir tudo com `docker-compose up --build`.
6. Demonstrar cadastro de usuario.
7. Mostrar log de boas-vindas no `NotificationsAPI`.
8. Demonstrar cadastro/listagem de jogo.
9. Demonstrar compra de jogo.
10. Mostrar `PaymentsAPI` processando o pagamento.
11. Mostrar `CatalogAPI` adicionando o jogo a biblioteca.
12. Mostrar `NotificationsAPI` enviando confirmacao de compra.
13. Mostrar os manifestos Kubernetes:
    - `Deployment`
    - `Service`
    - `ConfigMap`
    - `Secret`
14. Executar `kubectl apply -f .`.
15. Executar `kubectl get pods`.
16. Encerrar mostrando os links de repositorios e documentacao.

---

## 9. Definition of Done

Um item so pode ser marcado como concluido quando:

- O codigo estiver versionado no repositorio correto.
- O servico compilar isoladamente.
- O servico tiver Dockerfile funcional.
- O servico tiver manifests Kubernetes obrigatorios.
- O comportamento principal estiver testado quando aplicavel.
- O README estiver atualizado.
- O fluxo puder ser demonstrado em Docker Compose.
- O deploy puder ser demonstrado em Kubernetes local.

---

## 10. Proximos Passos Imediatos

1. Preencher nome do grupo no relatorio final.
2. Preencher participantes e usernames no Discord.
3. Gravar o video de demonstracao.
4. Inserir link do video no relatorio final.
5. Revisar o pacote final antes da submissao.
