namespace Elevator
{
    public class Floor
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

        public override string ToString()
        {
            return $"Floor {Number}" + (!Name.IsNullOrEmpty ? $" - {Name}" : string.Empty);
        }
    }
}