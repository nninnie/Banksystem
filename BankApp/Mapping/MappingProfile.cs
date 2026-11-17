using AutoMapper;
using BankApp.DataAccessLayer.DTOs;
using BankApp.DataAccessLayer.Models;
using BankApp.Helpers;
using BankApp.ViewModels;

namespace BankApp.Mapping
{
    // MappingProfile - How entities and DTOs become ViewModels
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Customer - CustomerViewModel
            CreateMap<Customer, CustomerViewModel>()
                // Name and phone are put together from two fields
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => $"{src.Givenname} {src.Surname}"))
                .ForMember(dest => dest.Telephone,
                    opt => opt.MapFrom(src => $"{src.Telephonecountrycode}{src.Telephonenumber}"))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.Emailaddress));

            // Account - AccountViewModel (DateOnly in the database, DateTime in the ViewModel)
            CreateMap<Account, AccountViewModel>()
                .ForMember(dest => dest.Created,
                    opt => opt.MapFrom(src => src.Created.ToDateTime(TimeOnly.MinValue)));

            // DTOs - ViewModels (same names, so no extra rules)
            CreateMap<CustomerDto, CustomerViewModel>();
            CreateMap<AccountDto, AccountViewModel>();
            CreateMap<TransactionDto, TransactionViewModel>();

            // Create/edit customer form - Customer
            CreateMap<CustomerFormViewModel, Customer>()
                // Country code is calculated from Country, the user never types it
                .ForMember(dest => dest.CountryCode,
                    opt => opt.MapFrom(src => CountryFlags.IsoCode(src.Country)))
                // Accounts are created by the service, not the form
                .ForMember(dest => dest.Dispositions,
                    opt => opt.Ignore());

            // Customer - edit form (fills the form from the database)
            CreateMap<Customer, CustomerFormViewModel>();

            // User Admin
            CreateMap<UserDto, UserViewModel>();
            CreateMap<UserDto, EditUserViewModel>();

            // Admin page for suspicious transactions
            CreateMap<SuspiciousTransactionDto, SuspiciousTransactionViewModel>();
        }
    }
}