namespace TodoApi.Models;

public class UserDTO
{
    public long Id { get; set; }
    public string UserDocument { get; set; }
    public string CreditCardToken { get; set; }
    public long Value { get; set; }
}