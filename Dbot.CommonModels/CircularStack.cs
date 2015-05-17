using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Dbot.CommonModels {
  /// <summary>
  /// This is a fixed size circular buffer that adds elements to the beginning of the collection, and then moves the pointer to the next "first" element.
  /// This gives fast insertions to the beginning of the collection, and gives us automatic removals when the collection as the pointer overwrites 
  /// the oldest element. 
  /// 
  /// Also, check out how Add() doesn't technically implement IEnumerable<T>, and yet it is used when we initialize a collection. Renaming it to "xAdd()" 
  /// will break its ability to initialize collections.
  /// </summary>
  public class CircularStack<T> : IEnumerable<T> {
    private readonly T[] _backingArray;
    private int _index;

    public CircularStack(int size) {
      _backingArray = new T[size];
    }

    public IEnumerator<T> GetEnumerator() {
      return _backingArray.Skip(_index).Concat(_backingArray.Take(_index)).Reverse().GetEnumerator();
      var listIndexes = new List<int>();
      for (var i = _backingArray.Length - 1; i >= 0; i--) {
        var temp = i + _index;
        if (temp >= _backingArray.Length) {
          temp = i + _index - _backingArray.Length;
        }
        listIndexes.Add(temp);
      }

      return listIndexes.Select(o => _backingArray[o]).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    public void Add(T item) {
      _backingArray[_index] = item;
      _index++;

      if (_index >= _backingArray.Length) {
        _index = 0;
      }
    }
  }
}
