namespace FinanceManager.CatalogService.Domain.Abstractions;

/// <summary>
/// Базовый класс для сущностей с поддержкой мягкого удаления
/// </summary>
public abstract class SoftDeletableEntity : IdentityModel
{
    /// <summary>
    /// Флаг мягкого удаления сущности
    /// </summary>
    public bool IsDeleted { get; private set; }

    /// <summary>
    /// Помечает сущность, как удаленную
    /// </summary>
    public virtual void MarkAsDeleted() => IsDeleted = true;

    /// <summary>
    /// Восстанавливает удаленную сущность
    /// </summary>
    public virtual void Restore() => IsDeleted = false;
}