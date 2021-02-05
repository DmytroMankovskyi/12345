﻿using System;
using EventsExpress.Core.DTOs;
using EventsExpress.Core.Services;
using EventsExpress.Db.Entities;
using EventsExpress.Test.ServiceTests.TestClasses.Location;
using Moq;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace EventsExpress.Test.ServiceTests
{
    [TestFixture]
    internal class LocationServiceTest : TestInitializer
    {
        private LocationService service;
        private EventLocation locationMap;
        private EventLocation locationOnline;

        private EventLocation FromDtoToEf(LocationDto locationDto)
        {
            return new EventLocation
            {
                Id = locationDto.Id,
                Point = locationDto.Point,
                OnlineMeeting = locationDto.OnlineMeeting,
                Type = locationDto.Type,
            };
        }

        [SetUp]
        protected override void Initialize()
        {
            base.Initialize();
            service = new LocationService(Context, MockMapper.Object);
            locationMap = FromDtoToEf(CreatingExistingLocation.LocationDTOMap);
            locationOnline = FromDtoToEf(CreatingExistingLocation.LocationDTOOnline);

            Context.EventLocations.Add(locationMap);
            Context.SaveChanges();
            Context.EventLocations.Add(locationOnline);
            Context.SaveChanges();

            MockMapper.Setup(u => u.Map<LocationDto>(It.IsAny<EventLocation>()))
                .Returns((EventLocation el) => el == null ?
                null :
                new LocationDto
                {
                    Id = el.Id,
                    Point = el.Point,
                    OnlineMeeting = el.OnlineMeeting,
                    Type = el.Type,
                });

            MockMapper.Setup(u => u.Map<LocationDto, EventLocation>(It.IsAny<LocationDto>()))
                .Returns((LocationDto el) => el == null ?
                null :
                new EventLocation
                {
                    Id = el.Id,
                    Point = el.Point,
                    OnlineMeeting = el.OnlineMeeting,
                    Type = el.Type,
                });
        }

        [TestCaseSource(typeof(CreatingNotExistingLocation))]
        [Category("Create")]
        public void Create_newLocation_IdEquals(LocationDto locationDto)
        {
            var result = service.Create(locationDto);

            Assert.That(Guid.Equals(result.Result, locationDto.Id), Is.True);
        }

        [TestCaseSource(typeof(CreatingNotExistingLocation))]
        [Category("Create")]
        public void Create_newLocation_DoesNotThrowAsync(LocationDto locationDto)
        {
            Assert.DoesNotThrowAsync(
                async () => await service.Create(locationDto));
        }

        [TestCaseSource(typeof(CreatingExistingLocation))]
        [Category("Create")]
        public void Create_ExistingLocation_Failed(LocationDto locationDto)
        {
            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await service.Create(locationDto));
        }

        [Test]
        [Category("Location by Point")]
        public void LocationByPoint_ExistingPoint_ReturnLocationDTO()
        {
            Point point = new Point(locationMap.Point.X, locationMap.Point.Y);
            Guid id = locationMap.Id;

            var actual = service.LocationByPoint(point);

            Assert.That(Is.Equals(actual.Id, id), Is.True);
        }

        [Test]
        [Category("Location by Point")]
        public void LocationByPoint_NotExistingPoint_ReturnLocationDTO()
        {
            Point point = new Point(1.1, 1.8);

            var actual = service.LocationByPoint(point);

            Assert.That(actual, Is.Null);
        }

        [Test]
        [Category("Location by Url")]
        public void LocationByUrl_ExistingUrl_ReturnLocationDTO()
        {
            Uri uri = new Uri(locationOnline.OnlineMeeting.ToString());
            Guid id = locationOnline.Id;

            var actual = service.LocationByURI(uri);

            Assert.That(Is.Equals(actual.Id, id), Is.True);
        }

        [Test]
        [Category("Location by Url")]
        public void LocationByUrl_NotExistingUrl_ReturnLocationDTO()
        {
            Uri uri = new Uri("http://basin.example.com/#branch");

            var actual = service.LocationByURI(uri);

            Assert.That(actual, Is.Null);
        }

        [TestCaseSource(typeof(CreatingExistingLocation))]
        [Category("Add Location To Event")]
        public void AddLocationToEvent_ExistingLocation_ReturnExistingLocationId(LocationDto locationDto)
        {
            var actual = service.AddLocationToEvent(locationDto);
            Assert.That(Is.Equals(locationDto.Id, actual.Result), Is.True);
        }

        [TestCaseSource(typeof(CreatingNotExistingLocation))]
        [Category("Add Location To Event")]
        public void AddLocationToEvent_NotExistingLocation_CreateNewLocation(LocationDto locationDto)
        {
            Assert.DoesNotThrowAsync(
                async () => await service.AddLocationToEvent(locationDto));
        }
    }
}
