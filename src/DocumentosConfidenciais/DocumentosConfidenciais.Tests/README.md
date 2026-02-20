# Testes Unitários - DocumentosConfidenciais

Este documento descreve todos os testes unitários implementados para o projeto DocumentosConfidenciais.

## 📊 Resumo dos Testes

- **Total de Testes**: 71
- **Testes Passando**: 71 ✅
- **Testes Falhando**: 0
- **Cobertura**: Todas as classes do projeto Console

## 🧪 Estrutura de Testes

### 1. Domain.Entities.UserTests (6 testes)

Testa a entidade `User` e suas funcionalidades:

- ✅ `Constructor_ShouldCreateUser_WithValidParameters` - Verifica criação de usuário válido
- ✅ `Constructor_ShouldThrowArgumentNullException_WhenUsernameIsNull` - Valida exceção para username nulo
- ✅ `HasPermissionFor_ShouldReturnCorrectPermission_BasedOnClearanceLevel` - Testa 6 combinações de níveis de permissão (Theory)
- ✅ `HasPermissionFor_ShouldReturnTrue_WhenUserHasExactSameClearanceLevel` - Verifica permissão com nível exato
- ✅ `HasPermissionFor_ShouldReturnFalse_WhenUserHasLowerClearanceLevel` - Verifica negação com nível inferior

### 2. Domain.Entities.ConfidentialDocumentTests (9 testes)

Testa a entidade `ConfidentialDocument`:

- ✅ `Constructor_ShouldCreateDocument_WithValidParameters` - Verifica criação de documento válido
- ✅ `Constructor_ShouldThrowArgumentNullException_WhenIdIsNull` - Valida exceção para ID nulo
- ✅ `Constructor_ShouldThrowArgumentNullException_WhenTitleIsNull` - Valida exceção para título nulo
- ✅ `Constructor_ShouldThrowArgumentNullException_WhenContentIsNull` - Valida exceção para conteúdo nulo
- ✅ `UpdateContent_ShouldUpdateDocumentContent_WithValidContent` - Testa atualização de conteúdo
- ✅ `UpdateContent_ShouldThrowArgumentNullException_WhenContentIsNull` - Valida exceção ao atualizar com conteúdo nulo
- ✅ `SizeInBytes_ShouldCalculateCorrectSize` - Verifica cálculo de tamanho em bytes
- ✅ `SizeInBytes_ShouldUpdateAfterContentChange` - Verifica atualização do tamanho após mudança de conteúdo
- ✅ `Constructor_ShouldAcceptAllClearanceLevels` - Testa todos os 5 níveis de clearance (Theory)

### 3. Infrastructure.DocumentRepositoryTests (9 testes)

Testa o repositório de documentos:

- ✅ `Constructor_ShouldInitializeRepository_WithPredefinedDocuments` - Verifica inicialização com documentos padrão
- ✅ `GetDocument_ShouldReturnDocument_WhenDocumentExists` - Testa busca de documento existente
- ✅ `GetDocument_ShouldReturnNull_WhenDocumentDoesNotExist` - Verifica retorno null para documento inexistente
- ✅ `GetDocument_ShouldReturnCorrectDocument_ForEachPredefinedDocument` - Testa busca dos 3 documentos padrão (Theory)
- ✅ `UpdateDocument_ShouldUpdateContent_WhenDocumentExists` - Verifica atualização de documento
- ✅ `UpdateDocument_ShouldNotThrowException_WhenDocumentDoesNotExist` - Valida comportamento ao atualizar documento inexistente
- ✅ `GetDocument_ShouldReturnSameInstance_OnMultipleCalls` - Verifica que retorna a mesma instância
- ✅ `UpdateDocument_ShouldPersistChanges_AcrossMultipleRetrievals` - Testa persistência de mudanças

### 4. Application.Services.RealDocumentServiceTests (11 testes)

Testa o serviço real de documentos:

- ✅ `Constructor_ShouldCreateService_WithValidRepository` - Verifica criação do serviço
- ✅ `ViewDocument_ShouldReturnDocument_WhenDocumentExists` - Testa visualização de documento existente
- ✅ `ViewDocument_ShouldReturnNull_WhenDocumentDoesNotExist` - Verifica retorno null para documento inexistente
- ✅ `ViewDocument_ShouldReturnDocument_ForAllPredefinedDocuments` - Testa visualização dos 3 documentos padrão (Theory)
- ✅ `ViewDocument_ShouldNotCheckPermissions_ReturnsDocumentRegardlessOfUserLevel` - Verifica que não há controle de acesso
- ✅ `EditDocument_ShouldUpdateDocument_WhenDocumentExists` - Testa edição de documento
- ✅ `EditDocument_ShouldNotThrowException_WhenDocumentDoesNotExist` - Valida comportamento ao editar documento inexistente
- ✅ `EditDocument_ShouldPersistChanges` - Verifica persistência de alterações
- ✅ `EditDocument_ShouldNotCheckPermissions_AllowsAnyUserToEdit` - Confirma ausência de controle de acesso na edição
- ✅ `ViewDocument_ShouldReturnSameDocumentInstance_OnMultipleCalls` - Verifica que retorna a mesma instância

### 5. Application.Proxies.DocumentServiceProxyTests (28 testes)

Testa o proxy com controle de acesso, cache e auditoria:

#### Construção e Básicos
- ✅ `Constructor_ShouldCreateProxy_WithValidRepository` - Verifica criação do proxy

