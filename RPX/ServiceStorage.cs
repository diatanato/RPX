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
using System.Collections.Generic;

namespace RPX
{
    /************************************************************************
    *                                                                       *
    *                                                                       *
    *                                                                       *
    ************************************************************************/

    static class ServiceStorage
    {
        private static readonly Dictionary<Type, ServiceInfo> services = new Dictionary<Type, ServiceInfo>();

        public static void Register<TInterface, TImplementation>() where TImplementation : TInterface
        {
            Register<TInterface, TImplementation>(false);
        }

        private static void Register<TInterface, TImplementation>(bool isSingleton) where TImplementation : TInterface
        {
            services.Add(typeof(TInterface), new ServiceInfo(typeof(TImplementation), isSingleton));
        }

        public static void RegisterSingleton<TInterface, TImplementation>() where TImplementation : TInterface
        {
            Register<TInterface, TImplementation>(true);
        }

        public static void RegisterSingleton<TInterface, TImplementation>(TImplementation implementation) where TImplementation : TInterface
        {
            services.Add(typeof(TInterface), new ServiceInfo(implementation));
        }

        public static TInterface Resolve<TInterface>()
        {
            return (TInterface)services[typeof(TInterface)].Implementation;
        }
    }

    /************************************************************************
    *                                                                       *
    *                                                                       *
    *                                                                       *
    ************************************************************************/

    class ServiceInfo
    {
        private object mImplementation;

        private readonly Type mType;
        private readonly bool mIsSingleton;

        public ServiceInfo(object implementation) : this(implementation.GetType(), true)
        {
            mImplementation = implementation;
        }

        public ServiceInfo(Type type, bool isSingleton)
        {
            mType = type;
            mIsSingleton = isSingleton;
        }

        public object Implementation
        {
            get
            {
                if (mIsSingleton)
                {
                    return mImplementation ?? (mImplementation = CreateInstance(mType));
                }
                return CreateInstance(mType);
            }
        }

        private static object CreateInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}
