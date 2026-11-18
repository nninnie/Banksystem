using BankApp.DataAccessLayer.Models;
using BankApp.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

// Colors 
var gold = new Color(0xF2, 0xD6, 0x75);
var green = new Color(0x2F, 0xA5, 0x74);

// TrueColor - Forces exact 24-bit colors
AnsiConsole.Profile.Capabilities.ColorSystem = ColorSystem.TrueColor;

// Config - Reads the connection string from appsettings.json 
var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();

// DbContext - Same BankAppContext as the web app 
var options = new DbContextOptionsBuilder<BankAppContext>()
    .UseSqlServer(config.GetConnectionString("DefaultConnection"),
        sql => sql.CommandTimeout(120))
    .Options;

using var context = new BankAppContext(options);

// Service 
var service = new SuspiciousTransactionService(context);

// State - Remembers where the last run ended
var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BankAppReports");
Directory.CreateDirectory(folder);
var stateFile = Path.Combine(folder, "last-checked-transaction-id.txt");

// First run 
var lastRunId = 0;
if (File.Exists(stateFile))
    lastRunId = int.Parse(File.ReadAllText(stateFile));

// Newest id - Read before searching
var newestId = await service.GetLastTransactionIdAsync();

// Header - Logo
var logo = new Panel(new Rows(
        new FigletText("MyBank").Centered().Color(gold),
        new Markup("[#2FA574]TRUST | SECURITY | GROWTH[/]").Centered()))
    .Border(BoxBorder.Rounded)
    .BorderColor(gold)
    .Padding(4, 1);
logo.Expand = false;
AnsiConsole.Write(logo);
AnsiConsole.MarkupLine("[grey]Suspicious transactions check[/]");
AnsiConsole.MarkupLine($"Checking transactions after id [#F2D675]{lastRunId}[/] up to [#F2D675]{newestId}[/]");
AnsiConsole.WriteLine();

// Tables
var summary = new Table().Border(TableBorder.Rounded).BorderColor(gold);
summary.AddColumn("Country");
summary.AddColumn(new TableColumn("Suspicious").RightAligned());
summary.AddColumn("Report file");

var examples = new Table().Border(TableBorder.Rounded).BorderColor(green);
examples.AddColumn("Country");
examples.AddColumn("Customer");
examples.AddColumn("Account");
examples.AddColumn("Transaction");
examples.AddColumn("Date");
examples.AddColumn(new TableColumn("Amount").RightAligned());
examples.AddColumn("Reason");

var reportWriter = new ReportWriter(folder);
var countries = await service.GetCountriesAsync();

// Loop - Runs the check once per country
foreach (var country in countries)
{
    // Status - Shows a spinner while the database search runs
    var suspicious = await AnsiConsole.Status()
        .StartAsync($"Checking {country}...", ctx => service.FindSuspiciousAsync(country, lastRunId));

    // Report - One .txt file per country 
    var reportFile = Path.GetFileName(reportWriter.Write(country, suspicious, lastRunId, newestId));

    // Summary - One row per country
    summary.AddRow(country, suspicious.Count.ToString("N0"), Markup.Escape(reportFile));

    // Examples - First three per country
    foreach (var s in suspicious.Take(3))
    {
        examples.AddRow(
            country,
            Markup.Escape($"{s.CustomerId} {s.CustomerName}"),
            s.AccountId.ToString(),
            s.TransactionId.ToString(),
            s.Date.ToString("yyyy-MM-dd"),
            s.Amount.ToString("N2"),
            Markup.Escape(s.Reason));
    }
}

// Result - Print the tables
AnsiConsole.Write(summary);
AnsiConsole.MarkupLine($"[grey]Reports saved in:[/] {Markup.Escape(folder)}");
AnsiConsole.MarkupLine("[grey]First three per country:[/]");
AnsiConsole.Write(examples);

// Save - Next run starts after this id
File.WriteAllText(stateFile, newestId.ToString());


Console.WriteLine();
Console.WriteLine("Press any key to exit...");
Console.ReadKey();