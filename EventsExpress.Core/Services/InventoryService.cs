﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EventsExpress.Core.DTOs;
using EventsExpress.Core.Exceptions;
using EventsExpress.Core.IServices;
using EventsExpress.Db.BaseService;
using EventsExpress.Db.EF;
using EventsExpress.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventsExpress.Core.Services
{
    public class InventoryService : BaseService<Inventory>, IInventoryService
    {
        public InventoryService(
            AppDbContext context,
            IMapper mapper)
            : base(context, mapper)
        {
        }

        public async Task<Guid> AddInventar(Guid eventId, InventoryDto inventoryDTO)
        {
            var ev = Context.Events.FirstOrDefault(e => e.Id == eventId);
            if (ev == null)
            {
                throw new EventsExpressException("Event not found!");
            }

            var entity = Mapper.Map<InventoryDto, Inventory>(inventoryDTO);
            entity.EventId = eventId;
            var result = Insert(entity);
            await Context.SaveChangesAsync();

            return result.Id;
        }

        public async Task<Guid> DeleteInventar(Guid id)
        {
            var inventar = Context.Inventories.Find(id);
            if (inventar == null)
            {
                throw new EventsExpressException("Not found");
            }

            var uei = Context.UserEventInventories.Where(ue => ue.InventoryId == id).ToArray();

            if (uei != null)
            {
                Context.UserEventInventories.RemoveRange(uei);
            }

            Context.Inventories.Remove(inventar);
            await Context.SaveChangesAsync();

            return inventar.Id;
        }

        public async Task<Guid> EditInventar(InventoryDto inventoryDTO)
        {
            var entity = Context.Inventories.Find(inventoryDTO.Id);
            if (entity == null)
            {
                throw new EventsExpressException("Object not found");
            }

            entity.ItemName = inventoryDTO.ItemName;
            if (entity.NeedQuantity > inventoryDTO.NeedQuantity)
            {
                var uei = Context.UserEventInventories.Where(ue => ue.InventoryId == inventoryDTO.Id).ToArray();

                if (uei != null)
                {
                    Context.UserEventInventories.RemoveRange(uei);
                }
            }

            entity.NeedQuantity = inventoryDTO.NeedQuantity;
            entity.UnitOfMeasuringId = inventoryDTO.UnitOfMeasuring.Id;
            await Context.SaveChangesAsync();

            return entity.Id;
        }

        public IEnumerable<InventoryDto> GetInventar(Guid eventId)
        {
            if (!Context.Events.Any(x => x.Id == eventId))
            {
                return new List<InventoryDto>();
            }

            return Mapper.Map<IEnumerable<InventoryDto>>(
                Context.Inventories
                    .Include(i => i.UnitOfMeasuring)
                    .Where(i => i.EventId == eventId));
        }

        public InventoryDto GetInventarById(Guid inventoryId)
        {
            var entity = Context.Inventories.Find(inventoryId);

            if (entity == null)
            {
                return new InventoryDto();
            }

            return Mapper.Map<Inventory, InventoryDto>(entity);
        }
    }
}
