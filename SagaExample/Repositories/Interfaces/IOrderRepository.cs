using SagaExample.Models.Dtos;

namespace SagaExample.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        CreateOrderResponseDto CreateOrder();
        RollbackOrderCreationResponseDto RollbackOrderCreation();
    }
}