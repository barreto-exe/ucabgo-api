using UcabGo.Core.Data.Evaluation.Dtos;
using UcabGo.Core.Data.Evaluation.Filter;
using UcabGo.Core.Data.Evaluation.Inputs;

namespace UcabGo.Application.Interfaces
{
    public interface IEvaluationService
    {
        Task<EvaluationDto> AddEvaluation(EvaluationInput input);
        Task<IEnumerable<EvaluationDto>> GetRecievedEvaluations(EvaluationFilter filter);
        Task<IEnumerable<EvaluationDto>> GetGivenEvaluations(EvaluationFilter filter);
        Task<float> GetEvaluationAverage(EvaluationFilter filter);
    }
}
