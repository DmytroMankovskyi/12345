﻿using AutoMapper;
using EventsExpress.Core.DTOs;
using EventsExpress.Core.Infrastructure;
using EventsExpress.Core.IServices;
using EventsExpress.Db.Entities;
using EventsExpress.Db.IRepo;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using EventsExpress.Core.Notifications;
using Microsoft.EntityFrameworkCore;

namespace EventsExpress.Core.Services
{
    public class EventService : IEventService
    {
        private readonly IUnitOfWork Db;
        private readonly IMapper _mapper;
        private readonly IHostingEnvironment _appEnvironment;
        private readonly IPhotoService _photoService;
        private readonly IMediator _mediator;

        public EventService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IHostingEnvironment hostingEnvironment,
            IMediator mediator,
            IPhotoService photoSrv
            )
        {
            Db = unitOfWork;
            _mapper = mapper;
            _photoService = photoSrv;
            _mediator = mediator;
            _appEnvironment = hostingEnvironment;
        }
       

        public bool Exists(Guid id) => (Db.EventRepository.Get(id) != null);

        public async Task<OperationResult> AddUserToEvent(Guid userId, Guid eventId)
        {
            var ev = Db.EventRepository
               .Get(includeProperties: "Visitors")
               .FirstOrDefault(e => e.Id == eventId);

            if (ev == null)
            {
                return new OperationResult(false, "Event not found!", "eventId");
            }

            if (ev.Visitors == null)
            {
                ev.Visitors = new List<UserEvent>();
            }

            ev.Visitors.Add(new UserEvent { EventId = eventId, UserId = userId });
            await Db.SaveAsync();

            return new OperationResult(true);
        }

        public async Task<OperationResult> DeleteUserFromEvent(Guid userId, Guid eventId)
        {
            var ev = Db.EventRepository
               .Get(includeProperties: "Visitors")
               .FirstOrDefault(e => e.Id == eventId);

            if (ev == null)
            {
                return new OperationResult(false, "Event not found!", "eventId");
            }

            if (ev.Visitors == null)
            {
                ev.Visitors = new List<UserEvent>();
                return new OperationResult(false, "Visitor not found!", "visitorId");
            }

            var v = ev.Visitors.FirstOrDefault(x => x.UserId == userId);
            if(v != null)
            {
                ev.Visitors.Remove(v);
            }
            else
            { 
                return new OperationResult(false, "Visitor not found!", "visitorId");
            }
            await Db.SaveAsync();

            return new OperationResult(true);
        }


        public async Task<OperationResult> Delete(Guid id)
        {
            if (id == null)
            {
                return new OperationResult(false, "Id field is '0'", "");
            }
            Event ev = Db.EventRepository.Get(id);
            if (ev == null)
            {
                return new OperationResult(false, "Not found", "");
            }

            var result = Db.EventRepository.Delete(ev);
            await Db.SaveAsync();

            if (result == null)
            {
                return new OperationResult(true);
            }
            else {
                return new OperationResult(false, "Error!", "");
            }
        }

        public IEnumerable<EventDTO> UpcomingEvents(int? num)
        {
            var ev = Db.EventRepository.Get()
                .Where(e => e.DateTo <= DateTime.UtcNow)
                .OrderBy(e => e.DateFrom)
                .Skip(0)
                .Take((int)num)
                .AsEnumerable();
                

            return _mapper.Map<IEnumerable<EventDTO>>(ev);
        }

        public async Task<OperationResult> Create(EventDTO e)
        {
            if(e.DateFrom == new DateTime())
            {
                e.DateFrom = DateTime.Today;
            }

            if (e.DateTo == new DateTime())
            {
                e.DateTo = DateTime.Today;
            }

            Event evnt = _mapper.Map<EventDTO, Event>(e);
            evnt.Photo = await _photoService.AddPhoto(e.Photo);

            List<EventCategory> eventCategories = new List<EventCategory>();
            if (e.Categories != null)
            {
                foreach (var item in e.Categories)
                {
                    eventCategories.Add(new EventCategory
                    {
                        Event = evnt,
                        CategoryId = item.Id
                    });
                }
            }
            evnt.Categories = eventCategories;
            try
            {
                var result = Db.EventRepository.Insert(evnt);
                await Db.SaveAsync();

                e.Id = result.Id;
                await _mediator.Publish(new EventCreatedMessage(e));
                return new OperationResult(true);
            }
            catch (Exception ex)
            {
                return new OperationResult(false, ex.Message, "");
            }
        }

