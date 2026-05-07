using ChineseAuctionAPI.Data;
using Microsoft.EntityFrameworkCore;


namespace StoreAPI.Data
{
    public class SalesContextFactory
    {
        private const string ConnectionString = "Server=DESKTOP-I18T6QA\\SQLEXPRESS;DataBase=0533155961_SalesAPI;Integrated Security=SSPI;Persist Security Info=False;TrustServerCertificate=True;";

        public static SaleContextDB CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SaleContextDB>();
            optionsBuilder.UseSqlServer(ConnectionString);
            return new SaleContextDB(optionsBuilder.Options);
        }
    }
}
