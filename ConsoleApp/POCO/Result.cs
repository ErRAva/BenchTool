using System.Diagnostics.CodeAnalysis;
using ConsoleApp.Utils;

namespace ConsoleApp.POCO;

public record Result<TOutput, TError> 
	where TOutput : class//, new()
	 where TError : class//, new()
{
	public bool IsSuccess { get; }
	[MemberNotNullWhen(true, nameof(IsSuccess))]
	public TOutput? Value { get; }
	[MemberNotNullWhen(false, nameof(IsSuccess))]
	public IEnumerable<TError>? Errors { get; }

	private Result(TOutput? value, bool isSuccess, TError? error)
	{
		Value = value;
		Errors = isSuccess ? null : new[] { error! };
		IsSuccess = isSuccess;
	}

	public static Result<TOutput, TError> Success(TOutput value) 
		=> new(value, true, null);
	public static Result<TOutput, TError> Failure(TError error) 
		=> new(null, false, error);

	public static implicit operator Result<TOutput, TError>(TOutput value) 
		=> new(value, true, null);
	public static implicit operator Result<TOutput, TError>(TError error) 
		=> new(null, false, error);

	//// Hmmm... Syntactic sugar is nice and all, but this hides logic, and it may not be worth it.
	//public TFunc Continue<TFunc>(Func<T, TFunc> successFunc, Func<IEnumerable<TError>, TFunc> failureFunc) 
	//// Suppressing nullable warning as the boolean check will protect against using a null value
	//	=> IsSuccess
	//		? successFunc(Value!)
	//		: failureFunc(Error!);
}

public record BenchError
{
	public required string Code { get; init; } = "Unknown";
	public required string Message { get; init; }
}




public record BenchCsvData
{
	public string? RoleId { get; init; }
	public string? Client { get; init; }
	public string? Title { get; init; }
	public string? Skills { get; init; }
	public string? StartDate { get; init; }
	public string? Level { get; init; }
	public string? Contact1Email { get; init; }
	public string? Contact2Email { get; init; }
	public string? Contact3Email { get; init; }
	public string? RoleDescription { get; init; }
	public string? AcceptingResumes { get; set; }

	public IEnumerable<string>? Errors { get; init; }

	public override string ToString()
	{
		return this.ToJsonString();
	}
}