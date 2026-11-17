using System.Text;
using BankApp.DataAccessLayer.DTOs;

// ReportWriter - Writes one .txt report per country
public class ReportWriter
{
    // Folder - Where the report files are saved (set once in the constructor)
    private readonly string _folder;

    public ReportWriter(string folder)
    {
        _folder = folder;
    }

    // Write - Builds the report text, saves it as a file and returns the full path
    public string Write(string country, List<SuspiciousTransactionDto> suspicious, int fromId, int toId)
    {
        // File name 
        var fileName = $"suspicious-{country}-{DateTime.Now:yyyy-MM-dd_HHmm}.txt";
        var path = Path.Combine(_folder, fileName);

        // StringBuilder - Faster than += when building many lines
        var sb = new StringBuilder();

        // Header 
        sb.AppendLine($"Suspicious transactions - {country}");
        sb.AppendLine($"Run: {DateTime.Now:yyyy-MM-dd HH:mm}");
        sb.AppendLine($"Checked transaction ids: {fromId + 1} - {toId}");
        sb.AppendLine($"Total suspicious: {suspicious.Count}");
        sb.AppendLine();

        // Grouping 
        foreach (var customer in suspicious
            .GroupBy(s => new { s.CustomerId, s.CustomerName })
            .OrderBy(g => g.Key.CustomerId))
        {
            sb.AppendLine($"Customer {customer.Key.CustomerId} {customer.Key.CustomerName}");

            foreach (var account in customer.GroupBy(s => s.AccountId))
            {
                sb.AppendLine($"  Account {account.Key}");

                // Transaction 
                foreach (var t in account.OrderBy(x => x.TransactionId))
                    sb.AppendLine($"    Transaction {t.TransactionId} | {t.Date:yyyy-MM-dd} | {t.Amount:N2} | {t.Reason}");
            }

            sb.AppendLine();
        }

        // Save 
        File.WriteAllText(path, sb.ToString());
        return path;
    }
}