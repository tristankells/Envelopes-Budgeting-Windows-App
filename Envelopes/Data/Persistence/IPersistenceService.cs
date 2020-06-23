using System.Threading.Tasks;

namespace Envelopes.Data.Persistence {
    public interface IPersistenceService {

        Task SaveApplicationData(ApplicationData data, string fileName);
        Task SaveApplicationData(ApplicationData data);

        Task<ApplicationData> GetApplicationData(string fileName);
        Task<ApplicationData> GetApplicationData();
    }
}