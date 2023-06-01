using AutoMapper;
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
        private readonly IVehicleService vehicleService;
        private readonly IDestinationService destinationService;
        private readonly IRideService rideService;
        private readonly IPassengerService passengerService;
        private readonly IMapper mapper;

        public async Task<EvaluationDto> AddEvaluation(EvaluationInput input)
        {
            var user = await userService.GetByEmail(input.Email);
            var ride = unitOfWork.RideRepository.GetAllIncluding("Passengers").FirstOrDefault(x => x.Id == input.RideId);
            if(ride == null)
            {
                throw new Exception("RIDE_NOT_FOUND");
            }
            var evaluator = ride.Passengers.FirstOrDefault(x => x.User == input.EvaluatorId);
            if(evaluator == null)
            {
                throw new Exception("EVALUATOR_NOT_FOUND");
            }
            var evaluated = ride.Passengers.FirstOrDefault(x => x.User == input.EvaluatedId);
            if (evaluated == null)
            {
                throw new Exception("EVALUATED_NOT_FOUND");
            }
            if(input.Stars < 1 || input.Stars > 5)
            {
                throw new Exception("INVALID_STARS");
            }

            var evaluation = mapper.Map<Evaluation>(input);

            await unitOfWork.EvaluationRepository.Add(evaluation);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<EvaluationDto>(evaluation);
        }

        public async Task<float> GetEvaluationAverage(EvaluationFilter filter)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<EvaluationDto>> GetGivenEvaluations(EvaluationFilter filter)
        {
            var user = await userService.GetByEmail(filter.Email);

            IQueryable<Evaluation> userEvaluations;

            switch(filter.Type)
            {
                case "ALL":
                    {
                        var evaluations =
                            unitOfWork
                            .EvaluationRepository
                            .GetAllIncluding(x => x.EvaluatorNavigation, x => x.EvaluatedNavigation, x => x.RideNavigation);

                        userEvaluations =
                            from e in evaluations
                            where e.EvaluatorNavigation.Id == user.Id
                            orderby e.EvaluationDate descending
                            select e;
                        break;
                    }
                case "D":
                    {
                        //Get evaluations the user has given when he is driver
                        userEvaluations = unitOfWork
                            .RideRepository
                            .GetAllIncluding(
                                r => r.VehicleNavigation,
                                r => r.DestinationNavigation,
                                r => r.DriverNavigation,
                                r => r.Evaluations)
                            .Where(x => x.Driver == user.Id)
                            .SelectMany(x => x.Evaluations)
                            .Where(x => x.Evaluator == user.Id);
                        break;
                    }
                case "P":
                    {
                        //Get evaluations the user has given when he is passenger
                        userEvaluations = unitOfWork
                            .RideRepository
                            .GetAllIncluding(
                                r => r.VehicleNavigation,
                                r => r.DestinationNavigation,
                                r => r.DriverNavigation,
                                r => r.Passengers,
                                r => r.Evaluations)
                            .Where(x => x.Passengers.Any(p => p.User == user.Id))
                            .SelectMany(x => x.Evaluations)
                            .Where(x => x.Evaluator == user.Id);
                        break;
                    }
                default:
                    {
                        throw new Exception("WRONG_TYPE");
                    }
            }

            return mapper.Map<IEnumerable<EvaluationDto>>(userEvaluations.ToList());
        }

        public async Task<IEnumerable<EvaluationDto>> GetRecievedEvaluations(EvaluationFilter filter)
        {
            var user = await userService.GetByEmail(filter.Email);

            IQueryable<Evaluation> userEvaluations;
            switch (filter.Type)
            {
                case "ALL":
                    {
                        var evaluations =
                            unitOfWork
                            .EvaluationRepository
                            .GetAllIncluding(x => x.EvaluatorNavigation, x => x.EvaluatedNavigation, x => x.RideNavigation);

                        userEvaluations =
                            from e in evaluations
                            where e.EvaluatedNavigation.Id == user.Id
                            orderby e.EvaluationDate descending
                            select e;
                        break;
                    }
                case "D":
                    {
                        //Get evaluations the user has given when he is driver
                        userEvaluations = unitOfWork
                            .RideRepository
                            .GetAllIncluding(
                                r => r.VehicleNavigation,
                                r => r.DestinationNavigation,
                                r => r.DriverNavigation,
                                r => r.Evaluations)
                            .Where(x => x.Driver == user.Id)
                            .SelectMany(x => x.Evaluations)
                            .Where(x => x.Evaluator == user.Id);
                        break;
                    }
                case "P":
                    {
                        //Get evaluations the user has given when he is passenger
                        userEvaluations = unitOfWork
                            .RideRepository
                            .GetAllIncluding(
                                r => r.VehicleNavigation,
                                r => r.DestinationNavigation,
                                r => r.DriverNavigation,
                                r => r.Passengers,
                                r => r.Evaluations)
                            .Where(x => x.Passengers.Any(p => p.User == user.Id))
                            .SelectMany(x => x.Evaluations)
                            .Where(x => x.Evaluator == user.Id);
                        break;
                    }
                default:
                    {
                        throw new Exception("WRONG_TYPE");
                    }
            }

            return mapper.Map<IEnumerable<EvaluationDto>>(userEvaluations.ToList());
        }
    }
}
