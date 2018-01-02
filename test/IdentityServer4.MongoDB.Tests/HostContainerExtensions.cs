using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using FakeItEasy;
using Microsoft.AspNetCore.Http;

namespace IdentityServer4.MongoDB.Tests
{
    public static class HostContainerExtensions
    {
        public static void Register<TImplementer, TService>(this HostContainer hostContainer)
        {
            if (!hostContainer.Container.ComponentRegistry.IsRegistered(new TypedService(typeof(TService))))
            {
                hostContainer.Container.ComponentRegistry.Register(
                    RegistrationBuilder.ForType<TImplementer>()
                        .As<TService>()
                        .SingleInstance()
                        .CreateRegistration()
                );
            }
        }

        public static void Register<TImplementer>(this HostContainer hostContainer)
        {
            if (!hostContainer.Container.ComponentRegistry.IsRegistered(new TypedService(typeof(TImplementer))))
            {
                hostContainer.Container.ComponentRegistry.Register(
                    RegistrationBuilder.ForType<TImplementer>()
                        .AsSelf()
                        .SingleInstance()
                        .CreateRegistration()
                );
            }
        }

        public static void Register<TService>(this HostContainer hostContainer, Func<IComponentContext, IEnumerable<Parameter>, TService> @delegate)
        {
            if (!hostContainer.Container.ComponentRegistry.IsRegistered(new TypedService(typeof(TService))))
            {
                hostContainer.Container.ComponentRegistry.Register(
                    RegistrationBuilder.ForDelegate(@delegate)
                        .As<TService>()
                        .SingleInstance()
                        .CreateRegistration()
                );
            }
        }

        public static void Register<TImplementer, TService>(this HostContainer hostContainer, Func<IComponentContext, IEnumerable<Parameter>, TImplementer> @delegate)
        {
            if (!hostContainer.Container.ComponentRegistry.IsRegistered(new TypedService(typeof(TService))))
            {
                hostContainer.Container.ComponentRegistry.Register(
                    RegistrationBuilder.ForDelegate(@delegate)
                        .As<TService>()
                        .SingleInstance()
                        .CreateRegistration()
                );
            }
        }

        public static void RegisterHttpContext(this HostContainer hostContainer, string sub = null)
        {
            // register fake HttpContextAccessor
            hostContainer.Register<HttpContextAccessor, IHttpContextAccessor>((context, parameters) => A.Fake<HttpContextAccessor>());

            var httpContextAccessor = hostContainer.Container.Resolve<IHttpContextAccessor>();
            if (httpContextAccessor.HttpContext != null) return;

            // assign http context
            // note: HttpContextAccessor stores data using CallContext.LogicalSetData where different test case (fact) has different call context
            // so reassign it if current http context is null
            httpContextAccessor.HttpContext = A.Fake<HttpContext>();
            httpContextAccessor.HttpContext.User = A.Fake<ClaimsPrincipal>();
            var ipAddress = IPAddress.Parse("127.0.0.1");
            A.CallTo(() => httpContextAccessor.HttpContext.Connection.RemoteIpAddress).Returns(ipAddress);
            A.CallTo(() => httpContextAccessor.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            //A.CallTo(() => httpContextAccessor.HttpContext.RequestServices).Returns(new AutofacServiceProvider(hostContainer.Container));
            if (!string.IsNullOrEmpty(sub))
            {
                var principal = new GenericPrincipal(new ClaimsIdentity(new[] {new Claim("sub", sub)}), null);
                A.CallTo(() => httpContextAccessor.HttpContext.User).Returns(principal);
            }
        }
    }
}