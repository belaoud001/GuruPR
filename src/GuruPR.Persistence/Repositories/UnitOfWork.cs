using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace GuruPR.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly GuruDBContext _guruDBContext;
    private IDbContextTransaction _transaction;

    private IProviderRepository _providerRepository;

    public UnitOfWork(GuruDBContext guruDBContext)
    {
        _guruDBContext = guruDBContext;
    }

    public IProviderRepository Providers => _providerRepository ??= new ProviderRepository(_guruDBContext);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _transaction = await _guruDBContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction in progress.");
        }

        try
        {
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction in progress.");
        }

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _guruDBContext.SaveChangesAsync(cancellationToken);
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
        _guruDBContext?.Dispose();
    }
}
