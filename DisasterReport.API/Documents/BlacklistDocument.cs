using DisasterReport.Services.Models.BlacklistEntryDTO; // Make sure this matches your project's namespace
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class BlacklistDocument : IDocument
{
    private readonly List<BlacklistExportDto> _blacklistEntries;

    public BlacklistDocument(List<BlacklistExportDto> blacklistEntries)
    {
        _blacklistEntries = blacklistEntries;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                // Configure page settings for a wide data table
                page.Margin(5);
                page.Size(PageSizes.A4.Width, PageSizes.A4.Height); // Corrected the usage of PageSizes
                page.DefaultTextStyle(x => x.FontSize(8)); // Use a slightly smaller font

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
    }

    void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().AlignCenter().Text("Blacklist History")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                column.Item().Text(text =>
                {
                    text.Span("Generated on: ").SemiBold();
                    text.Span($"{System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                });
            });
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(20).Column(column =>
        {
            column.Item().Table(table =>
            {
                // Define column widths: one fixed-width column and eight relative-width columns
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(30);     // No.
                    columns.RelativeColumn(7);      // Name
                    columns.RelativeColumn(8);      // Email
                    columns.RelativeColumn(7);      // Reason
                    columns.RelativeColumn(7);      // Blocked By
                    columns.RelativeColumn(6);      // Blocked At
                    columns.RelativeColumn(8);      // Unblocked Reason
                    columns.RelativeColumn(7);      // Unblocked By
                    columns.RelativeColumn(6);      // Unblocked At
                });

                // Define the table header
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("No");
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Name");
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Email");
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Reason");
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Blocked By");
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Blocked At");
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Unblocked Reason");
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Unblocked By");
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Unblocked At");
                });

                for (int i = 0; i < _blacklistEntries.Count; i++)
                {
                    var item = _blacklistEntries[i];
                    var rowNumber = i + 1;

                    // Set alternating background color for readability ("zebra striping")
                    var backgroundColor = i % 2 == 0 ? Colors.Grey.Lighten5 : Colors.White;

                    table.Cell().Background(backgroundColor).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(rowNumber.ToString());
                    table.Cell().Background(backgroundColor).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.Name);
                    table.Cell().Background(backgroundColor).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.Email);
                    table.Cell().Background(backgroundColor).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.Reason);
                    table.Cell().Background(backgroundColor).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.CreatedAdminName);
                    table.Cell().Background(backgroundColor).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text($"{item.CreatedAt:yyyy-MM-dd HH:mm}");

                    // Safely handle potential null values to prevent crashes
                    table.Cell().Background(backgroundColor).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.UpdatedReason ?? "");
                    table.Cell().Background(backgroundColor).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.UpdatedAdminName ?? "");
                    table.Cell().Background(backgroundColor).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.UpdatedAt?.ToString("yyyy-MM-dd HH:mm") ?? "");
                }
            });
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(x =>
        {
            x.Span("Page ");
            x.CurrentPageNumber();
        });
    }
}