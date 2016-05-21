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
using System.Windows;

namespace RPX.Utils
{
    /************************************************************************
    *                                                                       *
    *                                                                       *
    *                                                                       *
    ************************************************************************/

    public class DependencyPropertyChangedEventArgs<T> : EventArgs
    {
        public DependencyPropertyChangedEventArgs(DependencyPropertyChangedEventArgs e)
        {
            NewValue = (T)e.NewValue;
            OldValue = (T)e.OldValue;
            Property = e.Property;
        }

        public T NewValue { get; private set; }
        public T OldValue { get; private set; }
        public DependencyProperty Property { get; private set; }
    }

    /************************************************************************
    *                                                                       *
    *                                                                       *
    *                                                                       *
    ************************************************************************/

    public static class DependencyProperty<T> where T : DependencyObject
    {
        public static DependencyProperty Register<TProperty>(String propertyName)
        {
            return Register(propertyName, default(TProperty), null);
        }

        public static DependencyProperty Register<TProperty>(String propertyName, TProperty defaultValue)
        {
            return Register(propertyName, defaultValue, null);
        }

        public static DependencyProperty Register<TProperty>(String propertyName, Func<T, PropertyChangedCallback<TProperty>> propertyChangedCallbackFunc)
        {
            return Register(propertyName, default(TProperty), propertyChangedCallbackFunc);
        }

        public static DependencyProperty Register<TProperty>(String propertyName, TProperty defaultValue, Func<T, PropertyChangedCallback<TProperty>> propertyChangedCallbackFunc)
        {
            return DependencyProperty.Register(propertyName, typeof(TProperty), typeof(T), new PropertyMetadata(defaultValue, ConvertCallback(propertyChangedCallbackFunc)));
        }

        private static PropertyChangedCallback ConvertCallback<TProperty>(Func<T, PropertyChangedCallback<TProperty>> propertyChangedCallbackFunc)
        {
            if (propertyChangedCallbackFunc != null)
            {
                return (sender, e) =>
                {
                    propertyChangedCallbackFunc((T)sender)?.Invoke(new DependencyPropertyChangedEventArgs<TProperty>(e));
                };
            }
            return null;
        }
    }

    public delegate void PropertyChangedCallback<TProperty>(DependencyPropertyChangedEventArgs<TProperty> e);
}
