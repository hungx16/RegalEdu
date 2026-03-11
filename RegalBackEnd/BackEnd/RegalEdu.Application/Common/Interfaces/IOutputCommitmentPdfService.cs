using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Common.Interfaces
{
    public interface IOutputCommitmentPdfService
    {
        Task<byte[]> GeneratePdfAsync(OutputCommitmentModel model, CancellationToken ct = default);
    }
}