using SagaExample.Models.Dtos;

namespace SagaExample.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        UpdateInventoryResponseDto UpdateInventory();
        RollbackInventoryResponseDto RollbackInventoryUpdate();
    }
}