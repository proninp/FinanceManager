namespace FinanceManager.TransactionsService.Domain.Enums;


/// <summary>
/// Определяет роли пользователей в системе финансового менеджера
/// </summary>
public enum Role
{
    /// <summary>
    /// Обычный пользователь с базовыми правами доступа
    /// </summary>
    User,
    
    /// <summary>
    /// Администратор системы с расширенными правами управления
    /// </summary>
    Administrator
}