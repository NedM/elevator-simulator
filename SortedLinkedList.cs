using System;
using System.Collections.Generic;

namespace Elevator
{
    public class SortedLinkedList<T> where T : IComparable
    {
        private LinkedList<T> _list;
        private Comparison<T> _comparisonLogic;

        public SortedLinkedList(Comparison<T> comparisonLogic)
        {
            _list = new LinkedList<T>();
            _comparisonLogic = comparisonLogic;
        }

        public T First => _list != null && _list.Count > 0 ? _list.First.Value : default(T);

        public int Count => _list == null ? 0 : _list.Count;

        public virtual void Add(T element)
        {
            if (_list.First == null)
            {
                _list.AddFirst(element);
                return;
            }

            LinkedListNode<T> node = _list.First;

            while (true)
            {
                if (_comparisonLogic.Invoke(element, node.Value) < 0)
                {
                    _list.AddBefore(node, new LinkedListNode<T>(element));
                    break;
                }

                if (node.Next == null)
                {
                    _list.AddAfter(node, new LinkedListNode<T>(element));
                    break;
                }

                node = node.Next;
            }
        }

        public void Remove(T element)
        {
            _list.Remove(element);
        }
    }
}
