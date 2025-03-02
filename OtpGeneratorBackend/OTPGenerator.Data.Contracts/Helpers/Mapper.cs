using AutoMapper;
using OTPGenerator.Data.Contracts.Helpers.DTO;
using OTPGenerator.Data.Objects.Entities;

namespace OTPGenerator.Services.Business.Helpers;
public class Mapper : Profile
{
    public Mapper()
    {
        CreateMap<OTPEntity, OTPDTO>();
        CreateMap<OTPDTO, OTPEntity>();
    }
}
