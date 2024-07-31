using AutoMapper;
using Newtonsoft.Json;
using TrainingDay.Common;
using TrainingDay.Web.Data.Support;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Server.ViewModels.Exercises;

namespace TrainingDay.Web.Server.Mapper;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<ExerciseViewModel, WebExercise>()
            .ForMember(dest => dest.Description, (src) => src.MapFrom(viewModel => JsonConvert.SerializeObject(new Description()
            {
                StartPosition = viewModel.StartingPositionDescription,
                Advice = viewModel.AdviceDescription,
                Execution = viewModel.ExecutionDescription,
            })))
            .ForMember(dest => dest.MusclesString, (src) => src.MapFrom(viewModel => ExerciseTools.ConvertFromMuscleListToString(viewModel.Muscles.ToList())))
            .ForMember(dest => dest.TagsValue, (src) => src.MapFrom(viewModel => ExerciseTools.ConvertTagListToInt(viewModel.Tags.ToList())));


        CreateMap<ContactMeModel, SupportRequest>();
    }
}