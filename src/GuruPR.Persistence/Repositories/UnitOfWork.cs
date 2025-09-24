using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using GuruPR.Persistence.Contexts;
using GuruPR.Domain.Entities.OAuth;
using GuruPR.Application.Interfaces.Persistence;

namespace GuruPR.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly GuruDBContext _guruDbContext;
    private IDbContextTransaction? _transaction;

    private IProviderRepository? _providerRepository;

    public UnitOfWork(GuruDBContext guruDbContext)
    {
        _guruDbContext = guruDbContext;
    }

    public IProviderRepository Providers => _providerRepository ??= new ProviderRepository(_guruDbContext);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _transaction = await _guruDbContext.Database.BeginTransactionAsync(cancellationToken);
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

    public void Dispose()
    {
        _guruDbContext?.Dispose();
        _transaction?.Dispose();
    }

    public async Task<bool> TestAsync(CancellationToken cancellationToken = default)
    {
        await Providers.AddAsync(new Provider()
        {
            ClientId = "t",
            AuthorizationUrl = "https://test.com",
            ClientSecret = "s",
            Name = "Test",
            CreatedAt = DateTime.UtcNow,
            DefaultScopes = ["scope"],
            Id = 1,
            TokenUrl = "https://token.com"
        });

        await _guruDbContext.SaveChangesAsync();
        return await _guruDbContext.Database.CanConnectAsync();
    }
}
