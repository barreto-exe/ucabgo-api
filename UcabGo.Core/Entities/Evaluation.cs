namespace UcabGo.Core.Entities
{
    public partial class Evaluation : BaseEntity
    {
        public int Id { get; set; }
        public int Ride { get; set; }
        public int Evaluator { get; set; }
        public int Evaluated { get; set; }
        public string Type { get; set; } = null!;
        public int Stars { get; set; }
        public DateTime EvaluationDate { get; set; }

        public virtual User EvaluatedNavigation { get; set; } = null!;
        public virtual User EvaluatorNavigation { get; set; } = null!;
        public virtual Ride RideNavigation { get; set; } = null!;
    }
}
