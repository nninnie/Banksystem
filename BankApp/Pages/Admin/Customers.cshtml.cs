using AutoMapper;
using BankApp.Services.Interfaces;
using BankApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BankApp.Pages.Admin
{
    // PageModel – customer list: search, sorting, pagination
    [Authorize(Roles = "Admin,Cashier")]
    public class CustomersModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;

        public CustomersModel(ICustomerService customerService, IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
        }

        public List<CustomerViewModel> Customers { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        [StringLength(50, ErrorMessage = "Name or ID can be max 50 characters.")]
        public string? SearchName { get; set; }

        [BindProperty(SupportsGet = true)]
        [StringLength(50, ErrorMessage = "City can be max 50 characters.")]
        public string? SearchCity { get; set; }

        public string? SortColumn { get; set; }
        public string? SortOrder { get; set; }
        public int CurrentPage { get; set; }

        public bool HasNextPage { get; set; }

        // Loads one page (50 customers) for the search, sort and paging on the Customers page
        public async Task OnGetAsync(string? sortColumn, string? sortOrder, int pageNo)
        {
            if (pageNo < 1) pageNo = 1;

            SortColumn = sortColumn;
            SortOrder = sortOrder;
            CurrentPage = pageNo;

            if (!ModelState.IsValid)
                return;

            var customers = await _customerService.SearchCustomersAsync(
                SearchName, SearchCity, sortColumn, sortOrder, pageNo);

            HasNextPage = customers.Count > 50;

            Customers = _mapper.Map<List<CustomerViewModel>>(customers.Take(50));
        }
    }
}