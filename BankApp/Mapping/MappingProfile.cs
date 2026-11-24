using AutoMapper;
using BankApp.DataAccessLayer.DTOs;
using BankApp.DataAccessLayer.Models;
using BankApp.Helpers;
using BankApp.ViewModels;

namespace BankApp.Mapping
{
    // MappingProfile -  Defines how entities, DTOs and ViewModels are converted into each other
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Customer, CustomerViewModel>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => $"{src.Givenname} {src.Surname}"))
                .ForMember(dest => dest.Telephone,
                    opt => opt.MapFrom(src => $"{src.Telephonecountrycode}{src.Telephonenumber}"))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.Emailaddress));

            CreateMap<Account, AccountViewModel>()
                .ForMember(dest => dest.Created,
                    opt => opt.MapFrom(src => src.Created.ToDateTime(TimeOnly.MinValue)));

            CreateMap<CustomerDto, CustomerViewModel>();
            CreateMap<AccountDto, AccountViewModel>();
            CreateMap<TransactionDto, TransactionViewModel>();

            CreateMap<CustomerFormViewModel, Customer>()
                .ForMember(dest => dest.CountryCode,
                    opt => opt.MapFrom(src => CountryFlags.IsoCode(src.Country)))
                .ForMember(dest => dest.Dispositions,
                    opt => opt.Ignore());

            CreateMap<Customer, CustomerFormViewModel>();

            CreateMap<UserDto, UserViewModel>();
            CreateMap<UserDto, EditUserViewModel>();

            CreateMap<SuspiciousTransactionDto, SuspiciousTransactionViewModel>();
        }
    }
}