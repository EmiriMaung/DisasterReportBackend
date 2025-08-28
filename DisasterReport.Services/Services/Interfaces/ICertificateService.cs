namespace DisasterReport.Services.Services
{
    public interface ICertificateService
    {
        Task<byte[]> GenerateCertificatePdfAsync(int donationId);
    }
}
