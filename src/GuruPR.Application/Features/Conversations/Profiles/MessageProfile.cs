using AutoMapper;

using GuruPR.Application.Features.Conversations.Dtos;
using GuruPR.Domain.Entities.Message;

namespace GuruPR.Application.Features.Conversations.Profiles;

public class MessageProfile : Profile
{
    public MessageProfile()
    {

        CreateMap<Message, MessageDto>().ReverseMap();
    }
}
