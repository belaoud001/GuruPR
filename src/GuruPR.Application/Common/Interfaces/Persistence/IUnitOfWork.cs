namespace GuruPR.Application.Common.Interfaces.Persistence;

public interface IUnitOfWork : IDisposable
{
    IToolRepository Tools { get; }
    IAgentRepository Agents { get; }
    IMessageRepository Messages { get; }
    IProviderRepository Providers { get; }
    IConversationRepository Conversations { get; }
    IProviderConnectionRepository ProviderConnections { get; }

    IUserRepository Users { get; }

    // Distributed transaction (both contexts)
    Task BeginDistributedTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitDistributedTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackDistributedTransactionAsync(CancellationToken cancellationToken = default);

    // Guru context transaction
    Task BeginGuruTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitGuruTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackGuruTransactionAsync(CancellationToken cancellationToken = default);

    // Identity (User Management) context transaction
    Task BeginUserManagementTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitUserManagementTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackUserManagementTransactionAsync(CancellationToken cancellationToken = default);

    // Save changes
    Task<int> SaveGuruChangesAsync(CancellationToken cancellationToken = default);
    Task<int> SaveUserManagementChangesAsync(CancellationToken cancellationToken = default);
}
