namespace Application.UseCases;

public class DtoFilterIntRange
{
        private int? _From { get; set; }
        private int? _To { get; set; }
        public int From { get { return _From ?? int.MinValue; } set { _From = value; } }
        public int To { get { return _To ?? int.MaxValue; } set { _To = value; } }
        public bool IsEmpty => _From == null && _To == null;
    public bool Check(int value)
           => IsEmpty || (_From == null || value >= _From)  && (_To == null || value <= _To);
}
