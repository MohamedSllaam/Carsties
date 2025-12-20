using AutoMapper;
using Contracts;

namespace SearchService.RequestHelpers
{
    public class MappingProfiles:Profile
    {        
        public MappingProfiles()
        {
            CreateMap<AuctioCreated, Item>(); 
            CreateMap<AuctionUpdated, Item>(); 
        }
    }
}