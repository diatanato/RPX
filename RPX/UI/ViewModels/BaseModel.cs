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
using System.Linq.Expressions;
using System.Windows;

namespace RPX.UI.ViewModels
{
    using Interfaces;
    using Model;

    /************************************************************************
    *                                                                       *
    *  Базовый класс для моделей                                            *
    *                                                                       *
    ************************************************************************/

    public abstract class BaseModel : INotifyPropertyChanged 
    {
        protected static IState Model { get; private set; }

        static BaseModel()
        {
            Model = IsInDessignMode ? new DevState() : ServiceStorage.Resolve<IState>();
        }

        static bool IsInDessignMode
        {
            get { return (bool)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement)).Metadata.DefaultValue; }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void NotifyPropertyChanged<T>(Expression<Func<T>> propertyExpr)
        {
            MemberExpression memberExpression = propertyExpr.Body as MemberExpression;
            if (memberExpression == null)
            {
                return;
            }
            PropertyChanged(this, new PropertyChangedEventArgs(memberExpression.Member.Name));
        }
        #endregion
    }
}
