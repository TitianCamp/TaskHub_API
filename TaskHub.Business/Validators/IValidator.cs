using TaskHub.Business.Results;

namespace TaskHub.Business.Validators;

public interface IValidator<T>
{
    abstract Result<bool> Validate(T request);
}
