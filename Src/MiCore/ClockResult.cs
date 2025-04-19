using System;

namespace MiCore
{
    public class ClockResult
    {
        private DateTime _displayValue;
        public DateTime DisplayValue
        {
            get { return _displayValue; }
            set { _displayValue = value; }
        }

        public ClockResult(DateTime value)
        {
            _displayValue = value;
        }
    }
}

