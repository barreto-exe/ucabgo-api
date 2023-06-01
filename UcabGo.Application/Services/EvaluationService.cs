using AutoMapper;
using AutoMapper.Configuration.Annotations;
using System.Security.Cryptography;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Evaluation.Dtos;
using UcabGo.Core.Data.Evaluation.Filter;
using UcabGo.Core.Data.Evaluation.Inputs;
using UcabGo.Core.Entities;
using UcabGo.Core.Interfaces;

namespace UcabGo.Application.Services
{
    public class EvaluationService : IEvaluationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserService userService;
        private readonly IMapper mapper;
        public EvaluationService(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.userService = userService;
            this.mapper = mapper;
        }

        public async Task<EvaluationDto> AddEvaluation(EvaluationInput input)
        {
            var user = await userService.GetByEmail(input.Email);
            var evaluatedUser = await userService.GetById(input.EvaluatedId);
            var ride = unitOfWork.RideRepository.GetAllIncluding("Passengers").FirstOrDefault(x => x.Id == input.RideId);
            if(ride == null)
            {
                throw new Exception("RIDE_NOT_FOUND");
            }

            var usersInRide = ride.Passengers.Select(x => x.User).Append(ride.Driver);
            if(!usersInRide.Any(x => x == input.EvaluatorId))
            {
                throw new Exception("EVALUATOR_NOT_FOUND");
            }
            if (!usersInRide.Any(x => x == input.EvaluatedId))
            {
                throw new Exception("EVALUATED_NOT_FOUND");
            }
            if(input.Stars < 1 || input.Stars > 5)
            {
                throw new Exception("INVALID_STARS");
            }
            if(input.EvaluatorType != "D" && input.EvaluatorType != "P")
            {
                throw new Exception("WRONG_TYPE");
            }

            var evaluations = unitOfWork.EvaluationRepository.GetAll();
            var evaluationExists = (from e in evaluations
                                    where e.Ride == input.RideId && e.Evaluator == input.EvaluatorId && e.Evaluated == input.EvaluatedId
                                    select e).Any();
            if(evaluationExists)
            {
                throw new Exception("EVALUATION_EXISTS");
            }

            var evaluation = mapper.Map<Evaluation>(input);
            evaluation.EvaluationDate = DateTime.Now;

            await unitOfWork.EvaluationRepository.Add(evaluation);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<EvaluationDto>(evaluation);
        }

        public async Task<float> GetRecievedStarsAverage(EvaluationFilter filter)
        {
            if (filter.Type != "D" && filter.Type != "P" && filter.Type != "ALL")
            {
                throw new Exception("WRONG_TYPE");
            }

            var evaluations = await GetRecievedEvaluations(filter);
            if(evaluations.Count() > 0)
            {
                return (float) evaluations.Average(x => x.Stars);
            }

            return 0;
        }

        public async Task<IEnumerable<EvaluationDto>> GetGivenEvaluations(EvaluationFilter filter)
        {
            if (filter.Type != "D" && filter.Type != "P" && filter.Type != "ALL")
            {
                throw new Exception("WRONG_TYPE");
            }

            var user = await userService.GetByEmail(filter.Email);
            var userEvaluations = 
                GetAllEvaluations()
               .Where(x => x.Evaluator == user.Id && (filter.Type == "ALL" || x.Type == filter.Type));

            return mapper.Map<IEnumerable<EvaluationDto>>(userEvaluations.ToList());
        }

        public async Task<IEnumerable<EvaluationDto>> GetRecievedEvaluations(EvaluationFilter filter)
        {
            if(filter.Type != "D" && filter.Type != "P" && filter.Type != "ALL")
            {
                throw new Exception("WRONG_TYPE");
            }

            //Change filter Type: P -> D and D -> P
            if (filter.Type == "D")
            {
                filter.Type = "P";
            }
            else if (filter.Type == "P")
            {
                filter.Type = "D";
            }

            var user = await userService.GetByEmail(filter.Email);
            var userEvaluations = 
                GetAllEvaluations()
               .Where(x => x.Evaluated == user.Id && (filter.Type == "ALL" || x.Type == filter.Type));

            return mapper.Map<IEnumerable<EvaluationDto>>(userEvaluations.ToList());
        }

        private IQueryable<Evaluation> GetAllEvaluations()
        {
            return unitOfWork
                .EvaluationRepository
                .GetAllIncluding(x => x.EvaluatorNavigation, x => x.EvaluatedNavigation, x => x.RideNavigation);
        }
    }
}
