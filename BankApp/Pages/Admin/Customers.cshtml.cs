using AutoMapper;
using BankApp.Services.Interfaces;
using BankApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages.Admin
{

    // VeiwModel - for the customer list: handles searching, pagination, and displaying customers in the admin view
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

        public List<CustomerViewModel> Customers { get; set; }
        public string? SearchName { get; set; }
        public string? SearchCity { get; set; }
        public string? SortColumn { get; set; }
        public string? SortOrder { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }

        public async Task OnGetAsync(string? searchName, string? searchCity, string sortColumn, string sortOrder, int pageNo)
        {
            if (pageNo == 0) pageNo = 1;

            SearchName = searchName;
            SearchCity = searchCity;
            SortColumn = sortColumn;
            SortOrder = sortOrder;
            CurrentPage = pageNo;

            var customers = await _customerService.SearchCustomersAsync(
        searchName, searchCity, sortColumn, sortOrder, pageNo);

            Customers = _mapper.Map<List<CustomerViewModel>>(customers);
        }

    }

}