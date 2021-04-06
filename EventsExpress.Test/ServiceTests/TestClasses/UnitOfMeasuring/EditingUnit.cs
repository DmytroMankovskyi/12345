﻿using System;
using System.Collections;
using EventsExpress.Core.DTOs;

namespace EventsExpress.Test.ServiceTests.TestClasses.UnitOfMeasuring
{
    public class EditingUnit : IEnumerable
    {
        private static UnitOfMeasuringDto unitOfMeasuringDTONotExId = new UnitOfMeasuringDto
        {
            Id = Guid.NewGuid(),
            UnitName = "CorrectUnitName",
            ShortName = "CSN",
            IsDeleted = false,
        };

        private static UnitOfMeasuringDto deletedUnitOfMeasuringDTO = new UnitOfMeasuringDto
        {
            Id = Guid.NewGuid(),
            UnitName = "DeletedUnitName",
            ShortName = "DSN",
            IsDeleted = true,
        };

        public static UnitOfMeasuringDto DeletedUnitOfMeasuringDTO
        {
            get => deletedUnitOfMeasuringDTO;
        }

        public static UnitOfMeasuringDto UnitOfMeasuringDTONotExId
        {
            get => unitOfMeasuringDTONotExId;
        }

        public IEnumerator GetEnumerator()
        {
            yield return new object[] { unitOfMeasuringDTONotExId };
            yield return new object[] { deletedUnitOfMeasuringDTO };
        }
    }
}
