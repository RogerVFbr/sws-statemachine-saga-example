using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SagaExample.Controllers;
using SagaExample.Models;
using SagaExample.Repositories;
using SagaExample.Repositories.Interfaces;
using SagaExample.Services;
using SagaExample.Services.Interfaces;
using SWS.StateMachine.Configuration;

namespace SagaExample
{
    public static class ConfigurationModule
    {
        private static readonly ServiceProvider ServiceProvider;
        
        static ConfigurationModule()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
        
        public static T GetService<T>() => ServiceProvider.GetService<T>();
        
        private static void ConfigureServices(IServiceCollection services)
        {
            // Register your regular application dependencies
            services.AddSingleton<ICheckoutController, CheckoutController>();
            services.AddSingleton<ICheckoutService, CheckoutService>();
            services.AddSingleton<IOrderRepository, OrderRepository>();
            services.AddSingleton<IPaymentRepository, PaymentRepository>();
            services.AddSingleton<IInventoryRepository, InventoryRepository>();

            services.AddLogging(builder => builder
                .AddSimpleConsole(opt => opt.SingleLine = true)
                .AddDebug()
                .SetMinimumLevel(LogLevel.Debug));

            // Register SWS StateMachine state signatures
            services.AddSwsStateMachineSignature()
                .Task<CheckoutState, CheckoutState>();
        }
    }
}