

using System.Text.Json;
using TextShare.Business.Interfaces;
using TextShare.Domain.Entities.Complaints;

namespace TextShare.UI.Data
{
    /// <summary>
    /// Класс инициализации причин жалоб в базе данных.
    /// </summary>
    public class ComplaintReasonsDbInit : DbInitDataAbstract
    {
        private readonly IComplaintReasonService complaintReasonService;
        public bool InstallComplaintReasons { get; set; } = true;
        public ComplaintReasonsDbInit(WebApplication webApp) : base(webApp)
        {
            complaintReasonService = this.scope.ServiceProvider.GetRequiredService<IComplaintReasonService>();
        }

        public async override Task<bool> SeedData()
        {
            if (!InstallComplaintReasons) return false;
            int categoryComplaintReasons = await installComplaintReasons();

            return categoryComplaintReasons > 0;
        }

        public async Task<int> installComplaintReasons() 
        {
            int counter = 0;
            List<ComplaintReasons> reasons = await createComplaintReasons();
            foreach(ComplaintReasons reason in reasons)
            {
                if(!(await complaintReasonService.ContainsComplaintReasonAsync(reason)))
                {
                    await complaintReasonService.CreateComplaintReasonAsync(reason);
                    await complaintReasonService.SaveAsync();
                    counter ++;
                }
            }
            return counter;
        }

        private async Task<List<ComplaintReasons>> createComplaintReasons()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "complaintReasons.json");
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Файл {filePath} не найден.");
            }
            string json = await File.ReadAllTextAsync(filePath);

            var reasons = JsonSerializer.Deserialize<List<ComplaintReasons>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return reasons ?? new List<ComplaintReasons>();
        }
    }
}
