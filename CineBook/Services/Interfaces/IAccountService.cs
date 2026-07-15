using CineBook.Dtos.Accounts;
namespace CineBook.Services.Interfaces
{
    public interface IAccountService
    {

        Task<AccountOperationResultDto> RegisterAsync(
    RegisterAccountDto dto);

        Task<AccountOperationResultDto> LoginAsync(
            LoginAccountDto dto);

        Task LogoutAsync();
    }
}
