namespace LighthouseSocial.Domain.Common;

public abstract class EntityBase
{
    public Guid Id { get; protected set; }
    //todo@buraksenyurt CreatedAt, ModifiedAt,DeletedAt gibi alanlar eklenebilir.
}
