using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Trading.Service.Entities;

namespace Play.Trading.Service.Controllers
{
    [ApiController]
    [Route("store")]
    [Authorize]
    public class StoreController : ControllerBase
    {
        private readonly IRepository<CatalogItem> _catalogRepository;
        private readonly IRepository<InventoryItem> _inventoryRepository;

        private readonly IRepository<ApplicationUser> _usersRepository;

        public StoreController(IRepository<ApplicationUser> usersRepository,
                IRepository<InventoryItem> inventoryRepository,
                IRepository<CatalogItem> catalogRepository)
        {
            _usersRepository = usersRepository;
            _inventoryRepository = inventoryRepository;
            _catalogRepository = catalogRepository;
        }

        [HttpGet]
        public async Task<ActionResult<StoreDto>> GetAsync()
        {
            var userId = User.FindFirstValue("sub");

            var inventoryItems = await _inventoryRepository.GetAllAsync(item => item.UserId.ToString() == userId);

            var catalogItems = await _catalogRepository.GetAllAsync();

            var user = await _usersRepository.GetAsync(Guid.Parse(userId));

            var storeDto = new StoreDto(
                catalogItems.Select(catalogItem =>
                new StoreItemDto(
                    catalogItem.Id,
                    catalogItem.Name,
                    catalogItem.Description,
                    catalogItem.Price,
                    inventoryItems.FirstOrDefault(i => i.CatalogItemId == catalogItem.Id)?.Quantity ?? 0
                ))
            , user?.Gil ?? 0);

            return Ok(storeDto);
        }
    }
}