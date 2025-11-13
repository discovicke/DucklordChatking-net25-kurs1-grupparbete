using System;

namespace Shared;

public class UpdateUserDTO
{
    public string? OldUsername { get; set; }
    public string? NewUsername { get; set; }
    public string? Password { get; set; }
}