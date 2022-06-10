using Dreamrosia.Koin.Application.Configurations;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dreamrosia.Koin.Server.Extensions
{
    internal static class MvcBuilderExtensions
    {
        internal static IMvcBuilder AddValidators(this IMvcBuilder builder)
        {
            builder.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<ServerConfiguration>());
            return builder;
        }
    }
}