using Microsoft.AspNetCore.Identity;
using System;

namespace DynamicFormBuilder.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        // Full Name Property
        public string FullName => $"{FirstName} {LastName}";
    }
}