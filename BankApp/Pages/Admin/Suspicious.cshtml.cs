using AutoMapper;
using BankApp.Services.Interfaces;
using BankApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using BankApp.Helpers;

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
        private readonly IMemoryCache _cache;

        public SuspiciousTransactionsModel(ISuspiciousTransactionService service, IMapper mapper, IMemoryCache cache)
        {
            _service = service;
            _mapper = mapper;
            _cache = cache;
        }

        public List<string> Countries { get; set; } = new();
        public List<SuspiciousTransactionViewModel> PageItems { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Country { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Days { get; set; } = 30;

        // Search only runs when the Admin presses Show
        [BindProperty(SupportsGet = true)]
        public bool Search { get; set; }
        public int TotalCount { get; set; }
        public decimal TotalAmount { get; set; }
        public int SingleCount { get; set; }
        public int WindowCount { get; set; }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        // Shows suspicious transactions for one country and period, searched only on Show and cached for 5 minutes.
        public async Task OnGetAsync(int pageNo)
        {
            Countries = (await _service.GetCountriesAsync()).OrderBy(c => c).ToList();

            if (pageNo < 1) pageNo = 1;
            CurrentPage = pageNo;

            if (string.IsNullOrEmpty(Country))
                Country = Countries.FirstOrDefault();

            if (Country == null || !Countries.Contains(Country))
                ModelState.AddModelError(nameof(Country), "Choose a country from the list.");

            if (!DaysOptions.Contains(Days))
                ModelState.AddModelError(nameof(Days), "Choose All time, 7, 30 or 90 days.");

            if (!ModelState.IsValid || !Search)
                return;

            var key = $"suspicious-{Country}-{Days}";
            if (!_cache.TryGetValue(key, out List<SuspiciousTransactionViewModel>? all) || all == null)
            {
                DateOnly? since = Days == 0 ? null : SwedishTime.Today.AddDays(-Days);
                var found = await _service.FindSuspiciousAsync(Country!, 0, since);

                all = _mapper.Map<List<SuspiciousTransactionViewModel>>(found)
                    .OrderByDescending(t => t.TransactionId)
                    .ToList();

                _cache.Set(key, all, TimeSpan.FromMinutes(5));
            }

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