using System;

namespace ChatServer.Models;

public class Responses
{
    public record ApiSuccessResponseWithUsername(string Username, string SuccessMessage);
    public record ApiSuccessResponse(string SuccessMessage);
    public record ApiFailResponse(string FailMessage);

}
