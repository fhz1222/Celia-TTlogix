namespace Application.UseCases;

public class DtoFilterDateTimeRange
{
        private DateTime? _From { get; set; }
        private DateTime? _To { get; set; }
        public DateTime? From { get { return _From ?? DateTime.Now.AddYears(-250); } set { _From = value; } }
        public DateTime? To { get { return _To?.AddDays(1) ?? DateTime.Now.AddYears(250); } set { _To = value; } }
}