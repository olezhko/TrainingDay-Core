using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TrainingDay.Common.Communication;
using TrainingDay.Common.Extensions;
using TrainingDay.Common.Models;
using TrainingDay.Web.Data.OpenAI;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Server.ViewModels.Exercises;
using TrainingDay.Web.Services.Exercises;

namespace TrainingDay.Web.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExercisesController(TrainingDayContext context,
            IExerciseManager mngExercise,
            IOpenAIService aiService,
            IWebHostEnvironment environment,
            IMapper mapper) : ControllerBase
    {
        [HttpGet("search")]
        public async Task<IActionResult> GetAll(int? selectedMuscle, string? filterName, string twoLetterCulture)
        {
            var result = context.Exercises
                    .Include(item => item.Culture)
                    .AsNoTracking()
                    .Where(item => item.Culture.Code == twoLetterCulture)
                    .OrderBy(item => item.CodeNum)
                    .Select(item => new ExerciseViewModel(item));

            if (selectedMuscle != null)
            {
                result = result.Where(item => item.Muscles.Contains((MusclesEnum)selectedMuscle));
            }

            if (!string.IsNullOrEmpty(filterName))
            {
                result = result.Where(item => item.Name.ToLower().Contains(filterName.ToLower()));
            }

            return Ok(await result.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? codeNum, string twoLetterCulture)
        {
            var exercise = await context.Exercises.Include(item => item.Culture)
                    .AsNoTracking()
                .SingleOrDefaultAsync(item => item.CodeNum == codeNum && item.Culture.Code == twoLetterCulture);

            if (exercise == null)
            {
                return NotFound();
            }

            var exerciseViewModel = mapper.Map<ExerciseViewModel>(exercise);
            return Ok(exerciseViewModel);
        }

        // POST: ExerciseViewModels/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]ExerciseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var exercise = mapper.Map<WebExercise>(model);
            context.Exercises.Add(exercise);
            await context.SaveChangesAsync();
            return Ok(exercise);
        }

        [HttpPost("image")]
        public async Task<IActionResult> SetImage(UploadImageModel model, CancellationToken token)
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
        public async Task<IActionResult> GetEditParams(int cultureId)
        {
            var cu = await context.Cultures.AsNoTracking().FirstOrDefaultAsync(item => item.Id == cultureId);
            var culture = new CultureInfo(cu.Code);
            EditParameters model = new EditParameters();
            model.AllMuscles = new List<SelectListItem>();
            for (int i = 0; i < (int)MusclesEnum.None; i++)
            {
                model.AllMuscles.Add(new SelectListItem
                {
                    Value = i.ToString(),
                    Text = $"{ExerciseExtensions.GetEnumDescription((MusclesEnum)i, culture)}"
                });
            }

            model.AllTags = new List<SelectListItem>();
            for (int i = 0; i < (int)ExerciseTags.Last; i++)
            {
                model.AllTags.Add(new SelectListItem
                {
                    Value = i.ToString(),
                    Text = $"{ExerciseExtensions.GetEnumDescription((ExerciseTags)i, culture)}"
                });
            }

            model.OfferedCode = mngExercise.GetLastCode(cultureId);

            return Ok(model);
        }

        // PUT: ExerciseViewModels/Edit/5
        [HttpPut]
        public async Task<IActionResult> Edit(int id, [FromBody] ExerciseViewModel exerciseViewModel)
        {
            if (id != exerciseViewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var exercise = mapper.Map<WebExercise>(exerciseViewModel);
            context.Update(exercise);
            await context.SaveChangesAsync();
            return Ok(exercise);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
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
        public async Task<ActionResult<IEnumerable<ExerciseQueryResponse>>> GetExercisesByQuery(ExerciseQueryRequest query, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(query.Query))
            {
                return BadRequest("Query parameter is required.");
            }

            var response = await aiService.GetExercisesByQueryAsync(query.Query, token);

            return Ok(response);
        }
    }
}
