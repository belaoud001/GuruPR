using AutoMapper;

using GuruPR.Application.Features.Conversations.Commands.CreateConversation;
using GuruPR.Application.Features.Conversations.Commands.UpdateConversation;
using GuruPR.Application.Features.Conversations.Dtos;
using GuruPR.Domain.Entities.Conversation;
using GuruPR.Domain.Entities.Conversation.Operations;

namespace GuruPR.Application.Profiles.Conversations;

public class ConversationProfile : Profile
{
    public ConversationProfile()
    {
        CreateMap<Conversation, ConversationDto>();
        CreateMap<ConversationDto, Conversation>();

        CreateMap<Conversation, CreateConversationCommand>();
        CreateMap<CreateConversationCommand, Conversation>();

        CreateMap<UpdateConversationCommand, ConversationUpdateData>();
        CreateMap<ConversationUpdateData, UpdateConversationCommand>();
    }
}
