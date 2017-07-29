using System;

namespace Elevator
{
    public class Floor : IComparable, IComparable<Floor>, ISummonElevator
    {
        #region Operator Overrides

        public static Floor operator--(Floor f)
        {
            if (f == null)
            {
                throw new ArgumentNullException();
            }

            return new Floor(f.Number - 1, f.Name);
        }

        public static Floor operator ++(Floor f)
        {
            if (f == null)
            {
                throw new ArgumentNullException();
            }

            return new Floor(f.Number + 1, f.Name);
        }

        public static bool operator >(Floor a, Floor b)
        {
            if(a == null || b == null)
            {
                return false;
            }

            return a.CompareTo(b) > 0;
        }

        public static bool operator <(Floor a, Floor b)
        {
            if (a == null || b == null)
            {
                return false;
            }

            return a.CompareTo(b) < 0;
        }

        public static bool operator <=(Floor a, Floor b)
        {
            if (a == null || b == null)
            {
                return false;
            }

            return a.CompareTo(b) <= 0;
        }

        public static bool operator >=(Floor a, Floor b)
        {
            if (a == null || b == null)
            {
                return false;
            }

            return a.CompareTo(b) >= 0;
        }

        public static bool operator ==(Floor a, Floor b)
        {
            if (ReferenceEquals(null, a))
            {
                return ReferenceEquals(null, b);
            }

            return a.Equals(b);
        }

        public static bool operator !=(Floor a, Floor b)
        {
            if (ReferenceEquals(null, a))
            {
                return !ReferenceEquals(null, b);
            }

            return !a.Equals(b);
        }

        #endregion Operator Overrides

        private readonly string _name;
        private readonly int _number;

        //TODO: Restrict to collection of elevators serving this floor

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

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }

        public bool IsAbove(Floor other)
        {
            return this.CompareTo(other) > 0;
        }

        public bool IsBelow(Floor other)
        {
            return this.CompareTo(other) < 0;
        }

        public int SummonElevator(IRequestElevator elevatorRequestor, Direction direction)
        {
            return elevatorRequestor.RequestElevator(new FloorRequest(this, direction));
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