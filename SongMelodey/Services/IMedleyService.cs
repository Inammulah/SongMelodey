using SongMelodey.Services;
using SongMelodey.Results;
namespace SongMelodey.Services
{
    public interface IMedleyService
    {
        Task<MedleyResult> GenerateMedleyAsync(int medleyId, CancellationToken ct);
    }


}
