using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Abstractions.Services;
using FinanceManager.CatalogService.Contracts.DTOs.Banks;
using FinanceManager.CatalogService.Domain.Entities;
using FinanceManager.CatalogService.Implementations.Errors.Abstractions;
using FluentResults;
using Serilog;

namespace FinanceManager.CatalogService.Implementations.Services;

/// <summary>
/// Сервис для управления банками, реализующий основные CRUD-операции и дополнительные бизнес-функции.
/// </summary>
public class BankService(
    IUnitOfWork unitOfWork,
    IBankRepository bankRepository,
    ICountryRepository countryRepository,
    IBankErrorsFactory bankErrorsFactory,
    ICountryErrorsFactory countryErrorsFactory,
    ILogger logger)
    : IBankService
{
    /// <summary>
    /// Получает банк по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор банка.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат с DTO банка или ошибкой, если не найден.</returns>
    public async Task<Result<BankDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Получение банка по идентификатору: {BankId}", id);

        var bank =
            await bankRepository.GetByIdAsync(id, disableTracking: true, cancellationToken: cancellationToken);
        if (bank is null)
        {
            logger.Warning("Банк с идентификатором {BankId} не найден", id);
            return Result.Fail(bankErrorsFactory.NotFound(id));
        }

        logger.Information("Банк {BankId} успешно получен", id);
        return Result.Ok(bank.ToDto());
    }

    /// <summary>
    /// Получает список банков с пагинацией и фильтрацией.
    /// </summary>
    /// <param name="filter">Параметры фильтрации и пагинации.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат с коллекцией DTO банков.</returns>
    public async Task<Result<ICollection<BankDto>>> GetPagedAsync(BankFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Получение списка банков с фильтрацией: {@Filter}", filter);
        var banks =
            await bankRepository.GetPagedAsync(filter, cancellationToken: cancellationToken);

        var banksDto = banks.ToDto();

        logger.Information("Получено {Count} банков", banksDto.Count);
        return Result.Ok(banksDto);
    }

    /// <summary>
    /// Получает все банки.
    /// </summary>
    /// <param name="includeRelated">Включать связанные сущности.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат с коллекцией DTO банков.</returns>
    public async Task<Result<ICollection<BankDto>>> GetAllAsync(bool includeRelated = true,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Получение всех банков. Включить связанные данные: {IncludeRelated}", includeRelated);
        var banks =
            await bankRepository.GetAllAsync(cancellationToken: cancellationToken);

        var banksDto = banks.ToDto();

        logger.Information("Получено {Count} банков", banksDto.Count);
        return Result.Ok(banksDto);
    }

    /// <summary>
    /// Создаёт новый банк.
    /// </summary>
    /// <param name="createDto">Данные для создания банка.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат с DTO созданного банка или ошибкой.</returns>
    public async Task<Result<BankDto>> CreateAsync(CreateBankDto createDto,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Создание нового банка: {@CreateDto}", createDto);

        if (string.IsNullOrWhiteSpace(createDto.Name))
        {
            logger.Warning("Попытка создания банка без указания названия");
            return Result.Fail(bankErrorsFactory.NameIsRequired());
        }

        var country = await countryRepository.GetByIdAsync(createDto.CountryId, disableTracking: true,
            cancellationToken: cancellationToken);

        if (country is null)
        {
            logger.Warning("Страна с идентификатором {CountryId} не найдена для создания банка", createDto.CountryId);
            return Result.Fail(countryErrorsFactory.NotFound(createDto.CountryId));
        }

        if (!await CheckBankNameUniq(createDto.Name, createDto.CountryId, cancellationToken: cancellationToken))
        {
            logger.Warning(
                "Попытка создания банка с неуникальным названием '{BankName}' в стране {CountryId} ({CountryName})",
                createDto.Name, country.Id, country.Name);
            return Result.Fail(bankErrorsFactory.NameAlreadyExists(createDto.Name, country.Id, country.Name));
        }

        var bank = await bankRepository.AddAsync(createDto.ToBank(), cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.Information("Successfully created bank: {BankId} with name: {Name}",
            bank.Id, createDto.Name);

        logger.Information("Банк {BankId} с названием '{BankName}' успешно создан в стране {CountryName}",
            bank.Id, createDto.Name, country.Name);

        return Result.Ok(bank.ToDto());
    }

    /// <summary>
    /// Обновляет данные существующего банка.
    /// </summary>
    /// <param name="updateDto">Данные для обновления банка.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат с DTO обновленного банка или ошибкой.</returns>
    public async Task<Result<BankDto>> UpdateAsync(UpdateBankDto updateDto,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Обновление банка: {@UpdateDto}", updateDto);

        var bank =
            await bankRepository.GetByIdAsync(updateDto.Id, cancellationToken: cancellationToken);
        if (bank is null)
        {
            logger.Warning("Банк с идентификатором {BankId} не найден для обновления", updateDto.Id);
            return Result.Fail(bankErrorsFactory.NotFound(updateDto.Id));
        }

        var isNeedUpdate = false;

        Country? country = null;
        if (updateDto.CountryId is not null && updateDto.CountryId.Value != bank.CountryId)
        {
            country = await countryRepository.GetByIdAsync(updateDto.CountryId.Value, disableTracking: true,
                cancellationToken: cancellationToken);

            if (country is null)
            {
                logger.Warning("Страна с идентификатором {CountryId} не найдена для обновления банка {BankId}",
                    updateDto.CountryId.Value, updateDto.Id);
                return Result.Fail(countryErrorsFactory.NotFound(updateDto.CountryId.Value));
            }

            bank.CountryId = updateDto.CountryId.Value;
            isNeedUpdate = true;
        }

        country ??= bank.Country;

        if (updateDto.Name is not null && !string.Equals(updateDto.Name, bank.Name))
        {
            var isNameUnique =
                await bankRepository.IsNameUniqueByCountryAsync(
                    updateDto.Name, country.Id, cancellationToken: cancellationToken);
            if (!isNameUnique)
            {
                logger.Warning(
                    "Попытка обновления банка {BankId} с неуникальным названием '{BankName}' в стране {CountryName}",
                    updateDto.Id, updateDto.Name, country.Name);
                return Result.Fail(bankErrorsFactory.NameAlreadyExists(updateDto.Name, country.Id, country.Name));
            }

            bank.Name = updateDto.Name;
            isNeedUpdate = true;
        }

        if (isNeedUpdate)
        {
            // нам не нужно вызывать метод bankRepository.UpdateAsync(), так как сущность bank уже отслеживается
            await unitOfWork.CommitAsync(cancellationToken);
            logger.Information("Банк {BankId} успешно обновлен", updateDto.Id);
        }
        else
        {
            logger.Information("Изменения для банка {BankId} не обнаружены", updateDto.Id);
        }

        return Result.Ok(bank.ToDto());
    }

    /// <summary>
    /// Удаляет банк по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор банка.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат выполнения операции.</returns>
    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Удаление банка: {BankId}", id);

        if (!await bankRepository.CanBeDeletedAsync(id, cancellationToken))
        {
            logger.Warning("Банк {BankId} не может быть удален, так как используется в счетах", id);
            return Result.Fail(bankErrorsFactory.CannotDeleteUsedBank(id));
        }

        await bankRepository.DeleteAsync(id, cancellationToken);
        var affectedRows = await unitOfWork.CommitAsync(cancellationToken);
        if (affectedRows > 0)
        {
            logger.Information("Банк {BankId} успешно удален", id);
        }

        return Result.Ok();
    }

    /// <summary>
    /// Получает количество счетов, связанных с банком.
    /// </summary>
    /// <param name="bankId">Идентификатор банка.</param>
    /// <param name="includeArchivedAccounts">Включать архивные счета.</param>
    /// <param name="includeDeletedAccounts">Включать удалённые счета.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат с количеством счетов.</returns>
    public async Task<Result<int>> GetAccountsCountAsync(Guid bankId, bool includeArchivedAccounts = false,
        bool includeDeletedAccounts = false,
        CancellationToken cancellationToken = default)
    {
        logger.Information(
            "Получение количества счетов для банка {BankId}. Включить архивные: {IncludeArchived}, включить удаленные: {IncludeDeleted}",
            bankId, includeArchivedAccounts, includeDeletedAccounts);

        var bank = await bankRepository.GetByIdAsync(bankId, disableTracking: true,
            cancellationToken: cancellationToken);
        if (bank is null)
        {
            logger.Warning("Банк с идентификатором {BankId} не найден для подсчета счетов", bankId);
            return Result.Fail(bankErrorsFactory.NotFound(bankId));
        }

        var count = await bankRepository.GetAccountsCountAsync(
            bankId,
            includeArchivedAccounts,
            includeDeletedAccounts,
            cancellationToken);

        logger.Information("Для банка {BankId} найдено {Count} счетов", bankId, count);
        return Result.Ok(count);
    }

    /// <summary>
    /// Проверяет уникальность названия банка в рамках страны.
    /// </summary>
    /// <param name="name">Название банка.</param>
    /// <param name="countryId">Идентификатор страны.</param>
    /// <param name="excludeId">Идентификатор банка, который нужно исключить из проверки (опционально).</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если название уникально; иначе false.</returns>
    private async Task<bool> CheckBankNameUniq(string name, Guid countryId, Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        logger.Debug(
            "Проверка уникальности названия банка '{BankName}' в стране {CountryId}, исключая банк {ExcludeId}",
            name, countryId, excludeId);

        var isNameUnique =
            await bankRepository.IsNameUniqueByCountryAsync(name, countryId,
                cancellationToken: cancellationToken);
        
        logger.Debug("Название банка '{BankName}' в стране {CountryId} {UniqueResult}",
            name, countryId, isNameUnique ? "уникально" : "не уникально");
        
        return isNameUnique;
    }
}