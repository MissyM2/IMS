using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Plugins.InMemory
{
    public class InventoryTransactionRepository : IInventoryTransactionRepository
    {
        private readonly IInventoryRepository inventoryRepository;
        public List<InventoryTransaction> _inventoryTransactions = new List<InventoryTransaction>();

        public InventoryTransactionRepository(IInventoryRepository inventoryRepository)
        {
            this.inventoryRepository = inventoryRepository;
        }

        public async Task<IEnumerable<InventoryTransaction>> GetInventoryTransactionsAsync(string inventoryName, DateTime? dateFrom, DateTime? dateTo, InventoryTransactionType? transactionType)
        {
            // the string.empty means I will get all the inventories
            //  you can do this here, in the in-memory datastore because there will be no performance issue
            // later, with the db (here we used sqldb), there would be a performance issue, so we do it differently
            var inventories = (await inventoryRepository.GetInventoriesByNameAsync(string.Empty)).ToList();

            // using linq to query in-memory datastore
            // query below similar to sql but, with linc you do not need a table, you can use it to query a list or in-memory datastore
            /* select *
             * from inventorytransactions it
             * join inventories inv on it.inventoryid = inv.inventoryid ...
             */

            var query = from it in this._inventoryTransactions
                        join inv in inventories on it.InventoryId equals inv.InventoryId
                        where
                            (string.IsNullOrWhiteSpace(inventoryName) || inv.InventoryName.ToLower().IndexOf(inventoryName.ToLower()) >= 0)
                            &&
                            (!dateFrom.HasValue || it.TransactionDate >= dateFrom.Value.Date) &&
                            (!dateTo.HasValue || it.TransactionDate <= dateTo.Value.Date) &&
                            (!transactionType.HasValue || it.ActivityType == transactionType)
                        select new InventoryTransaction
                        {
                            Inventory = inv,
                            InventoryTransactionId = it.InventoryTransactionId,
                            PONumber = it.PONumber,
                            ProductionNumber = it.ProductionNumber,
                            InventoryId = it.InventoryId,
                            QauntityBefore = it.QauntityBefore,
                            ActivityType = it.ActivityType,
                            QauntityAfter = it.QauntityAfter,
                            TransactionDate = it.TransactionDate,
                            DoneBy = it.DoneBy,
                            UnitPrice = it.UnitPrice
                        };

            return query;
        }

        public Task ProduceAsync(string productionNumber, Inventory inventory, int quantityToConsume, string doneBy, double price)
        {
            this._inventoryTransactions.Add(new InventoryTransaction
            {
                ProductionNumber = productionNumber,
                InventoryId = inventory.InventoryId,
                QauntityBefore = inventory.Quantity,
                ActivityType = InventoryTransactionType.ProduceProduct,
                QauntityAfter = inventory.Quantity - quantityToConsume,
                TransactionDate = DateTime.Now,
                DoneBy = doneBy,
                UnitPrice = price
            });

            return Task.CompletedTask;
        }

        public Task PurchaseAsync(string poNumber, Inventory inventory, int quantity, string doneBy, double price)
        {
            this._inventoryTransactions.Add(new InventoryTransaction
            {
                PONumber = poNumber,
                InventoryId = inventory.InventoryId,
                QauntityBefore = inventory.Quantity,
                ActivityType = InventoryTransactionType.PurchaseInventory,
                QauntityAfter = inventory.Quantity + quantity,
                TransactionDate = DateTime.Now,
                DoneBy = doneBy,
                UnitPrice = price
            });

            return Task.CompletedTask;
        }
    }
}

