using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Inventories
{
    public class ViewInventoryByIdUseCase : IViewInventoryByIdUseCase
    {
        private readonly IInventoryRepository inventoryRepository;

        public ViewInventoryByIdUseCase(IInventoryRepository inventoryRepository)
        {
            this.inventoryRepository = inventoryRepository;
        }
        public async Task<Inventory> ExecuteAsync(int inventoryId)
        {
            return await this.inventoryRepository.GetInventoryByIdAsync(inventoryId);
        }
    }
}
