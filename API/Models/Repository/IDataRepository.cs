namespace API.Models.Repository;

public interface IDataRepository<TEntity, in TIdentifier> : IReadableRepository<TEntity, TIdentifier>, IWritableRepository<TEntity>;