namespace Application.Features.SampleEntities.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<SampleEntity, GetSampleEntitiesByNumberResponse>()
            .ReverseMap();

        CreateMap<SampleEntity, CreateSampleEntityResponse>()
            .ReverseMap();

        CreateMap<CreateSampleEntityCommand, SampleEntity>()
            .ForMember(x => x.Id, opt => opt.Ignore());
    }
}