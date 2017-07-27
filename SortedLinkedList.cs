using log4net;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Elevator
{
    public class SortedLinkedList<T> : IEnumerable<T>, ICollection<T>
        where T : IComparable
    {
        private object sync_lock = new object();

        private LinkedList<T> _list;
        private Comparison<T> _comparisonLogic;
        private ILog _log;

        public SortedLinkedList(Comparison<T> comparisonLogic)
        {
            if(null == comparisonLogic)
            {
                throw new ArgumentNullException("comparisonLogic", "Comparison logic cannot be null!");
            }

            _list = new LinkedList<T>();
            _comparisonLogic = comparisonLogic;
            _log = LogManager.GetLogger(typeof(ElevatorSystem));
        }

        public int Count
        {
            get
            {
                lock (sync_lock)
                {
                    return _list == null ? 0 : _list.Count;
                }
            }
        }

        public LinkedListNode<T> First
        {
            get
            {
                lock (sync_lock)
                {
                    return _list != null && _list.Count > 0 ? _list.First : null;
                }
            }
        }

        public bool IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

        public virtual void Add(T element)
        {
            if(null == element)
            {
                throw new ArgumentNullException("Cannot add null element to list!");
            }

            lock (sync_lock)
            {
                if (_list.First == null)
                {
                    _list.AddFirst(element);
                    _log.Debug($"Added first element: {element} to linked list!");

                    return;
                }

                LinkedListNode<T> node = _list.First;

                while (true)
                {
                    if (_comparisonLogic.Invoke(element, node.Value) < 0)
                    {
                        LinkedListNode<T> newNode = new LinkedListNode<T>(element);
                        _list.AddBefore(node, newNode);

                        _log.Debug($"Added {element} " + 
                            (newNode.Previous == null 
                                ? $"as the first element just before {node.Value}" 
                                : $"between {newNode.Previous.Value} and {node.Value}."));

                        break;
                    }

                    if (node.Next == null)
                    {
                        LinkedListNode<T> newNode = new LinkedListNode<T>(element);
                        _list.AddAfter(node, newNode);
                        _log.Debug($"Added {element} to end of the linked list just after {node.Value}.");

                        break;
                    }

                    node = node.Next;
                }
            }
        }

        public void Clear()
        {
            lock (sync_lock)
            {
                ((ICollection<T>)_list).Clear();
            }
        }

        public bool Contains(T element)
        {
            lock (sync_lock)
            {
                return _list.Contains(element);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock(sync_lock)
            {
                ((ICollection<T>)_list).CopyTo(array, arrayIndex);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (sync_lock)
            {
                return ((IEnumerable<T>)_list).GetEnumerator();
            }
        }

        public void Remove(T element)
        {
            lock (sync_lock)
            {
                _list.Remove(element);
            }
        }

        public T[] ToArray()
        {
            lock (sync_lock)
            {
                T[] array = new T[_list.Count];
                _list.CopyTo(array, 0);

                return array;
            }
        }

        public override string ToString()
        {
            lock (sync_lock)
            {
                return $"[{string.Join(", ", _list)}]";
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (sync_lock)
            {
                return ((IEnumerable<T>)_list).GetEnumerator();
            }
        }

        bool ICollection<T>.Remove(T item)
        {
            lock (sync_lock)
            {
                return ((ICollection<T>)_list).Remove(item);
            }
        }
    }
}
