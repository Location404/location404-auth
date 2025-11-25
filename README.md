# 🔐 Location404 Auth Service

Serviço de autenticação e gerenciamento de usuários para o Location404 - sistema completo de registro, login, tokens JWT e perfis de usuário.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Database-336791?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![JWT](https://img.shields.io/badge/JWT-Auth-000000?logo=jsonwebtokens)](https://jwt.io/)
[![BCrypt](https://img.shields.io/badge/BCrypt-Hashing-00ADD8)](https://github.com/BcryptNet/bcrypt.net)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

## 📋 Índice

- [Sobre o Projeto](#-sobre-o-projeto)
- [Funcionalidades](#-funcionalidades)
- [Arquitetura](#-arquitetura)
- [Tecnologias](#-tecnologias)
- [Pré-requisitos](#-pré-requisitos)
- [Instalação](#-instalação)
- [Configuração](#%EF%B8%8F-configuração)
- [Como Usar](#-como-usar)
- [API Endpoints](#-api-endpoints)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Testes](#-testes)
- [Observabilidade](#-observabilidade)
- [Licença](#-licença)

## 🎯 Sobre o Projeto

O **Location404 Auth Service** é o serviço de autenticação centralizado do Location404. Utilizando **JWT (JSON Web Tokens)** e **BCrypt** para hashing de senhas, o serviço fornece:

- **Registro de Usuários**: Criação de contas com email/senha
- **Autenticação**: Login seguro com JWT
- **Refresh Tokens**: Renovação automática de tokens
- **Gestão de Perfil**: Atualização de informações e avatar
- **Segurança**: BCrypt para senhas, HttpOnly cookies, CORS configurável

### Como Funciona

1. **Usuário se registra** → Senha hasheada com BCrypt, usuário criado no PostgreSQL
2. **Login realizado** → Validação de credenciais, geração de JWT + Refresh Token
3. **Tokens retornados** → Access Token (15min) e Refresh Token (7 dias) em HttpOnly cookies
4. **Requisições autenticadas** → Frontend envia cookies automaticamente
5. **Token expirado** → Frontend chama `/refresh` para renovar tokens
6. **Perfil atualizado** → Upload de imagem (validação de tipo e tamanho)

## ✨ Funcionalidades

### Autenticação
- ✅ Login com email e senha
- ✅ JWT com expiração configurável (padrão: 60min)
- ✅ Refresh Tokens com rotação automática (padrão: 24h)
- ✅ HttpOnly cookies para segurança
- ✅ Logout (invalidação de refresh token)

### Gestão de Usuários
- ✅ Registro com validação de email único
- ✅ Atualização de perfil (username, bio)
- ✅ Upload de imagem de perfil
- ✅ Validação de tipos de arquivo (JPEG, PNG, GIF)
- ✅ Limite de tamanho de imagem (5MB)

### Segurança
- ✅ Hashing de senhas com BCrypt (work factor 10)
- ✅ Validação de força de senha (min 6 caracteres)
- ✅ Tokens armazenados em HttpOnly cookies (proteção XSS)
- ✅ CORS configurável por ambiente
- ✅ Rate limiting (opcional, via middleware)

### Observabilidade
- ✅ OpenTelemetry integrado
- ✅ Traces distribuídos (login, registro, refresh)
- ✅ Métricas customizadas (usuários criados, logins, falhas)
- ✅ Logs estruturados com trace correlation

## 🏗️ Arquitetura

O projeto segue **Clean Architecture** com separação clara de responsabilidades:

```
┌─────────────────────────────────────────────────────────────┐
│                  API Layer (Minimal APIs)                    │
│  ┌─────────────┐  ┌──────────────┐  ┌──────────────────┐   │
│  │    Auth     │  │    User      │  │  Filters &       │   │
│  │  Endpoints  │  │  Endpoints   │  │  Middlewares     │   │
│  └─────────────┘  └──────────────┘  └──────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                            │
┌─────────────────────────────────────────────────────────────┐
│                    Application Layer                         │
│  ┌──────────────────┐  ┌──────────────────────────────┐    │
│  │ Commands/Queries │  │  Interfaces & Validators     │    │
│  │ - CreateUser     │  │  - IUserRepository           │    │
│  │ - Authenticate   │  │  - IPasswordHasher           │    │
│  │ - RefreshToken   │  │  - ITokenGenerator           │    │
│  └──────────────────┘  └──────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                            │
┌─────────────────────────────────────────────────────────────┐
│                      Domain Layer                            │
│  ┌──────────────┐  ┌──────────────┐  ┌─────────────────┐   │
│  │  Entities    │  │ Value Objects│  │  Domain Events  │   │
│  │  - User      │  │  - Email     │  │  - UserCreated  │   │
│  │  - RefreshTkn│  │  - Password  │  │  - UserAuth'd   │   │
│  └──────────────┘  └──────────────┘  └─────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                            │
┌─────────────────────────────────────────────────────────────┐
│                  Infrastructure Layer                        │
│  ┌──────────┐  ┌─────────┐  ┌──────────┐  ┌─────────────┐ │
│  │PostgreSQL│  │ BCrypt  │  │   JWT    │  │File Storage │ │
│  │ (EF Core)│  │(Hashing)│  │ (Tokens) │  │ (Images)    │ │
│  └──────────┘  └─────────┘  └──────────┘  └─────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

### Camadas

#### 🔵 API Layer (Minimal APIs)
- **Endpoints**: AuthenticationEndpoints, UserManagementEndpoints
- **Filters**: ValidationFilter (FluentValidation)
- **Middlewares**: CORS, Authentication, Exception Handling

#### 🟢 Application Layer (CQRS)
- **Commands**: CreateUser, Login, RefreshToken, UpdateUser
- **Queries**: GetCurrentUserInformation
- **Validators**: FluentValidation rules
- **Interfaces**: IPasswordHasher, ITokenGenerator

#### 🟡 Domain Layer
- **Entities**: User (aggregate root)
- **Value Objects**: Email, Password, ProfileImage
- **Domain Events**: UserCreated, UserAuthenticated

#### 🔴 Infrastructure Layer
- **Persistence**: EF Core + PostgreSQL
- **Security**: BCrypt.Net para hashing
- **JWT**: System.IdentityModel.Tokens.Jwt
- **File Storage**: Local filesystem (profile images)

### Fluxo de Autenticação

```
Frontend
    │
    ├─ POST /api/users ──────► CreateUserCommand
    │                          │
    │                          ├─ Validate email unique
    │                          ├─ Hash password (BCrypt)
    │                          ├─ Store in PostgreSQL
    │                          │
    ├─◄ 201 Created            User created
    │
    ├─ POST /api/auth/login ──► AuthenticateCommand
    │                          │
    │                          ├─ Find user by email
    │                          ├─ Verify password (BCrypt.Verify)
    │                          ├─ Generate JWT (claims: sub, email, username)
    │                          ├─ Generate Refresh Token (GUID)
    │                          ├─ Store Refresh Token in DB
    │                          │
    ├─◄ 200 OK                 Set HttpOnly Cookies:
    │   (+ cookies)            - accessToken (15min)
    │                          - refreshToken (7 days)
    │
    ├─ GET /api/users/me ─────► GetCurrentUserQuery
    │   (Authorization: JWT)   │
    │                          ├─ Extract userId from JWT claims
    │                          ├─ Fetch user from PostgreSQL
    │                          │
    ├─◄ 200 OK                 User information
    │
    ├─ POST /api/auth/refresh ► RefreshTokenCommand
    │   (refreshToken cookie)  │
    │                          ├─ Validate refresh token
    │                          ├─ Check expiration
    │                          ├─ Generate new JWT
    │                          ├─ Rotate refresh token (optional)
    │                          │
    ├─◄ 200 OK                 New tokens in cookies
```

## 🛠️ Tecnologias

### Backend
- **.NET 9.0** - Framework principal
- **ASP.NET Core Minimal APIs** - Endpoints RESTful
- **LiteBus** - CQRS pattern (Command/Query handlers)
- **FluentValidation** - Validação de requests

### Database & ORM
- **PostgreSQL 16** - Banco de dados relacional
- **Entity Framework Core 9** - ORM
- **Npgsql** - Provider PostgreSQL

### Security
- **BCrypt.Net** - Password hashing
- **System.IdentityModel.Tokens.Jwt** - JWT generation/validation
- **Microsoft.AspNetCore.Authentication.JwtBearer** - JWT middleware

### Observability
- **OpenTelemetry** - Distributed tracing
- **Shared.Observability** - Pacote NuGet customizado
- **Prometheus** - Métricas
- **Grafana Loki** - Logs estruturados

### Testing
- **xUnit** - Framework de testes
- **FluentAssertions** - Assertions expressivas
- **Moq** - Mocking
- **EF Core InMemory** - Testes de repositório

## 📦 Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL 16+](https://www.postgresql.org/download/)

**Opcional:**
- [Docker](https://www.docker.com/) - Para rodar PostgreSQL via containers

## 🚀 Instalação

### 1. Clone o repositório

```bash
git clone https://github.com/Location404/location404-auth.git
cd location404-auth
```

### 2. Restaurar dependências

```bash
dotnet restore
```

### 3. Build do projeto

```bash
dotnet build
```

### 4. Aplicar migrations (banco de dados)

```bash
cd src/Location404.Auth.API
dotnet ef database update --project ../Location404.Auth.Infrastructure
```

## ⚙️ Configuração

### appsettings.json

Edite `src/Location404.Auth.API/appsettings.json` ou use **variáveis de ambiente**:

```json
{
  "ConnectionStrings": {
    "UserIdentityDatabaseProduction": "Host=localhost;Port=5432;Database=location404_auth;Username=postgres;Password=your_password"
  },

  "JwtSettings": {
    "Issuer": "location404",
    "Audience": "location404",
    "SigningKey": "your-super-secret-key-min-32-chars-here",
    "AccessTokenMinutes": 60,
    "RefreshTokenMinutes": 1440
  },

  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173",
      "http://localhost:4200"
    ]
  }
}
```

### Variáveis de Ambiente (Docker/Produção)

```bash
# Database
ConnectionStrings__UserIdentityDatabaseProduction=Host=postgres;Port=5432;Database=location404_auth;Username=location404;Password=secure_password

# JWT
JwtSettings__SigningKey=your-super-secret-signing-key-min-32-characters
JwtSettings__AccessTokenMinutes=60
JwtSettings__RefreshTokenMinutes=1440

# CORS
Cors__AllowedOrigins__0=https://location404.com

# OpenTelemetry
OpenTelemetry__CollectorEndpoint=http://otel-collector:4317
OpenTelemetry__Tracing__SamplingRatio=0.1
```

### Gerar SigningKey Seguro

```bash
# Linux/macOS
openssl rand -base64 32

# Windows (PowerShell)
[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Minimum 0 -Maximum 256 }))
```

## 🎮 Como Usar

### Desenvolvimento Local

```bash
# 1. Inicie o PostgreSQL (ou use Docker)
docker run -d \
  --name postgres-auth \
  -e POSTGRES_DB=location404_auth \
  -e POSTGRES_USER=location404 \
  -e POSTGRES_PASSWORD=dev_password \
  -p 5432:5432 \
  postgres:16-alpine

# 2. Aplique as migrations
cd src/Location404.Auth.API
dotnet ef database update --project ../Location404.Auth.Infrastructure

# 3. Execute o serviço
dotnet run
```

A API estará disponível em:
- **Base URL**: `http://localhost:5185`
- **Swagger/Scalar**: `http://localhost:5185/scalar/v1`
- **Health Check**: `http://localhost:5185/health`
- **Metrics**: `http://localhost:5185/metrics`

### Docker Compose (Recomendado)

```bash
cd location404-utils/deploy/dev
docker-compose up -d location404-auth postgres
```

## 📡 API Endpoints

### Authentication

#### POST `/api/auth/login`

Autentica um usuário com email e senha.

**Request:**
```json
{
  "email": "user@example.com",
  "password": "SecurePassword123"
}
```

**Response (200 OK):**
```json
{
  "userId": "guid-here",
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "guid-refresh-token",
  "expiresIn": 3600
}
```

**Cookies Set:**
- `accessToken` - HttpOnly, Expires: 15min
- `refreshToken` - HttpOnly, Expires: 7 days, Path: `/api/auth/refresh`

**Status Codes:**
- `200 OK` - Login bem-sucedido
- `401 Unauthorized` - Credenciais inválidas

---

#### POST `/api/auth/refresh`

Renova o access token usando o refresh token.

**Request:**
- Requer cookie `refreshToken`

**Response (200 OK):**
```json
{
  "accessToken": "new-jwt-token",
  "expiresIn": 3600
}
```

**Status Codes:**
- `200 OK` - Token renovado com sucesso
- `401 Unauthorized` - Refresh token inválido ou expirado

---

### User Management

#### POST `/api/users`

Cria um novo usuário.

**Request:**
```json
{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "SecurePass123"
}
```

**Response (201 Created):**
```json
{
  "id": "guid-here",
  "username": "johndoe",
  "email": "john@example.com",
  "createdAt": "2025-11-25T12:00:00Z"
}
```

**Validations:**
- Email único (não pode duplicar)
- Senha mínima: 6 caracteres
- Username: 3-20 caracteres, alfanumérico

**Status Codes:**
- `201 Created` - Usuário criado com sucesso
- `400 Bad Request` - Validação falhou
- `409 Conflict` - Email já cadastrado

---

#### GET `/api/users/me`

Retorna informações do usuário autenticado.

**Headers:**
```
Authorization: Bearer {jwt-token}
```

**Response (200 OK):**
```json
{
  "id": "guid-here",
  "username": "johndoe",
  "email": "john@example.com",
  "profileImageUrl": "https://cdn.location404.com/profiles/guid.jpg",
  "bio": "Passionate gamer and traveler",
  "createdAt": "2025-11-25T12:00:00Z"
}
```

**Status Codes:**
- `200 OK` - Usuário encontrado
- `401 Unauthorized` - Token inválido ou ausente
- `404 Not Found` - Usuário não encontrado

---

#### PATCH `/api/users/{id}`

Atualiza informações do usuário.

**Headers:**
```
Authorization: Bearer {jwt-token}
Content-Type: multipart/form-data
```

**Request (multipart/form-data):**
```
username: newusername
bio: Updated bio text
profileImage: [binary file data]
```

**Validations:**
- ProfileImage: Max 5MB
- Tipos aceitos: JPEG, PNG, GIF
- Username: 3-20 caracteres

**Response (204 No Content)**

**Status Codes:**
- `204 No Content` - Atualização bem-sucedida
- `400 Bad Request` - Validação falhou (arquivo muito grande, tipo inválido)
- `401 Unauthorized` - Token inválido
- `403 Forbidden` - Tentativa de atualizar outro usuário
- `404 Not Found` - Usuário não encontrado

---

#### POST `/api/users/external`

Cria usuário via autenticação externa (Google, etc).

**Request:**
```json
{
  "provider": "Google",
  "providerUserId": "google-id-123",
  "email": "user@gmail.com",
  "username": "johndoe",
  "profileImageUrl": "https://lh3.googleusercontent.com/..."
}
```

**Response (201 Created):**
```json
{
  "id": "guid-here",
  "username": "johndoe",
  "email": "user@gmail.com",
  "provider": "Google"
}
```

**Status Codes:**
- `201 Created` - Usuário criado
- `400 Bad Request` - Provider inválido
- `409 Conflict` - Email já cadastrado

---

### Autenticação de Requests

Para endpoints protegidos, inclua o JWT no header:

```bash
curl -H "Authorization: Bearer {your-jwt-token}" \
     http://localhost:5185/api/users/me
```

Ou configure o cliente HTTP para enviar cookies automaticamente:

```typescript
// Axios
axios.defaults.withCredentials = true;

// Fetch
fetch('/api/users/me', {
  credentials: 'include'
});
```

## 📂 Estrutura do Projeto

```
location404-auth/
├── src/
│   ├── Location404.Auth.API/                    # API Layer (Minimal APIs)
│   │   ├── Endpoints/
│   │   │   ├── AuthenticationEndpoints.cs       # Login, Refresh
│   │   │   └── UserManagementEndpoints.cs       # User CRUD
│   │   ├── Filters/
│   │   │   └── ValidationFilter.cs              # FluentValidation filter
│   │   ├── Program.cs                           # Entry point + DI setup
│   │   └── appsettings.json
│   │
│   ├── Location404.Auth.Application/            # Application Layer (CQRS)
│   │   ├── Features/
│   │   │   ├── Authentication/
│   │   │   │   └── Commands/
│   │   │   │       ├── AuthenticateUserWithPasswordCommand/
│   │   │   │       └── RefreshTokenCommand/
│   │   │   │
│   │   │   └── UserManagement/
│   │   │       ├── Commands/
│   │   │       │   ├── CreateUserWithPasswordCommand/
│   │   │       │   ├── CreateUserWithExternalProviderCommand/
│   │   │       │   └── UpdateUserInformationsCommand/
│   │   │       └── Queries/
│   │   │           └── GetCurrentUserInformation/
│   │   │
│   │   ├── Common/
│   │   │   ├── Result/                          # Result pattern
│   │   │   └── Interfaces/
│   │   │       ├── IPasswordHasher.cs
│   │   │       ├── ITokenGenerator.cs
│   │   │       └── IUserRepository.cs
│   │   │
│   │   └── DTOs/
│   │       ├── UserDto.cs
│   │       └── TokenDto.cs
│   │
│   ├── Location404.Auth.Domain/                 # Domain Layer
│   │   ├── Entities/
│   │   │   ├── User.cs                          # Aggregate root
│   │   │   └── RefreshToken.cs
│   │   │
│   │   └── ValueObjects/
│   │       ├── Email.cs
│   │       ├── Password.cs
│   │       └── ProfileImage.cs
│   │
│   └── Location404.Auth.Infrastructure/         # Infrastructure
│       ├── Persistence/
│       │   ├── ApplicationDbContext.cs          # EF Core DbContext
│       │   ├── Configurations/
│       │   │   └── UserConfiguration.cs         # Entity configuration
│       │   └── Repositories/
│       │       └── UserRepository.cs            # Repository implementation
│       │
│       ├── Security/
│       │   ├── BcryptPasswordHasher.cs          # BCrypt implementation
│       │   └── JwtTokenGenerator.cs             # JWT generation
│       │
│       ├── FileStorage/
│       │   └── LocalFileStorageService.cs       # Profile image storage
│       │
│       ├── Migrations/                          # EF Core migrations
│       │   ├── 20250101000000_InitialCreate.cs
│       │   └── ApplicationDbContextModelSnapshot.cs
│       │
│       └── DependencyInjection.cs
│
├── tests/
│   ├── Location404.Auth.Application.UnitTests/
│   │   ├── Commands/
│   │   │   ├── AuthenticateUserTests.cs
│   │   │   ├── CreateUserTests.cs
│   │   │   └── RefreshTokenTests.cs
│   │   └── Validators/
│   │       └── CreateUserValidatorTests.cs
│   │
│   └── Location404.Auth.Domain.UnitTests/
│       ├── Entities/
│       │   └── UserTests.cs
│       └── ValueObjects/
│           ├── EmailTests.cs
│           └── PasswordTests.cs
│
├── Location404.Auth.sln
├── README.md
└── .gitignore
```

## 🧪 Testes

### Executar Todos os Testes

```bash
dotnet test
```

### Testes Unitários (Domain Layer)

```bash
dotnet test tests/Location404.Auth.Domain.UnitTests
```

### Testes de Application (Commands/Queries)

```bash
dotnet test tests/Location404.Auth.Application.UnitTests
```

### Cobertura de Código

```bash
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"TestResults/Report"
```

Abra `TestResults/Report/index.html` no navegador.

## 📊 Observabilidade

### Métricas (Prometheus)

Endpoint: `http://localhost:5185/metrics`

**Métricas customizadas:**
- `auth_users_created_total` - Total de usuários criados
- `auth_logins_total` - Total de logins (success/failure)
- `auth_token_refreshes_total` - Total de refresh tokens
- `auth_password_hash_duration_seconds` - Tempo de hashing BCrypt

### Traces (OpenTelemetry)

Configurado para exportar para coletor OTLP:
- Endpoint: `http://181.215.135.221:4317`
- Sampling: 10% em produção, 100% em desenvolvimento

**Traces automáticos:**
- HTTP requests (inbound)
- Database queries (EF Core)
- External API calls
- Command/Query handlers

### Logs (Structured)

Logs estruturados exportados para Grafana Loki:
- Formato: JSON
- Trace correlation: `trace_id`, `span_id`
- Enriched com properties: `user_id`, `email`, `command_name`

### Health Checks

```bash
# Health geral
curl http://localhost:5185/health

# Readiness (dependências prontas?)
curl http://localhost:5185/health/ready

# Liveness (processo vivo?)
curl http://localhost:5185/health/live
```

**Dependências verificadas:**
- PostgreSQL (timeout: 5s)
- File system (writable, para uploads)

## 📄 Licença

Este projeto está sob a licença **MIT**. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

## 🔗 Links Relacionados

- [location404-web](https://github.com/Location404/location404-web) - Frontend Vue.js
- [location404-game](https://github.com/Location404/location404-game) - Game engine SignalR
- [location404-data](https://github.com/Location404/location404-data) - API de dados e estatísticas
- [shared-observability](https://github.com/Location404/shared-observability) - Pacote de observabilidade

## 📞 Suporte

- **Issues**: [GitHub Issues](https://github.com/Location404/location404-auth/issues)
- **Discussões**: [GitHub Discussions](https://github.com/Location404/location404-auth/discussions)

---

<p align="center">
  Desenvolvido por <a href="https://github.com/ryanbromati">ryanbromati</a>
</p>
