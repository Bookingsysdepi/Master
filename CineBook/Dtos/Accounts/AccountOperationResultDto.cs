namespace CineBook.Dtos.Accounts;

public class AccountOperationResultDto
{
    public bool Succeeded { get; set; }
    public List<string> Errors { get; set; } = new();
}
