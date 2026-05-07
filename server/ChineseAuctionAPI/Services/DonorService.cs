using System.Drawing;
using ChineseAuctionAPI.DTOs;
using ChineseAuctionAPI.Models;
using ChineseAuctionAPI.Repositories;
using ChineseAuctionAPI.Services.Caching;
using Microsoft.Extensions.Logging;

namespace ChineseAuctionAPI.Services
{
    public class DonorService : IDonorService
    {
        private readonly IDonorRepository _repository;
        private readonly ILogger<DonorService> _logger;
        private readonly IConfiguration _configuration;
        private readonly ICacheService _cacheService;

        // Cache key constants
        private const string CACHE_KEY_PREFIX = "donor:";
        private const string CACHE_KEY_ALL = "donor:all";

        public DonorService(IDonorRepository repository, ILogger<DonorService> logger, IConfiguration configuration, ICacheService cacheService)
        {
            _repository = repository;
            _logger = logger;
            _configuration = configuration;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<DonorDTO>> GetAllDonorsAsync()
        {
            try
            {
                _logger.LogInformation("מתחיל שליפת כל התורמים - בדיקה בcache תחילה.");
                
                // Try cache first
                var cachedDonors = await _cacheService.GetAsync<List<DonorDTO>>(CACHE_KEY_ALL);
                if (cachedDonors != null)
                {
                    _logger.LogInformation("רשימת התורמים שלופה מ-Redis cache - {Count} תורמים.", cachedDonors.Count);
                    return cachedDonors;
                }

                // If not in cache, get from database
                _logger.LogInformation("רשימת התורמים לא בcache - שולף מהמאגר.");
                var donors = await _repository.GetAllAsync();

                var donorDtos = donors.Select(d => new DonorDTO
                {
                    IdDonor = d.IdDonor,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    Email = d.Email,
                    PhoneNumber = d.PhoneNumber
                }).ToList();

                // Cache the result
                if (donorDtos.Any())
                {
                    await _cacheService.SetAsync(CACHE_KEY_ALL, donorDtos);
                    _logger.LogInformation("נשלפו בהצלחה {Count} תורמים מהמאגר וצוינו בcache.", donorDtos.Count);
                }

                return donorDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "אירעה שגיאה בשכבת השירות בעת שליפת כל התורמים.");
                throw;
            }
        }

        public async Task<DonorDTO?> GetDonorByIdAsync(int id)
        {
            try
            {
                var cacheKey = $"{CACHE_KEY_PREFIX}{id}";
                _logger.LogInformation("מנסה לשלוף תורם עם מזהה: {Id} - בדיקה בcache תחילה.", id);
                
                // Try cache first
                var cachedDonor = await _cacheService.GetAsync<DonorDTO>(cacheKey);
                if (cachedDonor != null)
                {
                    _logger.LogInformation("תורם מזהה {Id} שלוף מ-Redis cache.", id);
                    return cachedDonor;
                }

                // If not in cache, get from database
                _logger.LogInformation("תורם מזהה {Id} לא בcache - שולף מהמאגר.", id);
                var d = await _repository.GetByIdAsync(id);

                if (d == null)
                {
                    _logger.LogWarning("תורם עם מזהה {Id} לא נמצא.", id);
                    return null;
                }

                var donorDto = new DonorDTO
                {
                    IdDonor = d.IdDonor,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    Email = d.Email,
                    PhoneNumber = d.PhoneNumber
                };

                // Cache the result
                await _cacheService.SetAsync(cacheKey, donorDto);
                _logger.LogInformation("תורם מזהה {Id} שלוף מהמאגר וצוין בcache.", id);

                return donorDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "אירעה שגיאה בשירות בעת שליפת תורם מזהה {Id}.", id);
                throw;
            }
        }

        public async Task<int> CreateDonorAsync(DonorCreateDTO donorDto)
        {
            try
            {
                _logger.LogInformation("מתחיל יצירת תורם חדש: {FirstName} {LastName}", donorDto.FirstName, donorDto.LastName);

                var donor = new Donor
                {
                    FirstName = donorDto.FirstName,
                    LastName = donorDto.LastName,
                    Email = donorDto.Email,
                    PhoneNumber = donorDto.PhoneNumber
                };

                int newId = await _repository.AddAsync(donor);

                _logger.LogInformation("תורם נוצר בהצלחה עם מזהה {NewId}.", newId);
                return newId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה ביצירת תורם חדש בשירות.");
                throw;
            }
        }

        public async Task UpdateDonorAsync(int id, DonorCreateDTO donorDto)
        {
            try
            {
                _logger.LogInformation("מעדכן נתוני תורם מזהה {Id}.", id);

                var donor = new Donor
                {
                    IdDonor = id, // חשוב לעדכון
                    FirstName = donorDto.FirstName,
                    LastName = donorDto.LastName,
                    Email = donorDto.Email,
                    PhoneNumber = donorDto.PhoneNumber
                };

                await _repository.UpdateAsync(donor);
                
                // Invalidate cache
                await _cacheService.RemoveAsync($"{CACHE_KEY_PREFIX}{id}");
                await _cacheService.RemoveAsync(CACHE_KEY_ALL);
                
                _logger.LogInformation("תורם {Id} עודכן בהצלחה וה-cache בוטל.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה בעת עדכון תורם מזהה {Id}.", id);
                throw;
            }
        }

        public async Task DeleteDonorAsync(int id)
        {
            try
            {
                _logger.LogInformation("מנסה למחוק תורם מזהה {Id}.", id);
                await _repository.DeleteAsync(id);
                
                // Invalidate cache
                await _cacheService.RemoveAsync($"{CACHE_KEY_PREFIX}{id}");
                await _cacheService.RemoveAsync(CACHE_KEY_ALL);
                
                _logger.LogInformation("תורם מזהה {Id} נמחק בהצלחה וה-cache בוטל.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה בעת מחיקת תורם מזהה {Id}.", id);
                throw;
            }
        }

        public async Task<IEnumerable<GiftDTO>> GetGiftsAsync(int donorId)
        {
            try
            {
               var gifts= await _repository.GetGiftsAsync(donorId);
                return gifts.Select(g => new GiftDTO
                {
                    Name = g.Name,
                    Description = g.Description,
                    CategoryId = g.CategoryId,
                    Amount = g.Amount,
                    Image = g.Image,
                    IdDonor = donorId
                });

            }
            catch (Exception ex)
            {
                throw new Exception("שגיאה בשליפת המתנות עבור התורם", ex);
            }
        }

        public async Task<Donor?> SortByGift(string donor)
        {
            try
            {
                return await _repository.SortByGift(donor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה בלתי צפויה סינון מילה {word}.", donor);
                throw;
            }
        }

        public async Task<IEnumerable<Donor?>> SortByEmail(string email)
        {
            try
            {
                return await _repository.SortByEmail(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה בלתי צפויה סינון מילה {email}.", email);
                throw;
            }
        }

        public async Task<IEnumerable<Donor?>> SortByName(string name)
        {
            try
            {
                return await _repository.SortByName(name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה בלתי צפויה סינון מילה {email}.", name);
                throw;
            }
        }
    }
}