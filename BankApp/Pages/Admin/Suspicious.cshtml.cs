using AutoMapper;
using BankApp.Services.Interfaces;
using BankApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages.Admin
{
    // PageModel - Suspicious transactions, Admin only
    [Authorize(Roles = "Admin")]
    public class SuspiciousTransactionsModel : PageModel
    {
        private const int PageSize = 50;
        private static readonly int[] DaysOptions = { 0, 7, 30, 90 };

        private readonly ISuspiciousTransactionService _service;
        private readonly IMapper _mapper;

        public SuspiciousTransactionsModel(ISuspiciousTransactionService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public List<string> Countries { get; set; } = new();
        public List<SuspiciousTransactionViewModel> PageItems { get; set; } = new();

        // Filters - bound from the URL (GET) and validated below
        [BindProperty(SupportsGet = true)]
        public string? Country { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Days { get; set; } = 30;

        // Summary for the cards
        public int TotalCount { get; set; }
        public decimal TotalAmount { get; set; }
        public int SingleCount { get; set; }
        public int WindowCount { get; set; }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public async Task OnGetAsync(int pageNo)
        {
            Countries = (await _service.GetCountriesAsync()).OrderBy(c => c).ToList();

            if (pageNo < 1) pageNo = 1;
            CurrentPage = pageNo;

            // First visit - pick the first country
            if (string.IsNullOrEmpty(Country))
                Country = Countries.FirstOrDefault();

            // Server-side validation of the filters
            if (Country == null || !Countries.Contains(Country))
                ModelState.AddModelError(nameof(Country), "Choose a country from the list.");

            if (!DaysOptions.Contains(Days))
                ModelState.AddModelError(nameof(Days), "Choose All time, 7, 30 or 90 days.");

            if (!ModelState.IsValid)
                return;

            // null = all time, otherwise how far back to show
            DateOnly? since = Days == 0 ? null : DateOnly.FromDateTime(DateTime.Now.AddDays(-Days));
            var found = await _service.FindSuspiciousAsync(Country!, 0, since);

            var all = _mapper.Map<List<SuspiciousTransactionViewModel>>(found)
                .OrderByDescending(t => t.TransactionId)
                .ToList();

            TotalCount = all.Count;
            TotalAmount = all.Sum(t => Math.Abs(t.Amount));
            SingleCount = all.Count(t => t.Reason.Contains("Single"));
            WindowCount = all.Count(t => t.Reason.Contains("Total"));

            TotalPages = Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
            if (CurrentPage > TotalPages) CurrentPage = TotalPages;

            PageItems = all.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
        }
    }
}