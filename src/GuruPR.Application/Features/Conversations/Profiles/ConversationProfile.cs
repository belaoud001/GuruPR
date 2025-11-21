using AutoMapper;

using GuruPR.Application.Features.Conversations.Commands.CreateConversation;
using GuruPR.Application.Features.Conversations.Commands.UpdateConversation;
using GuruPR.Application.Features.Conversations.Dtos;
using GuruPR.Domain.Entities.Conversation;
using GuruPR.Domain.Entities.Conversation.Operations;

namespace GuruPR.Application.Features.Conversations.Profiles;

public class ConversationProfile : Profile
{
    public ConversationProfile()
    {
        CreateMap<Conversation, ConversationDto>().ReverseMap();

        CreateMap<Conversation, CreateConversationCommand>().ReverseMap();

        CreateMap<UpdateConversationCommand, ConversationUpdateData>().ReverseMap();
    }
}
