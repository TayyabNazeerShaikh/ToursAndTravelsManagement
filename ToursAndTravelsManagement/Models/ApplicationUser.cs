﻿using Microsoft.AspNetCore.Identity;

namespace ToursAndTravelsManagement.Models;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? Address { get; set; } = string.Empty;
    public string? City { get; set; } = string.Empty;
    public string? Country { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public bool IsActive { get; set; }
}