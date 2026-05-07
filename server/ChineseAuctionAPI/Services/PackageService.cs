using ChineseAuctionAPI.DTOs;
using ChineseAuctionAPI.Models;
using ChineseAuctionAPI.Repositories;

namespace ChineseAuctionAPI.Services
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepo _repository;
        private readonly ILogger<PackageService> _logger;

        public PackageService(IPackageRepo repository, ILogger<PackageService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<PackageDTO>> GetAllPackagesAsync()
        {
            try
            {
                _logger.LogInformation("שולף את כל החבילות.");
                var packages = await _repository.GetAllAsync();
                return packages.Select(p => new PackageDTO
                {
                    IdPackage = p.IdPackage,
                    Name = p.Name,
                    Price = p.Price,
                    AmountRegular = p.AmountRegular,
                    AmountPremium = p.AmountPremium,
                    Description = p.Description
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה בשליפת חבילות.");
                throw;
            }
        }

        public async Task<PackageDTO?> GetPackageByIdAsync(int id)
        {
            var p = await _repository.GetByIdAsync(id);
            if (p == null) return null;

            return new PackageDTO
            {
                IdPackage = p.IdPackage,
                Name = p.Name,
                Price = p.Price,
                AmountRegular = p.AmountRegular,
                AmountPremium = p.AmountPremium,
                Description = p.Description
            };
        }

        public async Task<int> CreatePackageAsync(PackageCreateDTO dto)
        {
            var package = new Package
            {
                Name = dto.Name,
                Price = dto.Price,
                AmountRegular = dto.AmountRegular,
                AmountPremium = dto.AmountPremium,
                Description = dto.Description
            };
            return await _repository.AddAsync(package);
        }

        public async Task UpdatePackageAsync(int id, PackageCreateDTO dto)
        {
            var package = new Package
            {
                IdPackage = id,
                Name = dto.Name,
                Price = dto.Price,
                AmountRegular = dto.AmountRegular,
                AmountPremium = dto.AmountPremium,
                Description = dto.Description
            };
            await _repository.UpdateAsync(package);
        }

        public async Task DeletePackageAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}