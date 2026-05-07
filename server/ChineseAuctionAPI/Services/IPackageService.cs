using ChineseAuctionAPI.DTOs;

namespace ChineseAuctionAPI.Services
{
    public interface IPackageService
    {
        Task<IEnumerable<PackageDTO>> GetAllPackagesAsync();
        Task<PackageDTO?> GetPackageByIdAsync(int id);
        Task<int> CreatePackageAsync(PackageCreateDTO dto);
        Task UpdatePackageAsync(int id, PackageCreateDTO dto);
        Task DeletePackageAsync(int id);
    }
}