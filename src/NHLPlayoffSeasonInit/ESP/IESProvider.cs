using System.Threading.Tasks;

namespace NHLPlayoffSeasonInit.ESP
{
    public interface IESProvider
    {
        Task<Stream?> GetPlayerHeadShotAsync(string teamId, int playerNumber, CancellationToken cancellationToken);
        Task<string?> GetPlayerIdAsync(string teamId, int playerNumber, CancellationToken cancellationToken);
        Task<string?> GetTeamIdAsync(string commanName, CancellationToken cancellationToken);
    }
}