﻿using System;
using System.Collections;
using EventsExpress.ViewModels;

namespace EventsExpress.Test.ValidationTests.TestClasses.UnitOfMeasuring
{
    public class LetterAndCharactershUnitName : IEnumerable
    {
        private readonly UnitOfMeasuringViewModel modelNotLetterUnitName = new UnitOfMeasuringViewModel
        { Id = Guid.NewGuid(), UnitName = "78789878", ShortName = "rndSN" };

        private readonly UnitOfMeasuringViewModel modelLettersAndCharactersUnitName = new UnitOfMeasuringViewModel
        { Id = Guid.NewGuid(), UnitName = "78789878Unit", ShortName = "rndSN" };

        public IEnumerator GetEnumerator()
        {
            yield return new object[] { modelLettersAndCharactersUnitName };
            yield return new object[] { modelNotLetterUnitName };
        }
    }
}
