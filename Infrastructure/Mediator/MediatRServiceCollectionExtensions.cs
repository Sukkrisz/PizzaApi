﻿using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Mediator
{
    public static class MediatRServiceCollectionExtensions
    {
        public static IServiceCollection AddMediatRToAssemblies(this IServiceCollection services)
        {
            var projectsToAddTo = new string[] { "Data", "PizzaApi" };
            var assembliesToAddTo = AppDomain.CurrentDomain.GetAssemblies().Where(a =>
                                                                                    !a.IsDynamic &&
                                                                                    projectsToAddTo.Contains(a.FullName));

            foreach (var assembly in assembliesToAddTo)
            {
                services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
            }

            return services;
        }
    }
}
