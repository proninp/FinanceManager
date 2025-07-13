﻿using FinanceManager.CatalogService.Contracts.Common;
using FinanceManager.CatalogService.Contracts.DTOs.Abstractions;

namespace FinanceManager.CatalogService.Contracts.DTOs.Currencies;

/// <summary>
/// DTO для фильтрации и пагинации валют
/// </summary>
/// <param name="ItemsPerPage">Количество элементов на странице</param>
/// <param name="Page">Номер страницы</param>
/// <param name="NameContains">Содержит название валюты</param>
/// <param name="CharCode">Символьный код валюты</param>
/// <param name="NumCode">Числовой код валюты</param>
public record CurrencyFilterDto(
    int ItemsPerPage,
    int Page,
    string? NameContains = null,
    string? CharCode = null,
    string? NumCode = null
) : BasePaginationDto(ItemsPerPage, Page);