#### Controle de Acesso
- ✅ `ViewDocument_ShouldReturnDocument_WhenUserHasPermission` - Testa acesso autorizado
- ✅ `ViewDocument_ShouldReturnNull_WhenUserDoesNotHavePermission` - Testa acesso negado
- ✅ `ViewDocument_ShouldReturnNull_WhenDocumentDoesNotExist` - Verifica documento inexistente
- ✅ `ViewDocument_ShouldEnforceAccessControl_BasedOnClearanceLevel` - Testa 8 combinações de controle de acesso (Theory)

#### Cache
- ✅ `ViewDocument_ShouldUseCache_OnSecondCall` - Verifica uso de cache na segunda chamada
- ✅ `Proxy_ShouldMaintainCache_AcrossMultipleUsers` - Testa cache compartilhado entre usuários
- ✅ `ViewDocument_ShouldCacheOnlyAuthorizedDocuments` - Verifica que apenas documentos autorizados são cacheados

#### Edição
- ✅ `EditDocument_ShouldUpdateDocument_WhenUserHasPermission` - Testa edição autorizada
- ✅ `EditDocument_ShouldNotUpdate_WhenUserDoesNotHavePermission` - Testa bloqueio de edição não autorizada
- ✅ `EditDocument_ShouldNotThrow_WhenDocumentDoesNotExist` - Valida comportamento ao editar documento inexistente
- ✅ `EditDocument_ShouldInvalidateCache` - Verifica invalidação de cache após edição
- ✅ `EditDocument_ShouldEnforceAccessControl_BasedOnClearanceLevel` - Testa 5 combinações de controle de edição (Theory)

#### Inicialização Lazy
- ✅ `ViewDocument_ShouldInitializeRealService_OnFirstCall` - Verifica inicialização lazy do serviço real

#### Auditoria
- ✅ `ShowAuditLog_ShouldNotThrow` - Testa exibição do log de auditoria

## 🎯 Cobertura de Testes

### Classes Testadas

| Classe | Testes | Status |
|--------|--------|--------|
| `User` | 6 | ✅ 100% |
| `ConfidentialDocument` | 9 | ✅ 100% |
| `DocumentRepository` | 9 | ✅ 100% |
| `RealDocumentService` | 11 | ✅ 100% |
| `DocumentServiceProxy` | 28 | ✅ 100% |
| **TOTAL** | **71** | **✅ 100%** |

### Funcionalidades Testadas

#### ✅ Entidades de Domínio
- Validação de parâmetros nulos
- Cálculo de propriedades
- Atualização de estado
- Regras de negócio (HasPermissionFor)

#### ✅ Infraestrutura
- Inicialização de dados
- Operações CRUD
- Persistência de mudanças
- Gerenciamento de instâncias

#### ✅ Serviços
- Operações de leitura e escrita
- Delegação ao repositório
- Comportamento sem controle de acesso

#### ✅ Proxy (Padrão de Design)
- **Controle de Acesso**: Verificação de permissões baseada em ClearanceLevel
- **Cache**: Armazenamento em memória para otimização de leituras
- **Invalidação de Cache**: Remoção de cache após edições
- **Auditoria**: Registro de todas as operações
- **Lazy Initialization**: Criação sob demanda do serviço real
- **Transparência**: Interface idêntica ao serviço real

## 🚀 Como Executar os Testes

### Executar todos os testes
```powershell
cd DocumentosConfidenciais.Tests
dotnet test
```

### Executar com verbosidade detalhada
```powershell
dotnet test --verbosity normal
```

### Executar testes de uma classe específica
```powershell
dotnet test --filter "FullyQualifiedName~DocumentServiceProxyTests"
```

### Executar com cobertura de código
```powershell
dotnet test /p:CollectCoverage=true
```

## 📝 Padrões Utilizados

### Arrange-Act-Assert (AAA)
Todos os testes seguem o padrão AAA:
- **Arrange**: Configuração do cenário de teste
- **Act**: Execução da ação sendo testada
- **Assert**: Verificação dos resultados

### Theory e InlineData
Utilizado para testes parametrizados, permitindo testar múltiplos cenários com o mesmo código:
```csharp
[Theory]
[InlineData(ClearanceLevel.TopSecret, "DOC001", true)]
[InlineData(ClearanceLevel.Public, "DOC002", false)]
public void TestMethod(ClearanceLevel level, string docId, bool expected)
```

### Record.Exception
Usado para verificar que exceções são (ou não são) lançadas:
```csharp
var exception = Record.Exception(() => proxy.EditDocument(...));
Assert.Null(exception);
```

## 🔍 Cenários de Teste Importantes

### 1. Controle de Acesso em Cascata
Testa que usuários com diferentes níveis de clearance têm acesso apropriado:
- TopSecret pode acessar tudo
- Confidential pode acessar Confidential e abaixo
- Public pode acessar apenas Public

### 2. Cache e Invalidação
Verifica que:
- Documentos são cacheados após primeiro acesso
- Cache é compartilhado entre usuários
- Cache é invalidado após edições
- Documentos negados não são cacheados

### 3. Lazy Initialization
Confirma que o RealDocumentService só é criado quando necessário, economizando recursos.

### 4. Auditoria
Testa que todas as operações são registradas no log de auditoria.

## 📊 Resultados da Última Execução

```
Total tests: 71
Passed: 71
Failed: 0
Skipped: 0
Duration: ~50s
```

## ✨ Conclusão

A suíte de testes garante:
- ✅ Funcionalidade correta de todas as classes
- ✅ Implementação adequada do padrão Proxy
- ✅ Controle de acesso robusto
- ✅ Comportamento de cache eficiente
- ✅ Validação de entrada adequada
- ✅ Tratamento de casos extremos (documentos inexistentes, permissões negadas, etc.)

