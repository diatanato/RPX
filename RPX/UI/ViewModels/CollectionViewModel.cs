/*
===========================================================================

  Copyright (c) 2016 diatanato

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program.  If not, see http://www.gnu.org/licenses/

  This file is part of RPX source code.

===========================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace RPX.UI.ViewModels
{
    public class CollectionViewModel<T> : IEnumerable<T>, INotifyCollectionChanged
    {
        private Boolean mIsModified;
        private IEnumerable<T> mItems;

        public IEnumerable<T> SourceCollection { get; private set; }
        public event NotifyCollectionChangedEventHandler CollectionChanged = (sender, e) => { };

        public CollectionViewModel(IEnumerable<T> collection)
        {
            mIsModified = true;
            SourceCollection = collection;

            if (collection is INotifyCollectionChanged)
            {
                (collection as INotifyCollectionChanged).CollectionChanged += (sender, e) => NotifyCollectionChanged();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (mIsModified || mItems == null)
            {
                mItems = SourceCollection ?? Enumerable.Empty<T>();
                mIsModified = false;
            }
            return mItems.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void NotifyCollectionChanged()
        {
            mIsModified = true;
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
