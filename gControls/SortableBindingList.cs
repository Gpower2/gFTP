using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Collections;

namespace gpower2.gControls
{
    public class PropertyComparer<T> : IComparer<T>
    {
        private PropertyInfo _property;
        private ListSortDirection _sortDirection;

        public PropertyComparer(string sortProperty, ListSortDirection sortDirection)
        {
            _property = typeof(T).GetProperty(sortProperty);
            this._sortDirection = sortDirection;
        }

        public int Compare(T x, T y)
        {
            object valueX = _property.GetValue(x, null);
            object valueY = _property.GetValue(y, null);

            if (_sortDirection == ListSortDirection.Ascending)
            {
                return Comparer.Default.Compare(valueX, valueY);
            }
            else
            {
                return Comparer.Default.Compare(valueY, valueX);
            }
        }
    }

    public interface ISortableBindingList { }

    public class SortableBindingList<T> : BindingList<T>, ISortableBindingList
    {
        public SortableBindingList(IList<T> arg) : base(arg) { }

        public SortableBindingList() : base() { }

        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        private PropertyDescriptor _SortPropertyCore = null;

        protected override PropertyDescriptor SortPropertyCore
        {
            get
            {
                return _SortPropertyCore;
            }
        }

        private ListSortDirection _SortDirectionCore = ListSortDirection.Ascending;

        protected override ListSortDirection SortDirectionCore
        {
            get
            {
                return _SortDirectionCore;
            }
        }

        private bool _isSorted;

        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            // Apply and set the sort, if items to sort
            if (this.Items != null)
            {
                PropertyComparer<T> pc = new PropertyComparer<T>(property.Name, direction);
                (this.Items as List<T>).Sort(pc);
                _isSorted = true;
                _SortPropertyCore = property;
                _SortDirectionCore = direction;
            }
            else
            {
                _isSorted = false;
                _SortPropertyCore = null;
                _SortDirectionCore= ListSortDirection.Ascending;
            }
            
            // Let bound controls know they should refresh their views
            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override bool IsSortedCore
        {
            get { return _isSorted; }
        }

        protected override void RemoveSortCore()
        {
            _isSorted = false;
        }
    }
}
