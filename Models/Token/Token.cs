namespace MobileBackend.Models;

public class Token{
    public string AccessToken {get; set;} = null!;
    public string RefreshToken {get; set;} = null!;
}