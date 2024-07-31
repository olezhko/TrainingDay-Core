using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TrainingDay.Common;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities.MobileItems;
using TrainingDay.Web.Entities;

namespace TrainingDay.Web.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileTokensController : ControllerBase
    {
        private readonly TrainingDayContext _context;
        private readonly ILogger<MobileTokensController> _logger;

        public MobileTokensController(TrainingDayContext context, ILogger<MobileTokensController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostMobileToken([FromBody] FirebaseToken mobileTokenConnection)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var token = _context.MobileTokens.FirstOrDefault(item => item.Token == mobileTokenConnection.Token);
                if (token != null)
                {
                    token.Language = mobileTokenConnection.Language;
                    token.Zone = mobileTokenConnection.Zone;
                    token.LastSend = DateTime.Now;
                    _context.Update(token);
                    await _context.SaveChangesAsync();
                    return Ok();
                }

                MobileToken newItem = new MobileToken();
                newItem.Token = mobileTokenConnection.Token;
                newItem.Zone = mobileTokenConnection.Zone;
                newItem.Language = mobileTokenConnection.Language;
                newItem.LastSend = DateTime.Now;

                _context.MobileTokens.Add(newItem);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "PostMobileToken error");
                return BadRequest();
            }
        }

        [HttpPost("mobile-action")]
        public async Task<IActionResult> PostMobileAction([FromBody] MobilenAction token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mobileToken = await _context.MobileTokens.SingleOrDefaultAsync(m => m.Token == token.Token);
            if (mobileToken == null)
            {
                return NotFound(token);
            }

            switch (token.Action)
            {
                case MobileActions.Enter:
                    mobileToken.LastSend = DateTime.UtcNow;
                    break;
                case MobileActions.Workout:
                    mobileToken.LastWorkoutDateTime = DateTime.UtcNow;
                    break;
                case MobileActions.Weight:
                    mobileToken.LastBodyControlDateTime = DateTime.UtcNow;
                    break;
                default:
                    break;
            }
            
            _context.Update(mobileToken);
            await _context.SaveChangesAsync();

            return Ok(mobileToken);
        }

        // GET: api/MobileTokens
        [HttpGet("all")]
        [Authorize(Roles = "admin")]
        public IEnumerable<MobileToken> GetMobileTokens()
        {
            try
            {
                var items = _context.MobileTokens.ToList();
                return items;
            }
            catch (Exception e)
            {
                return new List<MobileToken>();
            }
        }

        [HttpGet("all={days}")]
        [Authorize(Roles = "admin")]
        public IEnumerable<MobileToken> GetMobileTokens(int days)
        {
            try
            {
                var items = _context.MobileTokens.ToList().Where(item => (DateTime.Now - item.LastSend) < TimeSpan.FromDays(days)).AsEnumerable();
                return items.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<MobileToken>();
            }
        }

        // GET: api/MobileTokens/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetMobileToken([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mobileToken = await _context.MobileTokens.SingleOrDefaultAsync(m => m.Id == id);

            if (mobileToken == null)
            {
                return NotFound();
            }

            return Ok(mobileToken);
        }

        [HttpPost("delete/{token}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete([FromRoute] string token)
        {
            var mobileToken = await _context.MobileTokens.SingleOrDefaultAsync(m => m.Token == token);
            if (mobileToken == null)
            {
                return NotFound(token);
            }

            _context.MobileTokens.Remove(mobileToken);
            await _context.SaveChangesAsync();

            return Ok(mobileToken);
        }

        private bool MobileTokenExists(int id)
        {
            return _context.MobileTokens.Any(e => e.Id == id);
        }

        [HttpPost("del_unused")]
        public async Task<IActionResult> RemoveUnusedTokens()
        {
            foreach (var contextMobileToken in _context.MobileTokens)
            {
                if (contextMobileToken.LastSend == new DateTime())
                {
                    _context.MobileTokens.Remove(contextMobileToken);
                    continue;
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("repo_sync")]
        public async Task<IActionResult> SyncUserRepo([FromBody] RepoMobileItem repo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var token = await _context.MobileTokens.FirstOrDefaultAsync(a => a.Token == repo.token);
                var user = await _context.Users.FirstOrDefaultAsync(a => a.Email == repo.mail);
                if (user == null)
                {
                    user = GetUserOrCreate(token.Id);
                }

                int serverId = 0;
                switch (repo.type)
                {
                    case SyncItemType.BodyControl:
                        WeightNote item = JsonConvert.DeserializeObject<WeightNote>(repo.itemString);
                        UserWeightNote newItem = new UserWeightNote();
                        newItem.Date = item.Date;
                        newItem.Type = item.Type;
                        newItem.Weight = item.Weight;
                        newItem.DatabaseId = item.Id;
                        newItem.UserId = user.Id;
                        _context.UserWeightNotes.Add(newItem);
                        await _context.SaveChangesAsync();
                        serverId = newItem.Id;
                        break;
                    case SyncItemType.Alarm:
                        Alarm alarm = JsonConvert.DeserializeObject<Alarm>(repo.itemString, new JsonSerializerSettings
                        {
                            DateParseHandling = DateParseHandling.DateTimeOffset,
                        });
                        var userAlarms = _context.UserAlarm.Where(item => item.UserId == user.Id);
                        var updateItem = userAlarms.FirstOrDefault(alarm1 => alarm1.DatabaseId == alarm.Id);
                        if (updateItem != null)
                        {
                            updateItem.Days = alarm.Days;
                            updateItem.TimeOffset = alarm.TimeOffset;
                            updateItem.TrainingId = alarm.TrainingId;
                            updateItem.Name = alarm.Name;
                            await _context.SaveChangesAsync();
                            serverId = updateItem.Id;
                        }
                        else
                        {
                            UserAlarm userAlarm = new UserAlarm(alarm);
                            userAlarm.UserId = user.Id;
                            _context.UserAlarm.Add(userAlarm);
                            await _context.SaveChangesAsync();
                            serverId = userAlarm.Id;
                        }

                        break;
                    case SyncItemType.Exercise:
                        var exercise = JsonConvert.DeserializeObject<Entities.WebExercise>(repo.itemString);
                        UserExercise userExercise = new UserExercise(exercise);
                        userExercise.UserId = user.Id;
                        _context.UserExercises.Add(userExercise);
                        await _context.SaveChangesAsync();
                        serverId = userExercise.Id;
                        break;
                    case SyncItemType.TrainingExercise:
                        var exerciseTraining = JsonConvert.DeserializeObject<TrainingExerciseComm>(repo.itemString);
                        UserTrainingExercise userExerciseTraining = new UserTrainingExercise(exerciseTraining);
                        userExerciseTraining.UserId = user.Id;
                        _context.UserTrainingExercises.Add(userExerciseTraining);
                        await _context.SaveChangesAsync();
                        serverId = userExerciseTraining.Id;
                        break;
                    case SyncItemType.TrainingItem:
                        var training = JsonConvert.DeserializeObject<Training>(repo.itemString);
                        UserTraining userTraining = new UserTraining(training);
                        userTraining.UserId = user.Id;
                        _context.UserTrainings.Add(userTraining);
                        await _context.SaveChangesAsync();
                        serverId = userTraining.Id;
                        break;
                    case SyncItemType.LastTrainingExercise:
                        var lastTrainingExercise = JsonConvert.DeserializeObject<LastTrainingExercise>(repo.itemString);
                        UserLastTrainingExercise userLastTrainingExercise = new UserLastTrainingExercise(lastTrainingExercise);
                        userLastTrainingExercise.UserId = user.Id;
                        _context.UserLastTrainingExercises.Add(userLastTrainingExercise);
                        await _context.SaveChangesAsync();
                        serverId = userLastTrainingExercise.Id;
                        break;
                    case SyncItemType.LastTraining:
                        var lastTraining = JsonConvert.DeserializeObject<LastTraining>(repo.itemString);
                        UserLastTraining userLastTraining = new UserLastTraining(lastTraining);
                        userLastTraining.UserId = user.Id;
                        _context.UserLastTrainings.Add(userLastTraining);
                        await _context.SaveChangesAsync();
                        serverId = userLastTraining.Id;
                        break;
                    case SyncItemType.SuperSets:
                        var superSet = JsonConvert.DeserializeObject<SuperSet>(repo.itemString);
                        UserSuperSet userSuperSet = new UserSuperSet(superSet);
                        userSuperSet.UserId = user.Id;
                        _context.UserSuperSets.Add(userSuperSet);
                        await _context.SaveChangesAsync();
                        serverId = userSuperSet.Id;
                        break;
                    case SyncItemType.TrainingGroup:
                        var union = JsonConvert.DeserializeObject<TrainingUnion>(repo.itemString);
                        UserTrainingGroup userUnion = new UserTrainingGroup(union);
                        userUnion.UserId = user.Id;
                        _context.UserTrainingGroups.Add(userUnion);
                        try
                        {
                            await _context.SaveChangesAsync();
                            serverId = userUnion.Id;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            return BadRequest(new Tuple<Exception, UserTrainingGroup>(e, userUnion));
                        }
                        break;
                }
                return Ok(serverId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(new Tuple<Exception, RepoMobileItem>(e, repo));
            }
        }

        [HttpPost("repo_delete")]
        public async Task<IActionResult> DeleteUserRepo([FromQuery] int type, string mail, string token)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(a => a.Email == mail);
                if (user == null)
                {
                    var tokenItem = await _context.MobileTokens.FirstOrDefaultAsync(a => a.Token == token);
                    user = GetUserOrCreate(tokenItem.Id);
                }

                switch ((SyncItemType)type)
                {
                    case SyncItemType.BodyControl:
                        var userWeights = _context.UserWeightNotes.Where(item => item.UserId == user.Id);
                        _context.UserWeightNotes.RemoveRange(userWeights);
                        break;
                    case SyncItemType.Alarm:
                        var userAlarms = _context.UserAlarm.Where(item => item.UserId == user.Id);
                        _context.UserAlarm.RemoveRange(userAlarms);
                        break;
                    case SyncItemType.Exercise:
                        var userExercises = _context.UserExercises.Where(item => item.UserId == user.Id);
                        _context.UserExercises.RemoveRange(userExercises);
                        break;
                    case SyncItemType.TrainingExercise:
                        var userTrainingExercises = _context.UserTrainingExercises.Where(item => item.UserId == user.Id);
                        _context.UserTrainingExercises.RemoveRange(userTrainingExercises);
                        break;
                    case SyncItemType.TrainingItem:
                        var userTrainings = _context.UserTrainings.Where(item => item.UserId == user.Id);
                        _context.UserTrainings.RemoveRange(userTrainings);
                        break;
                    case SyncItemType.SuperSets:
                        var userSuperSets = _context.UserSuperSets.Where(item => item.UserId == user.Id);
                        _context.UserSuperSets.RemoveRange(userSuperSets);
                        break;
                    case SyncItemType.LastTrainingExercise:
                        var userLastTrainingExercise = _context.UserLastTrainingExercises.Where(item => item.UserId == user.Id);
                        _context.UserLastTrainingExercises.RemoveRange(userLastTrainingExercise);
                        break;
                    case SyncItemType.LastTraining:
                        var userLastTraining = _context.UserLastTrainings.Where(item => item.UserId == user.Id);
                        _context.UserLastTrainings.RemoveRange(userLastTraining);
                        break;
                    case SyncItemType.TrainingGroup:
                        var userTrainingGroups = _context.UserTrainingGroups.Where(item => item.UserId == user.Id);
                        _context.UserTrainingGroups.RemoveRange(userTrainingGroups);
                        break;
                }

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest("Exception");
            }
        }

        private User GetUserOrCreate(int tokenId)
        {
            var userToken = _context.UserTokens.FirstOrDefault(item => item.TokenId == tokenId);
            if (userToken == null)
            {
                var user = new User();
                _context.Users.Add(user);
                _context.SaveChanges();

                var newUserToken = new UserMobileToken()
                {
                    TokenId = tokenId,
                    UserId = user.Id,
                };

                _context.UserTokens.Add(newUserToken);
                _context.SaveChanges();
                return user;
            }
            else
            {
                return _context.Users.First(item => item.Id == userToken.UserId);
            }
        }

        [HttpGet("repo_sync")]
        public async Task<IActionResult> GetUserRepo([FromQuery] string mail, string token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await _context.Users.FirstOrDefaultAsync(a => a.Email == mail);
            if (user == null)
            {
                return NotFound(mail);
            }

            RepoMobileSite data = new RepoMobileSite();
            var userWeights = _context.UserWeightNotes.Where(item => item.UserId == user.Id);
            var userAlarms = _context.UserAlarm.Where(item => item.UserId == user.Id);
            var userExercises = _context.UserExercises.Where(item => item.UserId == user.Id);
            var userTrainingExercises = _context.UserTrainingExercises.Where(item => item.UserId == user.Id);
            var userTrainings = _context.UserTrainings.Where(item => item.UserId == user.Id);
            var userSuperSets = _context.UserSuperSets.Where(item => item.UserId == user.Id);
            var userLastTrainingExercise = _context.UserLastTrainingExercises.Where(item => item.UserId == user.Id);
            var userLastTraining = _context.UserLastTrainings.Where(item => item.UserId == user.Id);
            var userTrainingGroups = _context.UserTrainingGroups.Where(item => item.UserId == user.Id);

            data.Exercises = userExercises.Select(item => new Entities.WebExercise()
            {
                Id = item.DatabaseId,
                Description = JsonConvert.SerializeObject(item.Description),
                ExerciseItemName = item.ExerciseItemName,
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
            data.Alarms = userAlarms.Select(item => new Alarm()
            {
                Days = item.Days,
                Id = item.DatabaseId,
                Name = item.Name,
                TimeOffset = item.TimeOffset,
                TrainingId = item.TrainingId,
                IsActive = item.IsActive
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
                IsExpanded = item.IsExpanded
            }).ToList();
            //var dataString = JsonConvert.SerializeObject(data);
            //System.IO.File.WriteAllText(dataString,);
            //prepare data
            //string filename = user.Id + ".zip";
            //return File(dataString, "application/octet-stream", filename);

            return Ok(data);
        }

        [HttpPost("token_user")]
        public async Task<IActionResult> ConnectTokenUser([FromBody] MobileUserToken repo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var token = await _context.MobileTokens.FirstOrDefaultAsync(a => a.Token == repo.Token);
                if (token == null)
                {
                    return NotFound(repo.Token);
                }

                var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == repo.UserId);
                if (user == null)
                {
                    return NotFound();
                }

                var userExist = _context.UserTokens.FirstOrDefault(item => item.UserId == user.Id);
                if (userExist == null)
                {
                    var newUserToken = new UserMobileToken()
                    {
                        TokenId = token.Id,
                        UserId = user.Id,
                    };

                    _context.UserTokens.Add(newUserToken);
                }
                else
                {
                    userExist.TokenId = token.Id;
                    _context.Update(userExist);
                }

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }
    }
}
