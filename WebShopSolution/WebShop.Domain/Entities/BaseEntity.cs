namespace WebShop.Domain.Entities;

public abstract class BaseEntity<TId>
{
    public TId Id { get; set; } = default!;
}