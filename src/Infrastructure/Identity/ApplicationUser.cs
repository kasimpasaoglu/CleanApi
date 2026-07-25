
using Application.Common.Models;
using Domain.Interfaces;

namespace Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string FullName => $"{FirstName} {LastName}".Trim(); // kolon yok, computed property
    public Guid? DepartmentId { get; private set; }
    public bool IsCompletedRegistration { get; private set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }
    public string? DeletedBy { get; set; }

    
    public void InitRegistration(string firstName, string lastName, string phone,string email, Guid departmentId)
    {

        FirstName = StringHelpers.CapitalizeEachWord(firstName);
        LastName = StringHelpers.CapitalizeEachWord(lastName);
        PhoneNumber = PhoneHelpers.NormalizePhoneNumber(phone);
        Email = email;
        DepartmentId = departmentId;
        UserName = email.Split('@')[0];
        IsCompletedRegistration = false;
    }

 
    public void SetIsCompletedRegistration()
    {
        IsCompletedRegistration = !string.IsNullOrWhiteSpace(FirstName)
                                  && !string.IsNullOrWhiteSpace(LastName)
                                  && !string.IsNullOrWhiteSpace(PhoneNumber)
                                  && DepartmentId != Guid.Empty
                                  && DepartmentId.HasValue;
    }

    public void UnMarkRegistrationCompleted()
    {
        IsCompletedRegistration = false;
    }
    
    

    public void SoftDelete(string deletedBy, DateTimeOffset now)
    {
        if (IsDeleted) return;

        IsDeleted = true;
        DeletedDate = now;
        DeletedBy = deletedBy;


        var suffix = $"__deleted__{Id}__{now:yyyyMMddHHmmss}";

        if (!string.IsNullOrWhiteSpace(Email))
            Email = $"{Email}{suffix}";

        if (!string.IsNullOrWhiteSpace(NormalizedEmail))
            NormalizedEmail = $"{NormalizedEmail}{suffix}";

        if (!string.IsNullOrWhiteSpace(UserName))
            UserName = $"{UserName}{suffix}";

        if (!string.IsNullOrWhiteSpace(NormalizedUserName))
            NormalizedUserName = $"{NormalizedUserName}{suffix}";
        
        LockoutEnabled = true;
        LockoutEnd = DateTimeOffset.MaxValue;
    }
    
    public UserDto ToUserDto(IEnumerable<string>? roles = null)
    {
        return new UserDto
        {
            Id = Id,
            FirstName = FirstName,
            LastName = LastName,
            UserName = UserName,
            FullName = FullName,
            Email = Email,
            PhoneNumber = PhoneNumber,
            DepartmentId = DepartmentId,
            IsCompletedRegistration = IsCompletedRegistration,
            AccessFailedCount = AccessFailedCount,
            IsLockedOut = LockoutEnd.HasValue,
            LockoutEndDate = LockoutEnd,
            Roles = roles?.ToList() ?? []
        };
    }



}
