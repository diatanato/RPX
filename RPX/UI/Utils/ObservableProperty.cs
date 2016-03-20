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
using System.ComponentModel;

namespace RPX.UI.Utils
{
    public class ValueChangedEventArgs<T> : EventArgs
    {
        public T Value { get; protected set; }

        public ValueChangedEventArgs(T value)
        {
            Value = value;
        }
    }

    public class ObservableProperty<T> : INotifyPropertyChanged
    {
        private T mValue;

        public T Value
        {
            get
            {
                return mValue;
            }
            set
            {
                if (ReferenceEquals(mValue, value))
                    return;

                if (ReferenceEquals(mValue, null) || !mValue.Equals(value))
                {
                    Changed(this, new ValueChangedEventArgs<T>(mValue = value));    
                }
            }
        }

        public event EventHandler<ValueChangedEventArgs<T>> Changed;
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public ObservableProperty()
        {
            Changed += (sender, e) => PropertyChanged(this, new PropertyChangedEventArgs(nameof(Value)));
        }

        public ObservableProperty(T value) : this()
        {
            mValue = value;
        }

        public void NotifyPropertyChanged()
        {
            Changed(this, new ValueChangedEventArgs<T>(mValue));
        }
    }
}

