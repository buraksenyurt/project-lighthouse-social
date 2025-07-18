namespace LighthouseSocial.Application.Dtos;

public record CommentDto(Guid UserId,Guid PhotoId,string Text,int Rating);
