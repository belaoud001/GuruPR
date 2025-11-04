using AutoMapper;

using GuruPR.Application.Dtos.Conversation;
using GuruPR.Domain.Entities.Conversation;

namespace GuruPR.Application.Profiles.Conversations;

public class ConversationProfile : Profile
{
    public ConversationProfile()
    {
        CreateMap<Conversation, ConversationDto>();
        CreateMap<ConversationDto, Conversation>();

        CreateMap<Conversation, CreateConversationRequest>();
        CreateMap<CreateConversationRequest, Conversation>();

        CreateMap<Conversation, UpdateConversationRequest>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UpdateConversationRequest, Conversation>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
