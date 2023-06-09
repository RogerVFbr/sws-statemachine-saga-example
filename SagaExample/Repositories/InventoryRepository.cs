using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using SagaExample.Exceptions;
using SagaExample.Models.Dtos;
using SagaExample.Repositories.Interfaces;

namespace SagaExample.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ILogger<InventoryRepository> _logger;
        
        public InventoryRepository(
            ILogger<InventoryRepository> logger)
        {
            _logger = logger;
        }

        public UpdateInventoryResponseDto UpdateInventory()
        {
            _logger.LogDebug("Updating Inventory ...");
            Thread.Sleep(1000);
            throw new Exception("Inventory Update went wrong.");
            return new UpdateInventoryResponseDto();
        }
        
        public RollbackInventoryResponseDto RollbackInventoryUpdate()
        {
            _logger.LogDebug("Reverting Inventory Update ...");
            Thread.Sleep(1000);
            // throw new Exception("Inventory Update Rollback went wrong.");
            return new RollbackInventoryResponseDto();
        }
    }
}