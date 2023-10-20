using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Inventories
{
    public class AddInventoryUseCase : IAddInventoryUseCase
    {
        private readonly IInventoryRepository inventoryRepository;

        public AddInventoryUseCase(IInventoryRepository inventoryRepository)
        {
            this.inventoryRepository = inventoryRepository;
        }
        public async Task ExecuteAsync(Inventory inventory)
        {
            await this.inventoryRepository.AddInventoryAsync(inventory);
        }
    }
}
