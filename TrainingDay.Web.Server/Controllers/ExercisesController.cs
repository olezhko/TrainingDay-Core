using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using TrainingDay.Common.Communication;
using TrainingDay.Common.Extensions;
using TrainingDay.Common.Models;
using TrainingDay.Web.Data.OpenAI;
using TrainingDay.Web.Server.Extensions;
using TrainingDay.Web.Server.Mapper;
using TrainingDay.Web.Server.ViewModels.Exercises;
using TrainingDay.Web.Services.Exercises;
using TrainingDay.Web.Services.Exercises.Models;

namespace TrainingDay.Web.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExercisesController(
        IExerciseManager mngExercise,
        IAiService aiService,
        IWebHostEnvironment environment) : ControllerBase
{
    [HttpGet("search")]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] MusclesEnum[] selectedMuscle,
        [FromQuery] ExerciseTags[] selectedTags,
        [FromQuery] DifficultTypes[] selectedDifficulty,
        [FromQuery] string? filterName,
        [FromQuery] string twoLetterCulture,
        CancellationToken cancellationToken)
    {
        var filter = new ExerciseSearchFilter(
            twoLetterCulture,
            filterName,
            selectedMuscle ?? [],
            selectedTags ?? [],
            selectedDifficulty ?? []);

        var exercises = await mngExercise.SearchAsync(filter, cancellationToken);
        return Ok(exercises.Select(item => item.ToViewModel()).ToList());
    }

    [HttpGet]
    public async Task<IActionResult> DetailsAsync(int? codeNum, string twoLetterCulture, CancellationToken cancellationToken)
    {
        if (codeNum == null)
        {
            return NotFound();
        }

        var result = await mngExercise.GetByCodeAsync(codeNum.Value, twoLetterCulture, cancellationToken);
        if (result.IsFailure)
        {
            return this.Problem(result.Error);
        }

        return Ok(result.Value.ToViewModel());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAsync([FromBody] ExerciseViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await mngExercise.CreateAsync(model.ToModel(), cancellationToken);
        if (result.IsFailure)
        {
            return this.Problem(result.Error);
        }

        return Ok(result.Value.ToViewModel());
    }

    [HttpPost("image")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetImageAsync(UploadImageModel model, CancellationToken token)
    {
        if (model.ImageFile == null)
        {
            return BadRequest();
        }

        await using var content = model.ImageFile.OpenReadStream();
        var folder = Path.Combine(environment.WebRootPath, "exercise_images");
        var result = await mngExercise.SaveImageAsync(model.CodeNum, content, model.ImageFile.Length, folder, token);
        if (result.IsFailure)
        {
            return this.Problem(result.Error);
        }

        return Ok();
    }

    [HttpGet("editor")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetEditParamsAsync(string cu, CancellationToken cancellationToken)
    {
        var lastCode = await mngExercise.GetLastCodeAsync(cu, cancellationToken);
        if (lastCode.IsFailure)
        {
            return this.Problem(lastCode.Error);
        }

        var culture = new CultureInfo(cu);

        EditParameters model = new EditParameters();
        model.AllMuscles = new List<SelectListItem>();
        var muscleCollection = Enum.GetValues<MusclesEnum>();
        for (int i = 0; i < muscleCollection.Length; i++)
        {
            model.AllMuscles.Add(new SelectListItem
            {
                Value = i.ToString(),
                Text = $"{ExerciseExtensions.GetEnumDescription(muscleCollection[i], culture)}"
            });
        }

        var exerciseTagCollection = Enum.GetValues<ExerciseTags>();
        model.AllTags = new List<SelectListItem>();
        for (int i = 0; i < exerciseTagCollection.Length; i++)
        {
            model.AllTags.Add(new SelectListItem
            {
                Value = i.ToString(),
                Text = $"{ExerciseExtensions.GetEnumDescription(exerciseTagCollection[i], culture)}"
            });
        }

        model.OfferedCode = lastCode.Value;

        return Ok(model);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditAsync([FromRoute] int id, [FromBody] ExerciseViewModel exerciseViewModel, CancellationToken cancellationToken)
    {
        if (id != exerciseViewModel.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await mngExercise.UpdateAsync(id, exerciseViewModel.ToModel(), cancellationToken);
        if (result.IsFailure)
        {
            return this.Problem(result.Error);
        }

        return Ok(result.Value.ToViewModel());
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var result = await mngExercise.DeleteAsync(id, cancellationToken);
        if (result.IsFailure)
        {
            return this.Problem(result.Error);
        }

        return Ok();
    }

    [HttpPost("query")]
    public async Task<IActionResult> GetExercisesByQueryAsync(ExerciseQueryRequest query, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(query.Query))
        {
            return BadRequest("Query parameter is required.");
        }

        var response = await aiService.GetExercisesByQueryAsync(query.Query, token);

        return Ok(response);
    }

    [HttpPost("query/extended")]
    public async Task<IActionResult> GetExercisesByQueryExtendedAsync(ExerciseQueryRequest query, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(query.Query))
        {
            return BadRequest("Query parameter is required.");
        }

        var queryResponse = await aiService.GetExercisesByQueryAsync(query.Query, token);
        var exercises = await mngExercise.GetExercisesByCodesAsync(queryResponse.Exercises.Select(item => item.Guid), token);
        return Ok(exercises.Select(item => item.ToResponse()).ToList());
    }
}
