using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TrainingDay.Common.Models;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Entities.MobileItems;

namespace TrainingDay.Web.Server.Controllers
{
    public class UserRepoController(TrainingDayContext context, ILogger<MobileTokensController> logger) : ControllerBase
    {
        private MobileUser GetUserOrCreate(int tokenId)
        {
            var userToken = context.UserTokens.FirstOrDefault(item => item.TokenId == tokenId);
            if (userToken == null)
            {
                var user = new MobileUser();
                context.MobileUsers.Add(user);
                context.SaveChanges();

                var newUserToken = new UserMobileToken()
                {
                    TokenId = tokenId,
                    UserId = user.Id,
                };

                context.UserTokens.Add(newUserToken);
                context.SaveChanges();
                return user;
            }
            else
            {
                return context.MobileUsers.First(item => item.Id == userToken.UserId);
            }
        }

        [HttpGet("repo_sync")]
        public async Task<IActionResult> GetUserRepo([FromQuery] string mail, string token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await context.MobileUsers.FirstOrDefaultAsync(a => a.Email == mail);
            if (user == null)
            {
                return NotFound(mail);
            }

            RepositoryBase data = new RepositoryBase();
            var userWeights = context.UserWeightNotes.Where(item => item.UserId == user.Id);
            var userExercises = context.UserExercises.Where(item => item.UserId == user.Id);
            var userTrainingExercises = context.UserTrainingExercises.Where(item => item.UserId == user.Id);
            var userTrainings = context.UserTrainings.Where(item => item.UserId == user.Id);
            var userSuperSets = context.UserSuperSets.Where(item => item.UserId == user.Id);
            var userLastTrainingExercise = context.UserLastTrainingExercises.Where(item => item.UserId == user.Id);
            var userLastTraining = context.UserLastTrainings.Where(item => item.UserId == user.Id);
            var userTrainingGroups = context.UserTrainingGroups.Where(item => item.UserId == user.Id);

            data.Exercises = userExercises.Select(item => new Entities.WebExercise()
            {
                Id = item.DatabaseId,
                Description = JsonConvert.SerializeObject(item.Description),
                Name = item.Name,
                MusclesString = item.MusclesString,
                TagsValue = item.TagsValue,
                CodeNum = item.CodeNum,
            }).ToList();
            data.BodyControl = userWeights.Select(item => new WeightNote()
            {
                Id = item.DatabaseId,
                Date = item.Date,
                Weight = item.Weight,
                Type = item.Type
            }).ToList();
            data.TrainingExercise = userTrainingExercises.Select(item => new TrainingExerciseComm()
            {
                TrainingId = item.TrainingId,
                Id = item.DatabaseId,
                ExerciseId = item.ExerciseId,
                SuperSetId = item.SuperSetId,
                WeightAndRepsString = item.WeightAndRepsString,
                OrderNumber = item.OrderNumber
            }).ToList();
            data.Trainings = userTrainings.Select(item => new Training()
            {
                Id = item.DatabaseId,
                Title = item.Title
            }).ToList();
            data.SuperSets = userSuperSets.Select(item => new SuperSet()
            {
                Id = item.DatabaseId,
                TrainingId = item.TrainingId,
                Count = item.Count
            }).ToList();
            data.LastTrainingExercises = userLastTrainingExercise.Select(item => new LastTrainingExercise()
            {
                LastTrainingId = item.LastTrainingId,
                Id = item.DatabaseId,
                ExerciseName = item.ExerciseName,
                SuperSetId = item.SuperSetId,
                WeightAndRepsString = item.WeightAndRepsString,
                OrderNumber = item.OrderNumber,
                Description = item.Description,
                MusclesString = item.MusclesString,
                TagsValue = item.TagsValue
            }).ToList();
            data.LastTrainings = userLastTraining.Select(item => new LastTraining()
            {
                Id = item.DatabaseId,
                TrainingId = item.TrainingId,
                Title = item.Title,
                Time = item.Time,
                ElapsedTime = item.ElapsedTime
            }).ToList();
            data.TrainingUnions = userTrainingGroups.Select(item => new TrainingUnion()
            {
                Id = item.DatabaseId,
                Name = item.Name,
                TrainingIDsString = item.TrainingIDsString,
            }).ToList();
            //var dataString = JsonConvert.SerializeObject(data);
            //System.IO.File.WriteAllText(dataString,);
            //prepare data
            //string filename = user.Id + ".zip";
            //return File(dataString, "application/octet-stream", filename);

            return Ok(data);
        }
    }
}
