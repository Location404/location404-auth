using FluentAssertions;
using Location404.Auth.Application.Common.Result;

namespace Location404.Auth.Application.UnitTests.Common;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessResult()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_ShouldCreateFailureResult()
    {
        var error = new Error("TestError", "Test error message", ErrorType.Validation);

        var result = Result.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Constructor_WithSuccessTrueAndNonNoneError_ShouldThrowInvalidOperationException()
    {
        var error = new Error("TestError", "Test error", ErrorType.Validation);

        var act = () => new TestResult(true, error);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Constructor_WithSuccessFalseAndNoneError_ShouldThrowInvalidOperationException()
    {
        var act = () => new TestResult(false, Error.None);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void SuccessGeneric_ShouldCreateSuccessResultWithValue()
    {
        var value = "test value";

        var result = Result.Success(value);

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().Be(value);
        result.Error.Should().Be(Error.None);
    }

    private class TestResult : Result
    {
        public TestResult(bool isSuccess, Error error) : base(isSuccess, error)
        {
        }
    }
}

public class ResultOfTTests
{
    [Fact]
    public void Success_ShouldCreateSuccessResultWithValue()
    {
        var value = 42;

        var result = Result<int>.Success(value);

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().Be(value);
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_ShouldCreateFailureResult()
    {
        var error = new Error("TestError", "Test error message", ErrorType.Validation);

        var result = Result<int>.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Value_WhenSuccess_ShouldReturnValue()
    {
        var value = "test value";
        var result = Result<string>.Success(value);

        var returnedValue = result.Value;

        returnedValue.Should().Be(value);
    }

    [Fact]
    public void Value_WhenFailure_ShouldThrowInvalidOperationException()
    {
        var error = new Error("TestError", "Test error", ErrorType.Validation);
        var result = Result<string>.Failure(error);

        var act = () => result.Value;

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Match_WhenSuccess_ShouldExecuteOnSuccessFunction()
    {
        var value = 42;
        var result = Result<int>.Success(value);

        var matchResult = result.Match(
            onSuccess: v => $"Success: {v}",
            onFailure: e => $"Failure: {e.Message}");

        matchResult.Should().Be("Success: 42");
    }

    [Fact]
    public void Match_WhenFailure_ShouldExecuteOnFailureFunction()
    {
        var error = new Error("TestError", "Test error message", ErrorType.Validation);
        var result = Result<int>.Failure(error);

        var matchResult = result.Match(
            onSuccess: v => $"Success: {v}",
            onFailure: e => $"Failure: {e.Message}");

        matchResult.Should().Be("Failure: Test error message");
    }

    [Fact]
    public void Match_WithComplexTransformation_ShouldWork()
    {
        var result = Result<int>.Success(10);

        var transformed = result.Match(
            onSuccess: v => v * 2,
            onFailure: _ => 0);

        transformed.Should().Be(20);
    }

    [Fact]
    public void Success_WithReferenceType_ShouldStoreValue()
    {
        var value = new TestClass { Id = 1, Name = "Test" };

        var result = Result<TestClass>.Success(value);

        result.Value.Should().BeSameAs(value);
        result.Value.Id.Should().Be(1);
        result.Value.Name.Should().Be("Test");
    }

    [Fact]
    public void Success_WithNull_ShouldAllowNullValue()
    {
        string? value = null;

        var result = Result<string?>.Success(value);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    private class TestClass
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
