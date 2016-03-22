﻿/*
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
    public class CollectionViewModel<TSource, TDest> : IEnumerable<TDest>, INotifyCollectionChanged
    {
        private Boolean mIsModified;
        private IEnumerable<TDest> mItems;

        private List<Func<TSource, IComparable>> mOrderBy;

        public Func<TSource, TDest> Selector { get; set; }
        public IEnumerable<TSource> SourceCollection { get; }
        public event NotifyCollectionChangedEventHandler CollectionChanged = (sender, e) => { };

        public CollectionViewModel(IEnumerable<TSource> collection, Func<TSource, TDest> selector)
        {
            mIsModified = true;

            Selector = selector;
            SourceCollection = collection;

            if (collection is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)collection).CollectionChanged += (sender, e) => NotifyCollectionChanged();
            }
        }

        public List<Func<TSource, IComparable>> OrderBy
        {
            get { return mOrderBy; }
            set
            {
                mOrderBy = value;
                NotifyCollectionChanged();
            }
        }

        public IEnumerator<TDest> GetEnumerator()
        {
            if (mIsModified || mItems == null)
            {
                var source = SourceCollection ?? Enumerable.Empty<TSource>();

                if (OrderBy?.Count > 0)
                {
                    source = source.OrderBy(OrderBy[0]);
                    for (int index = 1; index < OrderBy.Count; ++index)
                    {
                        source = (source as IOrderedEnumerable<TSource>).ThenBy(OrderBy[index]);
                    }
                }
                mItems = source.Select(Selector).ToArray();
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