        public async Task<OperationResult> Edit(EventDTO e)
        {
            var evnt = Db.EventRepository.Get(includeProperties: "Photo,Categories.Category").FirstOrDefault(x => x.Id == e.Id);
            evnt.Title = e.Title;
            evnt.Description = e.Description;
            evnt.DateFrom = e.DateFrom;
            evnt.DateTo = e.DateTo;
            evnt.CityId = e.CityId;

            if (e.Photo != null && evnt.Photo != null) 
            {
                await _photoService.Delete(evnt.Photo.Id);
                evnt.Photo = await _photoService.AddPhoto(e.Photo);
            }
                                    
            List<EventCategory> eventCategories = new List<EventCategory>();

            if (e.Categories != null)
            {
                foreach (var item in e.Categories)
                {
                    eventCategories.Add(new EventCategory
                    {
                        EventId = evnt.Id,
                        CategoryId = item.Id
                    });
                }
            }
            evnt.Categories = eventCategories;

            await Db.SaveAsync();
            return new OperationResult(true);
        }

        public IEnumerable<EventDTO> Events(EventFilterViewModel model, out int Count)
        {


            IQueryable<Event> events = Db.EventRepository.Get(includeProperties: "Photo,Owner,City.Country,Categories.Category");

            if (model.KeyWord != null)
            {
                events = events.Where(x => x.Title.Contains(model.KeyWord) || x.Description.Contains(model.KeyWord));
            }
            if(model.DateFrom != new DateTime())
            {
                events = events.Where(x => x.DateFrom >= model.DateFrom);
            }

            if (model.DateTo != new DateTime())
            {
                events = events.Where(x => x.DateTo <= model.DateTo);
            }                 
            
            if(model.Categories != null)
            {
                var categories = model.Categories.Split(",");
                List<Guid> categories_id = new List<Guid>(); 
                foreach (var x in categories)
                {
                    Guid item;
                    var res = Guid.TryParse(x, out item);
                    if (res)
                    {
                        categories_id.Add(item);
                    }
                }                                                                       
                events = events.Where(x => x.Categories.Any(category => categories_id.Contains(category.CategoryId)));  
            }

            Count = events.Count();

            var IEvents = _mapper.Map<IEnumerable<EventDTO>>(events.Skip((model.Page - 1) * model.PageSize).Take(model.PageSize));
                      
            return IEvents;
           
        }

        public EventDTO EventById(Guid eventId)
        {
            var evv = Db.EventRepository.Get(includeProperties: "Photo,Owner.Photo,City.Country,Categories.Category,Visitors.User.Photo").FirstOrDefault(x => x.Id == eventId);

            var res = _mapper.Map<EventDTO>(evv);
            return res;
        }

        public IEnumerable<EventDTO> EventsByUserId(Guid userId)
        {  
            var evv = Db.EventRepository.Get().Where(e => e.OwnerId == userId);
            return _mapper.Map<IEnumerable<EventDTO>>(evv);

        }

        public EventDTO Details(Guid event_id)
        {
            var ev = Db.EventRepository.Get(includeProperties: "Title,Description,DateFrom,DateTo,City,Photo,EventCategory,Visitors").FirstOrDefault(e => e.Id == event_id);
            List<string> Categories = new List<string>();
            foreach (var x in Db.CategoryRepository.EventCategories(event_id))
            {
                Categories.Add(x.Name);
            }
            EventDTO eventDTO = _mapper.Map<Event, EventDTO>(ev);

            //eventDTO.Visitors = ev.Visitors
            //    .Select(x => x.User.Id)
            //   .ToList();

            return eventDTO;
        }
    }
}

