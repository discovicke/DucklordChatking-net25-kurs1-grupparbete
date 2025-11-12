using System;

namespace ChatServer.Models;

public class Responses
{
    public record ApiResponseWithUsername(string Username, string SuccessMessage);
    public record ApiResponseSuccess(string SuccessMessage);
    public record ApiResponseFail(string FailMessage);

}
