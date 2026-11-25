using BankApp.DataAccessLayer.DTOs;
using System.Text;

// ReportWriter - Writes one .txt report per country
public class ReportWriter
{
    private readonly string _folder;

    public ReportWriter(string folder)
    {
        _folder = folder;
    }

    // Write - Builds the report text, saves it as a file and returns the full path
    public string Write(string country, List<SuspiciousTransactionDto> suspicious, int fromId, int toId)
    {
        var fileName = $"suspicious-{country}-{DateTime.Now:yyyy-MM-dd_HHmm}.txt";
        var path = Path.Combine(_folder, fileName);
 
        var sb = new StringBuilder();

        sb.AppendLine($"Suspicious transactions - {country}");
        sb.AppendLine($"Run: {DateTime.Now:yyyy-MM-dd HH:mm}");
        sb.AppendLine($"Checked transaction ids: {fromId + 1} - {toId}");
        sb.AppendLine($"Total suspicious: {suspicious.Count}");
        sb.AppendLine();

        foreach (var customer in suspicious
            .GroupBy(s => new { s.CustomerId, s.CustomerName })
            .OrderBy(g => g.Key.CustomerId))
        {
            sb.AppendLine($"Customer {customer.Key.CustomerId} {customer.Key.CustomerName}");

            foreach (var account in customer.GroupBy(s => s.AccountId))
            {
                sb.AppendLine($"  Account {account.Key}");

                foreach (var t in account.OrderBy(x => x.TransactionId))
                    sb.AppendLine($"    Transaction {t.TransactionId} | {t.Date:yyyy-MM-dd} | {t.Amount:N2} | {t.Reason}");
            }

            sb.AppendLine();
        }

        File.WriteAllText(path, sb.ToString());
        return path;
    }
}