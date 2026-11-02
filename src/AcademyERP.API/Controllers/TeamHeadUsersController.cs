using System.ComponentModel.DataAnnotations;
using System.Globalization;
using AcademyERP.Application.Constants;
using AcademyERP.Domain.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/team-head-users")]
[Authorize(Roles = Roles.SuperAdmin + "," + Roles.Administrator)]
public class TeamHeadUsersController : ControllerBase
{
    private const int MaximumTeamHeads = 8;
    private readonly UserManager<ApplicationUser> _userManager;

    public TeamHeadUsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userManager.GetUsersInRoleAsync(Roles.TeamHead);
        var results = new List<TeamHeadUserResponse>(users.Count);

        foreach (var user in users.OrderBy(x => x.FullName))
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var lastLoginValue = claims
                .FirstOrDefault(x => x.Type == AccountClaimTypes.LastLoginUtc)?.Value;
            DateTime? lastLogin = DateTime.TryParse(
                lastLoginValue,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out var parsed)
                ? parsed
                : null;

            results.Add(new TeamHeadUserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Username = user.UserName ?? user.Email ?? string.Empty,
                Email = user.Email ?? string.Empty,
                IsActive = user.IsActive,
                LastLoginAt = lastLogin
            });
        }

        return Ok(results);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTeamHeadUserRequest request)
    {
        var name = request.FullName.Trim();
        var email = request.Email.Trim();
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(request.TemporaryPassword))
            return BadRequest("Name, email, and temporary password are required.");

        var teamHeads = await _userManager.GetUsersInRoleAsync(Roles.TeamHead);
        if (teamHeads.Count(user => user.IsActive) >= MaximumTeamHeads)
            return BadRequest("The maximum of eight active Team Head accounts has been reached.");

        if (await _userManager.FindByEmailAsync(email) is not null ||
            await _userManager.FindByNameAsync(email) is not null)
            return Conflict("An account with this username or email already exists.");

        var user = new ApplicationUser
        {
            FullName = name,
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            IsActive = true
        };

        var createResult = await _userManager.CreateAsync(user, request.TemporaryPassword);
        if (!createResult.Succeeded)
            return BadRequest(string.Join(" ", createResult.Errors.Select(x => x.Description)));

        var roleResult = await _userManager.AddToRoleAsync(user, Roles.TeamHead);
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            return BadRequest("Could not assign Team Head access.");
        }

        return Ok(new { user.Id, user.FullName, user.Email });
    }

    [HttpPut("{id:guid}/username")]
    public async Task<IActionResult> UpdateUsername(Guid id, UpdateTeamHeadUsernameRequest request)
    {
        var username = request.Username.Trim();
        if (string.IsNullOrWhiteSpace(username) ||
            !new EmailAddressAttribute().IsValid(username))
            return BadRequest("Enter a valid email-style username; this is also used to sign in.");

        var user = await FindTeamHeadAsync(id);
        if (user is null)
            return NotFound("Team Head account not found.");

        var existingByEmail = await _userManager.FindByEmailAsync(username);
        var existingByName = await _userManager.FindByNameAsync(username);
        if ((existingByEmail is not null && existingByEmail.Id != id) ||
            (existingByName is not null && existingByName.Id != id))
            return Conflict("That username or email is already in use.");

        user.Email = username;
        user.UserName = username;
        user.EmailConfirmed = true;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(string.Join(" ", result.Errors.Select(x => x.Description)));

        return Ok(new { user.Id, user.FullName, user.Email, Username = user.UserName });
    }

    [HttpPut("{id:guid}/password")]
    public async Task<IActionResult> ResetPassword(Guid id, ResetTeamHeadPasswordRequest request)
    {
        var user = await FindTeamHeadAsync(id);
        if (user is null)
            return NotFound("Team Head account not found.");
        if (string.IsNullOrWhiteSpace(request.NewPassword))
            return BadRequest("Enter a new password.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
        if (!result.Succeeded)
            return BadRequest(string.Join(" ", result.Errors.Select(x => x.Description)));

        return NoContent();
    }

    [HttpPut("{id:guid}/active")]
    public async Task<IActionResult> SetActive(Guid id, SetTeamHeadActiveRequest request)
    {
        var user = await FindTeamHeadAsync(id);
        if (user is null)
            return NotFound("Team Head account not found.");

        user.IsActive = request.IsActive;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(string.Join(" ", result.Errors.Select(x => x.Description)));

        return NoContent();
    }

    private async Task<ApplicationUser?> FindTeamHeadAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        return user is not null && await _userManager.IsInRoleAsync(user, Roles.TeamHead)
            ? user
            : null;
    }
}

public sealed class CreateTeamHeadUserRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TemporaryPassword { get; set; } = string.Empty;
}

public sealed class UpdateTeamHeadUsernameRequest
{
    public string Username { get; set; } = string.Empty;
}

public sealed class ResetTeamHeadPasswordRequest
{
    public string NewPassword { get; set; } = string.Empty;
}

public sealed class SetTeamHeadActiveRequest
{
    public bool IsActive { get; set; }
}

public sealed class TeamHeadUserResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
