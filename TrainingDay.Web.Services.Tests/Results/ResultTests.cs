using TrainingDay.Web.Data.Results;
using TrainingDay.Web.Services.Exercises;

namespace TrainingDay.Web.Services.Tests.Results
{
    public class ResultTests
    {
        [Fact]
        public void Success_HasNoError()
        {
            var result = Result.Success();

            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Equal(Error.None, result.Error);
        }

        [Fact]
        public void Failure_CarriesError()
        {
            var result = Result.Failure(ExerciseErrors.NotFound);

            Assert.True(result.IsFailure);
            Assert.Equal(ExerciseErrors.NotFound, result.Error);
        }

        [Fact]
        public void Failure_WithNoneError_Throws()
        {
            Assert.Throws<ArgumentException>(() => Result.Failure(Error.None));
        }

        [Fact]
        public void ValueResult_ImplicitFromValue_IsSuccess()
        {
            Result<int> result = 42;

            Assert.True(result.IsSuccess);
            Assert.Equal(42, result.Value);
        }

        [Fact]
        public void ValueResult_ImplicitFromError_IsFailure()
        {
            Result<int> result = ExerciseErrors.UnknownCulture;

            Assert.True(result.IsFailure);
            Assert.Equal("Exercises.UnknownCulture", result.Error.Code);
            Assert.Equal(ErrorType.Validation, result.Error.Type);
        }

        [Fact]
        public void ValueResult_ValueOfFailure_Throws()
        {
            Result<string> result = ExerciseErrors.NotFound;

            Assert.Throws<InvalidOperationException>(() => result.Value);
        }

        [Fact]
        public void Errors_CompareByValue()
        {
            var copy = Error.NotFound("Exercises.NotFound", "Exercise not found.");

            Assert.Equal(ExerciseErrors.NotFound, copy);
            Assert.True(ExerciseErrors.NotFound == copy);
        }
    }
}
