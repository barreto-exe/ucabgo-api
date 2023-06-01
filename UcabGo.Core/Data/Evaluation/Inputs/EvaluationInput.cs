using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Data.Evaluation.Inputs
{
    public class EvaluationInput : BaseRequest
    {
        [Required]
        public int RideId { get; set; }
        [Required]
        public int EvaluatorId { get; set; }
        [Required]
        public int EvaluatedId { get; set; }
        [Required]
        public string EvaluatorType { get; set; }
        [Required]
        public int Stars { get; set; }
    }
}
