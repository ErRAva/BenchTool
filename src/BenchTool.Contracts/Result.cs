using System.Diagnostics.CodeAnalysis;
using BenchTool.Common;

namespace BenchTool.Contracts;

public record Result<TOutput, TError>
	where TOutput : class
	where TError : class
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
		=> this.ToJsonString();
}
