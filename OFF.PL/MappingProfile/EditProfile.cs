using AutoMapper;
using OFF.DAL.Model;
using OFF.PL.ViewModels;
using System.Numerics;

namespace OFF.PL.MappingProfile
{
    public class EditProfile:Profile

    {
        public EditProfile()
        {
            CreateMap<EditingVM, PLayer>().ReverseMap();

            CreateMap<PLayer, PlayerRegisterVM>()
     .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber));

            CreateMap<Agent, AgentRegisterVM>()
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber));


            CreateMap<PostVM, Post>().ReverseMap();
            CreateMap<ContactVM, Contact>().ReverseMap();


        }


    }
}
