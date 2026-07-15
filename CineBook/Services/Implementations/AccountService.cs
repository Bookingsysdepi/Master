using CineBook.Dtos.Accounts;
using CineBook.Models;
using CineBook.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CineBook.Services.Implementations;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<AccountOperationResultDto> RegisterAsync(
        RegisterAccountDto dto)
    {
        var user = new ApplicationUser
        {
            FullName = dto.FullName,
            Email = dto.Email,
            UserName = dto.Email
        };

        var createResult = await _userManager.CreateAsync(
            user,
            dto.Password);

        if (!createResult.Succeeded)
        {
            return new AccountOperationResultDto
            {
                Succeeded = false,
                Errors = createResult.Errors
                    .Select(error => error.Description)
                    .ToList()
            };
        }

        var roleResult = await _userManager.AddToRoleAsync(
            user,
            "Customer");

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            return new AccountOperationResultDto
            {
                Succeeded = false,
                Errors = roleResult.Errors
                    .Select(error => error.Description)
                    .ToList()
            };
        }

        await _signInManager.SignInAsync(
            user,
            isPersistent: false);

        return new AccountOperationResultDto
        {
            Succeeded = true
        };
    }

    public async Task<AccountOperationResultDto> LoginAsync(
        LoginAccountDto dto)
    {
        var result = await _signInManager.PasswordSignInAsync(
            dto.Email,
            dto.Password,
            dto.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            return new AccountOperationResultDto
            {
                Succeeded = true
            };
        }

        return new AccountOperationResultDto
        {
            Succeeded = false,
            Errors = new List<string>
            {
                "Invalid login attempt."
            }
        };
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}