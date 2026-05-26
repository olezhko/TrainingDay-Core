using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TrainingDay.Common.Communication;
using TrainingDay.Common.Extensions;
using TrainingDay.Common.Models;
using TrainingDay.Web.Data.OpenAI;
using TrainingDay.Web.Database;
using TrainingDay.Web.Server.Mapper;
using TrainingDay.Web.Server.ViewModels.Exercises;
using TrainingDay.Web.Services.Exercises;

namespace TrainingDay.Web.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExercisesController(TrainingDayContext context,
            IExerciseManager mngExercise,
            IOpenAIService aiService,
            IWebHostEnvironment environment) : ControllerBase
    {
        [HttpGet("search")]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] MusclesEnum[] selectedMuscle,
            [FromQuery] ExerciseTags[] selectedTags,
            [FromQuery] string? filterName,
            [FromQuery] string twoLetterCulture,
            CancellationToken cancellationToken)
        {
            var query = context.Exercises
                .Include(item => item.Culture)
                .AsNoTracking()
                .Where(item => item.Culture.Code == twoLetterCulture);

            if (!string.IsNullOrEmpty(filterName))
            {
                query = query.Where(item => item.Name.Contains(filterName, StringComparison.CurrentCultureIgnoreCase));
            }

            var entities = await query.OrderBy(item => item.CodeNum).ToListAsync(cancellationToken);
            IEnumerable<ExerciseViewModel> result = entities.Select(item => new ExerciseViewModel(item));

            if (selectedMuscle != null && selectedMuscle.Length > 0)
            {
                result = result.Where(item => selectedMuscle.Any(m => item.Muscles.Contains(m)));
            }

            if (selectedTags != null && selectedTags.Length > 0)
            {
                result = result.Where(item => selectedTags.Any(t => item.Tags.Contains(t)));
            }

            return Ok(result.ToList());
        }

        [HttpGet]
        public async Task<IActionResult> DetailsAsync(int? codeNum, string twoLetterCulture)
        {
            var exercise = await context.Exercises.Include(item => item.Culture)
                    .AsNoTracking()
                .SingleOrDefaultAsync(item => item.CodeNum == codeNum && item.Culture.Code == twoLetterCulture);

            if (exercise == null)
            {
                return NotFound();
            }

            return Ok(exercise.ToViewModel());
        }

        // POST: ExerciseViewModels/Create
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ExerciseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var exercise = model.ToEntity();
            context.Exercises.Add(exercise);
            await context.SaveChangesAsync();
            return Ok(exercise);
        }

        [HttpPost("image")]
        public async Task<IActionResult> SetImageAsync(UploadImageModel model, CancellationToken token)
        {
            if (model.ImageFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.ImageFile.CopyToAsync(memoryStream);

                    // Upload the file if less than 2 MB
                    if (memoryStream.Length < 2097152)
                    {
                        var folder = Path.Combine(environment.WebRootPath, "exercise_images");
                        string filePath = Path.Combine(folder, model.CodeNum + ".jpg");
                        using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(fileStream);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("File", "The file is too large.");
                    }
                }

                return Ok();
            }

            return BadRequest();
        }

        [HttpGet("editor")]
        public async Task<IActionResult> GetEditParamsAsync(int cultureId)
        {
            var cu = await context.Cultures.AsNoTracking().FirstOrDefaultAsync(item => item.Id == cultureId);
            var culture = new CultureInfo(cu.Code);
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

            model.OfferedCode = mngExercise.GetLastCode(cultureId);

            return Ok(model);
        }

        // PUT: ExerciseViewModels/Edit/5
        [HttpPut]
        public async Task<IActionResult> EditAsync(int id, [FromBody] ExerciseViewModel exerciseViewModel)
        {
            if (id != exerciseViewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var exercise = exerciseViewModel.ToEntity();
            context.Update(exercise);
            await context.SaveChangesAsync();
            return Ok(exercise);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (!ExerciseExists(id))
            {
                return NotFound();
            }

            var exerciseViewModel = await context.Exercises.FindAsync(id);
            context.Exercises.Remove(exerciseViewModel);
            await context.SaveChangesAsync();
            return Ok();
        }

        private bool ExerciseExists(int id)
        {
            return context.Exercises.Any(e => e.Id == id);
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
            var response = await mngExercise.GetExercisesByCodesAsync(queryResponse.Exercises.Select(item => item.Guid), token);
            return Ok(response);
        }
    }
}
