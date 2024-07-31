using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TrainingDay.Common;
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
            IWebHostEnvironment environment,
            IMapper mapper) : ControllerBase
    {
        [HttpGet("search")]
        public async Task<IActionResult> GetAll(int? selectedMuscle, string? filterName, string twoLetterCulture)
        {
            IEnumerable<ExerciseViewModel> result = new List<ExerciseViewModel>();
            if (selectedMuscle != null)
            {
                result = await context.Exercises.AsNoTracking()
                    .Where(item => item.Culture == twoLetterCulture)
                    .OrderBy(item => item.CodeNum)
                    .Select(item => new ExerciseViewModel(item)).ToListAsync();

                result = result.Where(item => item.Muscles.Contains((MusclesEnum)selectedMuscle)).ToList();
            }
            else
            {
                result = await context.Exercises.AsNoTracking()
                    .Where(item => item.Culture == twoLetterCulture)
                    .OrderBy(item => item.CodeNum)
                    .Select(item => new ExerciseViewModel(item)).ToListAsync();
            }

            if (!string.IsNullOrEmpty(filterName))
            {
                result = result.Where(item => item.ExerciseItemName.ToLower().Contains(filterName.ToLower())).ToList();
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? codeNum, string twoLetterCulture)
        {
            var exercise = await context.Exercises
                .SingleOrDefaultAsync(item => item.CodeNum == codeNum && item.Culture == twoLetterCulture);

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
        public async Task<IActionResult> GetEditParams(string cu)
        {
            var culture = new CultureInfo(cu);
            EditParameters model = new EditParameters();
            model.AllMuscles = new List<SelectListItem>();
            for (int i = 0; i < (int)MusclesEnum.None; i++)
            {
                model.AllMuscles.Add(new SelectListItem
                {
                    Value = i.ToString(),
                    Text = $"{ExerciseTools.GetEnumDescription((MusclesEnum)i, culture)}"
                });
            }

            model.AllTags = new List<SelectListItem>();
            for (int i = 0; i < (int)ExerciseTags.Last; i++)
            {
                model.AllTags.Add(new SelectListItem
                {
                    Value = i.ToString(),
                    Text = $"{ExerciseTools.GetEnumDescription((ExerciseTags)i, culture)}"
                });
            }

            model.OfferedCode = mngExercise.GetLastCode(cu);

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
    }
}
