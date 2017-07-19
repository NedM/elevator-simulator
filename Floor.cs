using System;

namespace Elevator
{
    public class Floor : IComparable, IComparable<Floor>, IRequestElevator
    {
        private readonly string _name;
        private readonly int _number;

        public Floor(int number, string name = null)
        {            
            _number = number;
            _name = name;
        }

        public int Number { get { return _number; } }

        public string Name { get { return _name; } }

        public int CompareTo(object obj)
        {
            if(null == obj)
            {
                return 1;
            }

            Floor floor = obj as Floor;

            if(null != floor)
            {
                return CompareTo(floor);
            }

            throw new ArgumentException($"Object with type {obj.GetType().Name} is not a Floor!");
        }

        public int CompareTo(Floor other)
        {
            return null == other ? 1 : Number.CompareTo(other.Number);
        }

        public int RequestElevator(Direction direction)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Floor floor = obj as Floor;

            if(null != floor)
            {
                return Equals(floor);
            }

            return base.Equals(obj);
        }

        public bool Equals(Floor other)
        {
            return null != other && Number.Equals(other.Number);
        }

        public override string ToString()
        {
            return string.Format("Floor {0}{1}", Number, 
                (Name != null && Name.Length > 0) 
                    ? " - " + Name 
                    : string.Empty);
        }
    }
}