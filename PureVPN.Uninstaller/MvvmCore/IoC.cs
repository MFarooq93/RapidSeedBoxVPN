using System;
using System.Collections.Generic;
using System.Linq;
using Tally;

namespace MvvmCore
{
    /// <summary>
    /// Used to get and add instances using an IoC container and to inject dependencies into certain existing classes.
    /// </summary>
    public class IoC
    {
        private static Dictionary<Type, object> singletons;
        private static Dictionary<Type, object> Singletons
        {
            get
            {
                if (singletons == null)
                    singletons = new Dictionary<Type, object>();
                return singletons;
            }
        }

        private static Dictionary<Type, Type> transients;
        private static Dictionary<Type, Type> Transients
        {
            get
            {
                if (transients == null)
                    transients = new Dictionary<Type, Type>();
                return transients;
            }
        }

        private static void SetSingleton<Service, Implementation>()
        {
            try
            {
                if (Get<Service>() == null)
                    Singletons.Add(typeof(Service), Activator.CreateInstance(typeof(Implementation)));
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private static void SetSingleton(Type service, Type implementation)
        {
            try
            {
                if (Get(service) == null)
                    Singletons.Add(service, Activator.CreateInstance(implementation));
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private static void SetSingleton<Service>()
        {
            try
            {
                if (Get<Service>() == null)
                    Singletons.Add(typeof(Service), Activator.CreateInstance(typeof(Service)));
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private static void SetSingleton(Type service)
        {
            try
            {
                if (Get(service) == null)
                    Singletons.Add(service, Activator.CreateInstance(service));
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private static void SetTransient<Service, Implementation>()
        {
            try
            {
                if (GetTransient<Service>() == null)
                    Transients.Add(typeof(Service), typeof(Implementation));
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private static void SetTransient(Type service, Type implementation)
        {
            try
            {
                if (GetTransient(service) == null)
                    Transients.Add(service, implementation);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private static void SetTransient<Service>()
        {
            try
            {
                if (GetTransient<Service>() == null)
                    Transients.Add(typeof(Service), typeof(Service));
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private static void SetTransient(Type service)
        {
            try
            {
                if (GetTransient(service) == null)
                    Transients.Add(service, service);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        /// <summary>
        /// Adds the specific type object in IoC Container.
        /// </summary>
        /// <typeparam name="Service">Type of object to be stored in IoC Container.</typeparam>
        /// <param name="isSingleTon">Specifies whether the object should be a singleton or not.</param>
        public static void AddInContainer<Service>(bool isSingleTon = true)
        {
            if (isSingleTon)
                SetSingleton<Service>();
            else
                SetTransient<Service>();
        }

        /// <summary>
        /// Adds the specific type object in IoC Container.
        /// </summary>
        /// <param name="service">Type of object to be stored in IoC Container.</param>
        /// <param name="isSingleTon">Specifies whether the object should be a singleton or not.</param>
        public static void AddInContainer(Type service, bool isSingleTon = true)
        {
            if (isSingleTon)
                SetSingleton(service);
            else
                SetTransient(service);
        }

        /// <summary>
        /// Adds the specific type object in IoC Container.
        /// </summary>
        /// <typeparam name="Service">Interface or Abstract class.</typeparam>
        /// <typeparam name="Implementation">Implementation of the Service.</typeparam>
        /// <param name="isSingleTon">Specifies whether the object should be a singleton or not.</param>
        public static void AddInContainer<Service, Implementation>(bool isSingleTon = true)
        {
            if (isSingleTon)
                SetSingleton<Service, Implementation>();
            else
                SetTransient<Service, Implementation>();
        }

        /// <summary>
        /// Adds the specific type object in IoC Container.
        /// </summary>
        /// <param name="service">Interface or Abstract class.</param>
        /// <param name="implementation">Implementation of the Service.</param>
        /// <param name="isSingleTon">Specifies whether the object should be a singleton or not.</param>
        public static void AddInContainer(Type service, Type implementation, bool isSingleTon = true)
        {
            if (isSingleTon)
                SetSingleton(service, implementation);
            else
                SetTransient(service, implementation);
        }

        private static Service GetSingleton<Service>()
        {
            try { return (Service)Singletons.FirstOrDefault(x => x.Key == typeof(Service)).Value; }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return default(Service);
        }

        private static object GetSingleton(Type service)
        {
            try { return Singletons.FirstOrDefault(x => x.Key == service).Value; }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        private static Service GetTransient<Service>()
        {
            try
            {
                var service = Transients.FirstOrDefault(x => x.Key == typeof(Service)).Value;
                if (service != null)
                    return (Service)Activator.CreateInstance(service);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return default(Service);
        }

        private static object GetTransient(Type service)
        {
            try
            {
                var _service = Transients.FirstOrDefault(x => x.Key == service).Value;
                if (_service != null)
                    return Activator.CreateInstance(_service);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        /// <summary>
        /// Gets the stored object from IoC container.
        /// </summary>
        /// <typeparam name="Service">The type that was added in IoC Container.</typeparam>
        /// <returns>Instance of the stored object.</returns>
        public static Service Get<Service>()
        {
            var instance = GetSingleton<Service>();
            if (instance == null)
                instance = GetTransient<Service>();

            return instance;
        }

        /// <summary>
        /// Gets the stored object from IoC container.
        /// </summary>
        /// <param name="service">The type that was added in IoC Container.</param>
        /// <returns>Instance of the stored object.</returns>
        public static object Get(Type service)
        {
            var instance = GetSingleton(service);
            if (instance == null)
                instance = GetTransient(service);

            return instance;
        }

    }
}
