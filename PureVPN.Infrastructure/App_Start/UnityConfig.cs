using Microsoft.AspNet.SignalR.Hubs;
using NotificationCenter.Builders;
using NotificationCenter.Infrastructure.Builders;
using NotificationCenter.Infrastructure.Contracts;
using NotificationCenter.Infrastructure.Models;
using PureVPN.Core.Interfaces;
using PureVPN.Descifrar;
using PureVPN.Descifrar.Infrastructure;
using PureVPN.Infrastructure.App_Start;
using PureVPN.Infrastructure.UnitTestConfig;
using PureVPN.Realtime.Repository;
using PureVPN.Realtime.Repository.Infrastructure;
using PureVPN.Realtime.Repository.Infrastructure.Models;
using PureVPN.Service;
using PureVPN.Service.Contracts;
using PureVPN.Service.Directors;
using PureVPN.Service.Helper;
using System;

using Unity;

namespace PureVPN.Infrastructure
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your type's mappings here.
            // container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<IAtomService, AtomService>();
            container.RegisterType<IPureAtomConfigurationService, PureAtomConfigurationService>();
            container.RegisterType<IPureAtomManagerService, PureAtomManagerService>();
            container.RegisterType<IPureAtomBPCManagerService, PureAtomBPCManagerService>();
            container.RegisterType<ISentryService, SentryService>();
            container.RegisterType<IMixpanelService, MixpanelService>();
            container.RegisterType<IDialerToServerService, DialerToServerService>();
            container.RegisterType<IAuthTokenService, Auth2TokenService>();
            container.RegisterType<IClientAppAccessTokenService, AuthTokenService>();
            container.RegisterType<IMigrationService, MigrationService>();
            container.RegisterType<IPortForwardingDetailsService, PortForwardingDetailsService>();
            container.RegisterType<IUserProfileProvider, UserProfileProvider>();
            container.RegisterType<ICachingService, CachingService>(new Unity.Lifetime.SingletonLifetimeManager());
            container.RegisterType<IDashboardNotificationsService, DashboardNotificationsService>(new Unity.Lifetime.SingletonLifetimeManager());
            container.RegisterType<ISymmetricManager, SymmetricManager>(new Unity.Lifetime.SingletonLifetimeManager());
            container.RegisterType<IBaseURLProvider, BaseURLProvider>(new Unity.Lifetime.SingletonLifetimeManager());
            container.RegisterType<IOAuth2BaseURLProvider, BaseURLProvider>(new Unity.Lifetime.SingletonLifetimeManager());
            container.RegisterType<ITelemetryService, AuthenticationTelemetryService>();
            container.RegisterType<INotificationsService, NotificationsService>();
            container.RegisterType<INotificationInstanceDirector, NCDirector>();
            container.RegisterType<INCManagerBuilder, NCManagerBuilder>();
            container.RegisterType<INotificationCenterErrorLogger, NotificationCenterErrorLogger>();
            container.RegisterType<IPureLinkParser, PureLinkParserService>();
            container.RegisterType<ITroubleshootErrorService, TroubleshootErrorService>();
            container.RegisterType<IDislikeReasonsService, DislikeReasonsService>(new Unity.Lifetime.SingletonLifetimeManager());

            // Registrations for realtime data
            container.RegisterType<IConnectionStringProvider, RealtimeConnectionStringProvider>();
            container.RegisterType(typeof(IRealtimeRepository), typeof(RealtimeRepository), new Unity.Lifetime.SingletonLifetimeManager());

            container.RegisterType<IHubActivator, UnityHubActivator>(new Unity.Lifetime.ContainerControlledLifetimeManager());
            container.RegisterType<IServiceLocator, CustomUnityServiceLocator>();
            // container.RegisterType<IHubActivator, UnityHubActivator>(new Unity.Lifetime.ContainerControlledLifetimeManager());
        }
    }
}