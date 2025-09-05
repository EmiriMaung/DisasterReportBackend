using DisasterReport.Data.Domain;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services
{
    public class CertificateService : ICertificateService
    {
        private readonly ApplicationDBContext _context;

        public CertificateService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<byte[]> GenerateCertificatePdfAsync(int donationId)
        {
            var donation = await _context.Donations
                .Include(d => d.DonateRequest)
                    .ThenInclude(r => r.RequestedByUser)
                .Include(d => d.DonateRequest)
                    .ThenInclude(r => r.Organization)
                .FirstOrDefaultAsync(d => d.Id == donationId);

            if (donation == null)
                throw new Exception("Donation not found");

            var donorName = donation.DonateRequest?.RequestedByUser?.Name ?? "Donor";
            var organizationName = donation.DonateRequest?.Organization?.Name ?? "CivicResponders"; // default platform
            var supportType = donation.DonateRequest?.SupportType;
            var amount = donation.DonateRequest?.Amount;
            var date = donation.DonatedAt;

            byte[] pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(50);
                    page.Background(Colors.Grey.Lighten3);

                    page.Content().Column(col =>
                    {
                        col.Spacing(30);

                        // Platform name / logo
                        col.Item().AlignCenter().Text("CivicResponders")
                            .FontSize(24)
                            .SemiBold()
                            .FontColor(Colors.Blue.Medium);

                        // Certificate title
                        col.Item().AlignCenter().Text("Certificate of Appreciation")
                            .FontSize(32)
                            .Bold()
                            .FontColor(Colors.Black);

                        // Donor name
                        col.Item().AlignCenter().Text(donorName)
                            .FontSize(24)
                            .Bold()
                            .FontColor(Colors.Black);

                        // Donation details
                        col.Item().AlignCenter().Text($"has generously contributed to {organizationName}")
                            .FontSize(16);

                        col.Item().AlignCenter().Text($"Support Type: {supportType ?? "General"}")
                            .FontSize(14);

                        col.Item().AlignCenter().Text($"Amount: {amount?.ToString("N2") ?? "-"} MMK")
                            .FontSize(14);

                        col.Item().AlignCenter().Text($"Date: {date:MMMM dd, yyyy}")
                            .FontSize(14);

                        // Footer / appreciation
                        col.Item().AlignCenter().PaddingTop(50)
                            .Text("We deeply appreciate your contribution!")
                            .FontSize(16)
                            .Italic();

                        // Optional signature lines
                        col.Item().AlignCenter().PaddingTop(30).Row(row =>
                        {
                            row.RelativeColumn().LineHorizontal(1).LineColor(Colors.Black);
                            row.RelativeColumn().LineHorizontal(1).LineColor(Colors.Black);
                        });
                    });
                });
            }).GeneratePdf();

            return pdfBytes;
        }
    }
}
