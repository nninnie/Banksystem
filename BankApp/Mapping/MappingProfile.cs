using AutoMapper;
using BankApp.DataAccessLayer.DTOs;
using BankApp.DataAccessLayer.Models;
using BankApp.ViewModels;

namespace BankApp.Mapping
{
    // MappingProfile – defines how entities and DTOs are converted into ViewModels
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Maps Customer entity - CustomerViewModel
            CreateMap<Customer, CustomerViewModel>()
                // Creates FullName from Givenname + Surname
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => $"{src.Givenname} {src.Surname}"))

                // Maps telephone country code + number into one string
                .ForMember(dest => dest.Telephone,
                    opt => opt.MapFrom(src => $"{src.Telephonecountrycode}{src.Telephonenumber}"))

                // Maps email address
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.Emailaddress));

            // Maps Account entity - AccountViewModel
            // Created is DateOnly in the database and DateTime in the ViewModel
            CreateMap<Account, AccountViewModel>()
                .ForMember(dest => dest.Created,
                    opt => opt.MapFrom(src => src.Created.ToDateTime(TimeOnly.MinValue)));

            // Maps CustomerDto - CustomerViewModel (Top 10 per country)
            CreateMap<CustomerDto, CustomerViewModel>();

            // Maps AccountDto - AccountViewModel (account page)
            CreateMap<AccountDto, AccountViewModel>();

            // Maps TransactionDto - TransactionViewModel (account page and dashboard)
            CreateMap<TransactionDto, TransactionViewModel>();
        }
    }
}