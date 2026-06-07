using TaskHub.Business.DataModels.Team;
using TaskHub.Business.Results;

namespace TaskHub.Business.Validators;

public class TeamValidator : IValidator<TeamRequest>
{
    public Result<bool> Validate(TeamRequest request)
    {
        if (request == null)
        {
            return Result.Failure<bool>("Team model is null");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result.Failure<bool>("Team name is required");
        }
        if (string.IsNullOrWhiteSpace(request.Description))
        {
            return Result.Failure<bool>("Team description is required");
        }

        return Result.Success(true);
    }
}
