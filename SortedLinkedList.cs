﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Elevator
{
    public class SortedLinkedList<T> : IEnumerable<T>, ICollection<T>
        where T : IComparable
    {
        private LinkedList<T> _list;
        private Comparison<T> _comparisonLogic;

        public SortedLinkedList(Comparison<T> comparisonLogic)
        {
            _list = new LinkedList<T>();
            _comparisonLogic = comparisonLogic;
        }

        public int Count => _list == null ? 0 : _list.Count;

        public LinkedListNode<T> First => _list != null && _list.Count > 0 ? _list.First : null;

        public bool IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

        public virtual void Add(T element)
        {
            if(null == element)
            {
                throw new ArgumentNullException("Cannot add null element to list!");
            }

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

        public void Clear()
        {
            ((ICollection<T>)_list).Clear();
        }

        public bool Contains(T element)
        {
            return _list.Contains(element);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((ICollection<T>)_list).CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_list).GetEnumerator();
        }

        public void Remove(T element)
        {
            _list.Remove(element);
        }

        public T[] ToArray()
        {
            T[] array = new T[_list.Count];
            _list.CopyTo(array, 0);

            return array;
        }

        public override string ToString()
        {            
            return $"[{string.Join(", ", _list)}]";
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)_list).GetEnumerator();
        }

        bool ICollection<T>.Remove(T item)
        {
            return ((ICollection<T>)_list).Remove(item);
        }
    }
}
