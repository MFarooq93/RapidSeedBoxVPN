using AutoMapper;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using PureVPN.Infrastructure.App_Start;
using PureVPN.Service;
using PureVPN.Service.Contracts;
using PureVPN.Service.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Unity;
using Unity.Lifetime;
using UnityDependencyResolver = Unity.AspNet.WebApi.UnityDependencyResolver;

namespace PureVPN.Infrastructure
{
    public static class ComponentRegister
    {
        public static void RegisterContainer()
        {
            //UnityConfig.RegisterTypes(UnityConfig.Container);
            UnityConfig.Container.RegisterInstance(Service.Helper.MappingProfile.InitializeAutoMapper());

        }
        public static object Resolve<T>(Type service)
        {
            return UnityConfig.Container.Resolve(service);
        }
        public static T Resolve<T>()
        {
            return UnityConfig.Container.Resolve<T>();
        }
        public static object Resolve(Type service, string key)
        {
            return UnityConfig.Container.Resolve(service, key);
        }
        public static IEnumerable<object> ResolveAll(Type service)
        {
            return UnityConfig.Container.ResolveAll(service);
        }
        public static void RegisterInstance<TInterface>(TInterface instance)
        {
            UnityConfig.Container.RegisterInstance(instance);
        }

        public static void RegisterType<TFrom, TTo>() where TTo: TFrom
        {
            UnityConfig.Container.RegisterType<TFrom, TTo>();
        }
        public static void RegisterTypeSingleton<TFrom, TTo>() where TTo : TFrom
        {
            UnityConfig.Container.RegisterType<TFrom, TTo>(new SingletonLifetimeManager());
        }

        public static void BuildUp<TInterface>(TInterface instance)
        {
            UnityConfig.Container.BuildUp(instance);
        }
        public static void InitialiseWebApi(HttpConfiguration config)
        {
            config.DependencyResolver = new UnityDependencyResolver(UnityConfig.Container);
        }


        public static void InitialiseSignalr()
        {
            var container = new UnityHubActivator(UnityConfig.Container);
            GlobalHost.DependencyResolver.Register(typeof(IHubActivator), () => container);
        }


    }
}

