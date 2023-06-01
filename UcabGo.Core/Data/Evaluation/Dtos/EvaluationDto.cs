using UcabGo.Core.Data.Ride.Dtos;
using UcabGo.Core.Data.User.Dto;

namespace UcabGo.Core.Data.Evaluation.Dtos
{
    public class EvaluationDto
    {
        public int Id { get; set; }
        public RideDto Ride { get; set; }
        public UserDto Evaluated { get; set; }
        public UserDto Evaluator { get; set; }
        public int Stars { get; set; }
        public DateTime EvaluationDate { get; set; }
    }
}
