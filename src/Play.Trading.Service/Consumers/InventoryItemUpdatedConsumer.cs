using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Play.Common;
using Play.Inventory.Contracts;
using Play.Trading.Service.Entities;

namespace Play.Trading.Service.Consumers
{
    public class InventoryItemUpdatedConsumer : IConsumer<InventoryItemUpdated>
    {
        private readonly IRepository<InventoryItem> _repository;

        public InventoryItemUpdatedConsumer(IRepository<InventoryItem> repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<InventoryItemUpdated> context)
        {
            var message = context.Message;

            var inventoryItem = await _repository.GetAsync(item => item.CatalogItemId == message.CatalogItemId
                                                            && item.UserId == message.UserId);

            if (inventoryItem is null)
            {
                inventoryItem = new InventoryItem
                {
                    UserId = message.UserId,
                    CatalogItemId = message.CatalogItemId,
                    Quantity = message.NewTotalQuantity
                };

                await _repository.CreateAsync(inventoryItem);

            }
            else
            {
                inventoryItem.Quantity = message.NewTotalQuantity;
                await _repository.UpdateAsync(inventoryItem);
            }

        }
    }
}