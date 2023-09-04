
using System.Threading.Tasks;

namespace AlbionDataProjectHttpRequest
{
    public interface IDataProjectHttpRequest
    {
        Task<string> PerformAlbionDataProjectHttpRequest(string FormedHttpRequestFullUrl, string CurrentPricesLogsPath, string CurrentPricesJsonLogsPath);
        void FormatAlbionDataProjectHttpRequest();
    }
}
