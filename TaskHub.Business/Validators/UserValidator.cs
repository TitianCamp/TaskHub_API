using System.ComponentModel.DataAnnotations;
using TaskHub.Business.DataModels.User;
using TaskHub.Business.Results;

namespace TaskHub.Business.Validators;

public class UserValidator : 
    IValidator<UserRegisterRequest>, IValidator<UserLoginRequest>
{
    public Result<bool> Validate(UserRegisterRequest request)
    {
        if (request == null)
        {
            return Result.Failure<bool>("User Register model is null");
        }

        if (string.IsNullOrWhiteSpace(request.FirstName))
        {
            return Result.Failure<bool>("First name is required");
        }

        if (string.IsNullOrWhiteSpace(request.LastName))
        {
            return Result.Failure<bool>("Last name is required");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Result.Failure<bool>("Email is required");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return Result.Failure<bool>("Password is required");
        }

        EmailAddressAttribute emailAddressAttribute = new();
        if (!emailAddressAttribute.IsValid(request.Email))
        {
            return Result.Failure<bool>("Invalid email format");
        }

        return Result.Success(true);
    }

    public Result<bool> Validate(UserLoginRequest request)
    {
        if (request == null)
        {
            return Result.Failure<bool>("User Login model is null");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Result.Failure<bool>("Email is required");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return Result.Failure<bool>("Password is required");
        }

        EmailAddressAttribute emailAddressAttribute = new();
        if (!emailAddressAttribute.IsValid(request.Email))
        {
            return Result.Failure<bool>("Invalid email format");
        }

        return Result.Success(true);
    }
}
