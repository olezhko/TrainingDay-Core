using TrainingDay.Common.Models;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Services.Exercises.Models;
using TrainingDay.Web.Services.Mapping;

namespace TrainingDay.Web.Services.Tests.Mapping
{
    public class ExerciseMappingExtensionsTests
    {
        [Fact]
        public void ApplyTo_ThenToModel_KeepsEditableFields()
        {
            var model = new ExerciseModel
            {
                CodeNum = 42,
                Name = "Push-up",
                StartingPosition = "Plank",
                Execution = "Lower and push",
                Advice = "Keep the back straight",
                Muscles = [MusclesEnum.Neck, MusclesEnum.ShouldersFront],
                Tags = [ExerciseTags.CanDoAtHome, ExerciseTags.ExerciseByTime],
                DifficultType = DifficultTypes.Medium,
            };
            var entity = new WebExercise { Id = 7, Culture = new Culture { Id = 2, Code = "en" } };

            model.ApplyTo(entity);
            var result = entity.ToModel();

            Assert.Equal(7, result.Id);
            Assert.Equal("en", result.CultureCode);
            Assert.Equal(model.CodeNum, result.CodeNum);
            Assert.Equal(model.Name, result.Name);
            Assert.Equal(model.StartingPosition, result.StartingPosition);
            Assert.Equal(model.Execution, result.Execution);
            Assert.Equal(model.Advice, result.Advice);
            Assert.Equal(model.Muscles, result.Muscles);
            Assert.Equal(model.Tags, result.Tags);
            Assert.Equal(model.DifficultType, result.DifficultType);
        }

        [Fact]
        public void ToModel_WithoutDescription_LeavesTextEmpty()
        {
            var entity = new WebExercise { Name = "Squat", MusclesString = string.Empty };

            var result = entity.ToModel();

            Assert.Null(result.StartingPosition);
            Assert.Null(result.Execution);
            Assert.Null(result.Advice);
            Assert.Null(result.CultureCode);
            Assert.Empty(result.Muscles);
        }
    }
}
