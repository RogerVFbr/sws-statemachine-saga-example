using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using SagaExample.Models.Dtos;
using SagaExample.Repositories.Interfaces;

namespace SagaExample.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(ILogger<OrderRepository> logger)
        {
            _logger = logger;
        }

        public CreateOrderResponseDto CreateOrder()
        {
            _logger.LogDebug("Creating Order ...");
            Thread.Sleep(1000);
            // throw new Exception("Order Creation went wrong.");
            return new CreateOrderResponseDto();
        }
        
        public RollbackOrderCreationResponseDto RollbackOrderCreation()
        {
            _logger.LogDebug("Reverting Order Creation ...");
            Thread.Sleep(1000);
            // throw new Exception("Order Rollback went wrong.");
            return new RollbackOrderCreationResponseDto();
        }
    }
}