namespace Raga.Domain.Common;

public interface IBusinessRule
{
    string Message { get; }
    string ErrorCode { get; }
    bool IsBroken();
}