using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Vahti.Mobile.Forms.Models
{
    /// <summary>
    /// Represents a location containing measurements shown in UI
    /// </summary>
    public class Location : List<Measurement>, INotifyPropertyChanged
    {
        private int _order;
        private string _name;
        private DateTime _timeStamp;
        private int _updateInterval;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        public DateTime Timestamp
        {
            get
            {
                return _timeStamp;
            }
            set
            {
                _timeStamp = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Timestamp)));
            }
        }
    
        public int UpdateInterval
        {
            get
            {
                return _updateInterval;
            }
            set
            {
                _updateInterval = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UpdateInterval)));
            }
        }

        public int Order
        {
            get
            {
                return _order;
            }
            set
            {
                _order = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Order)));
            }
        }

        public Location(string name, DateTime timestamp, int updateInterval, List<Measurement> measurements, int order) : base(measurements)
        {
            _name = name;
            _timeStamp = timestamp;
            _updateInterval = updateInterval;
            _order = order;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
