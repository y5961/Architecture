using ChineseAuctionAPI.DTOs;
using ChineseAuctionAPI.Models;
using ChineseAuctionAPI.Repositories;
using ChineseAuctionAPI.Services.Caching;
using Microsoft.Extensions.Logging;

namespace ChineseAuctionAPI.Services
{
    public class GiftCategoryService : IGiftCategoryService
    {
        private readonly IGiftCategoryRepo _repository;
        private readonly ILogger<GiftCategoryService> _logger;
        private readonly IConfiguration _configuration;
        private readonly ICacheService _cacheService;

        // Cache key constants
        private const string CACHE_KEY_PREFIX = "category:";
        private const string CACHE_KEY_ALL = "category:all";

        public GiftCategoryService(IGiftCategoryRepo repository, ILogger<GiftCategoryService> logger, IConfiguration configuration, ICacheService cacheService)
        {
            _repository = repository;
            _logger = logger;
            _configuration = configuration;
            _cacheService = cacheService;
        }

        public async Task<List<GiftCategoryDTO>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("מתחיל תהליך שליפת כל קטגוריות המתנות - בדיקה בcache תחילה.");
                
                // Try cache first
                var cachedCategories = await _cacheService.GetAsync<List<GiftCategoryDTO>>(CACHE_KEY_ALL);
                if (cachedCategories != null)
                {
                    _logger.LogInformation("רשימת הקטגוריות שלופה מ-Redis cache - {Count} קטגוריות.", cachedCategories.Count);
                    return cachedCategories;
                }

                // If not in cache, get from database
                _logger.LogInformation("רשימת הקטגוריות לא בcache - שולף מהמאגר.");
                var categories = await _repository.GetAllAsync();

                var categoryDtos = categories.Select(c => new GiftCategoryDTO
                {
                    Id = c.IdGiftCategory,
                    Name = c.Name
                }).ToList();

                // Cache the result
                if (categoryDtos.Any())
                {
                    await _cacheService.SetAsync(CACHE_KEY_ALL, categoryDtos);
                    _logger.LogInformation("נשלפו בהצלחה {Count} קטגוריות מהמאגר וצוינו בcache.", categoryDtos.Count);
                }

                return categoryDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "אירעה שגיאה בלתי צפויה בעת ניסיון לשלוף את כל הקטגוריות.");
                throw;
            }
        }

        public async Task<GiftCategoryDTO?> GetByIdAsync(int id)
        {
            try
            {
                var cacheKey = $"{CACHE_KEY_PREFIX}{id}";
                _logger.LogInformation("מנסה לשלוף קטגוריית מתנה עם מזהה: {Id} - בדיקה בcache תחילה.", id);
                
                // Try cache first
                var cachedCategory = await _cacheService.GetAsync<GiftCategoryDTO>(cacheKey);
                if (cachedCategory != null)
                {
                    _logger.LogInformation("קטגוריה מזהה {Id} שלופה מ-Redis cache.", id);
                    return cachedCategory;
                }

                // If not in cache, get from database
                _logger.LogInformation("קטגוריה מזהה {Id} לא בcache - שולף מהמאגר.", id);
                var category = await _repository.GetByIdAsync(id);

                if (category == null)
                {
                    _logger.LogWarning("קטגוריית מתנה עם מזהה {Id} לא נמצאה במערכת.", id);
                    return null;
                }

                var categoryDto = new GiftCategoryDTO
                {
                    Id = category.IdGiftCategory,
                    Name = category.Name
                };

                // Cache the result
                await _cacheService.SetAsync(cacheKey, categoryDto);
                _logger.LogInformation("קטגוריה {Id} שלופה מהמאגר וצוינה בcache.", id);

                return categoryDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה בשירות בעת שליפת קטגוריה עם מזהה {Id}.", id);
                throw;
            }
        }

        public async Task<GiftCategoryDTO> CreateAsync(CreateGiftCategoryDTO dto)
        {
            try
            {
                _logger.LogInformation("מתחיל יצירת קטגוריה חדשה בשם: {Name}", dto.Name);

                var category = new GiftCategory
                {
                    Name = dto.Name
                };

                await _repository.CreateAsync(category);
                _logger.LogInformation("קטגוריה חדשה נוצרה בהצלחה עם מזהה: {Id}", category.IdGiftCategory);

                return new GiftCategoryDTO
                {
                    Id = category.IdGiftCategory,
                    Name = category.Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "נכשלה יצירת קטגוריה חדשה בשם {Name}.", dto.Name);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(int id, UpdateGiftCategoryDTO dto)
        {
            try
            {
                _logger.LogInformation("מעדכן קטגוריה מזהה {Id}.", id);

                var category = await _repository.GetByIdAsync(id);
                if (category == null)
                {
                    _logger.LogWarning("ניסיון לעדכן קטגוריה שלא קיימת במערכת: {Id}.", id);
                    return false;
                }

                category.Name = dto.Name;
                await _repository.UpdateAsync(category);

                // Invalidate cache
                await _cacheService.RemoveAsync($"{CACHE_KEY_PREFIX}{id}");
                await _cacheService.RemoveAsync(CACHE_KEY_ALL);

                _logger.LogInformation("קטגוריה {Id} עודכנה בהצלחה לערך: {Name} וה-cache בוטל.", id, dto.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה בעת עדכון קטגוריה {Id}.", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("מבקש למחוק קטגוריה מזהה {Id}.", id);

                var category = await _repository.GetByIdAsync(id);
                if (category == null)
                {
                    _logger.LogWarning("ניסיון למחוק קטגוריה שלא קיימת: {Id}.", id);
                    return false;
                }

                await _repository.DeleteAsync(category);
                
                // Invalidate cache
                await _cacheService.RemoveAsync($"{CACHE_KEY_PREFIX}{id}");
                await _cacheService.RemoveAsync(CACHE_KEY_ALL);
                
                _logger.LogInformation("קטגוריה {Id} נמחקה מהמערכת בהצלחה וה-cache בוטל.", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה בעת ניסיון למחוק קטגוריה {Id}.", id);
                throw;
            }
        }
    }
}