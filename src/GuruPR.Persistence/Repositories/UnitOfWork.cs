using System.Transactions;

using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Persistence.Contexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace GuruPR.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly GuruDbContext _guruDbContext;
    private readonly UserManagementDbContext _userManagementDbContext;

    private IDbContextTransaction? _guruTransaction;
    private IDbContextTransaction? _userManagementTransaction;
    private TransactionScope? _transactionScope;

    private IToolRepository? _toolRepository;
    private IAgentRepository? _agentRepository;
    private IMessageRepository? _messageRepository;
    private IProviderRepository? _providerRepository;
    private IConversationRepository? _conversationRepository;

    private IUserRepository? _userRepository;

    public UnitOfWork(GuruDbContext guruDbContext, UserManagementDbContext userManagementDbContext)
    {
        _guruDbContext = guruDbContext;
        _userManagementDbContext = userManagementDbContext;
    }

    public IToolRepository Tools => _toolRepository ??= new ToolRepository(_guruDbContext);
    public IAgentRepository Agents => _agentRepository ??= new AgentRepository(_guruDbContext);
    public IMessageRepository Messages => _messageRepository ??= new MessageRepository(_guruDbContext);
    public IProviderRepository Providers => _providerRepository ??= new ProviderRepository(_guruDbContext);
    public IConversationRepository Conversations => _conversationRepository ??= new ConversationRepository(_guruDbContext);

    public IUserRepository Users => _userRepository ??= new UserRepository(_userManagementDbContext);

    public Task BeginDistributedTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transactionScope != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _transactionScope = new TransactionScope(TransactionScopeOption.Required,
                                                 new TransactionOptions
                                                 {
                                                     IsolationLevel = IsolationLevel.ReadCommitted,
                                                     Timeout = TransactionManager.DefaultTimeout
                                                 },
                                                 TransactionScopeAsyncFlowOption.Enabled);

        return Task.CompletedTask;
    }

    public async Task CommitDistributedTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transactionScope == null)
        {
            throw new InvalidOperationException("No distributed transaction in progress.");
        }

        try
        {
            await _guruDbContext.SaveChangesAsync(cancellationToken);
            await _userManagementDbContext.SaveChangesAsync(cancellationToken);

            _transactionScope.Complete();
        }
        finally
        {
            _transactionScope.Dispose();
            _transactionScope = null;
        }
    }

    public Task RollbackDistributedTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transactionScope == null)
        {
            throw new InvalidOperationException("No transaction in progress.");
        }

        // TransactionScope doesn't have RollbackAsync - just dispose it
        // Not calling Complete() before disposing automatically rolls back
        _transactionScope.Dispose();
        _transactionScope = null;

        return Task.CompletedTask;
    }

    // Guru context-specific transaction
    public async Task BeginGuruTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_guruTransaction != null)
        {
            throw new InvalidOperationException("Guru transaction is already in progress.");
        }

        _guruTransaction = await _guruDbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitGuruTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_guruTransaction == null)
        {
            throw new InvalidOperationException("No Guru transaction in progress.");
        }

        try
        {
            await _guruDbContext.SaveChangesAsync(cancellationToken);
            await _guruTransaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await _guruTransaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _guruTransaction.DisposeAsync();
            _guruTransaction = null;
        }
    }

    public async Task RollbackGuruTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_guruTransaction == null)
        {
            return;
        }

        try
        {
            await _guruTransaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _guruTransaction.DisposeAsync();
            _guruTransaction = null;
        }
    }

    // Identity (User Management) context-specific transaction
    public async Task BeginUserManagementTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_userManagementTransaction != null)
        {
            throw new InvalidOperationException("Identity transaction is already in progress.");
        }
        _userManagementTransaction = await _userManagementDbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitUserManagementTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_userManagementTransaction == null)
        {
            throw new InvalidOperationException("No Identity transaction in progress.");
        }

        try
        {
            await _userManagementDbContext.SaveChangesAsync(cancellationToken);
            await _userManagementTransaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await _userManagementTransaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _userManagementTransaction.DisposeAsync();
            _userManagementTransaction = null;
        }
    }

    public async Task RollbackUserManagementTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_userManagementTransaction == null)
        {
            return;
        }

        try
        {
            await _userManagementTransaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _userManagementTransaction.DisposeAsync();
            _userManagementTransaction = null;
        }
    }

    public async Task<int> SaveGuruChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _guruDbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException("Concurrency conflict occurred.", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("Database update failed.", ex);
        }
    }

    public async Task<int> SaveUserManagementChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _userManagementDbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException("Concurrency conflict occurred.", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("Database update failed.", ex);
        }
    }

    public void Dispose()
    {
        _transactionScope?.Dispose();

        _guruTransaction?.Dispose();
        _userManagementTransaction?.Dispose();

        _guruDbContext?.Dispose();
        _userManagementDbContext?.Dispose();

        GC.SuppressFinalize(this);
    }
}