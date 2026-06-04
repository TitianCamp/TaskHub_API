using TaskHub.Business.DataModels.User;

namespace TaskHub.Business.Validators;

public interface IUserValidator :
    IValidator<UserRegisterRequest>, IValidator<UserLoginRequest>
{ }
