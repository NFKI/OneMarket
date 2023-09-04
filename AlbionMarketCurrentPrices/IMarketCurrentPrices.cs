
using System.Threading.Tasks;

namespace AlbionMarketCurrentPrices
{
    public interface IMarketCurrentPrices
    {
        void ParseMarketCurrentPrices(string ResponseBody);
        void ExtractRepeatedJsonAnswerForCurrentPrices(string RepeatedHttpRequestId, string CurrentPricesJsonLogsPath);
    }
}
