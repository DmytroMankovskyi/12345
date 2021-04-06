﻿using System;
using EventsExpress.Core.IServices;
using EventsExpress.Test.ValidationTests.TestClasses.UnitOfMeasuring;
using EventsExpress.Validation;
using EventsExpress.ViewModels;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace EventsExpress.Test.ServiceTests
{
    [TestFixture]
    public class UnitOfMeasuringViewModelValidatorTests : TestInitializer
    {
        private const string ExistedUnitOfMeasuring = "The same UNIT OF MEASURING and SHORT UNIT OF MEASURING already exists!";
        private const string CountOfCharactersUnitName = "Unit Name needs to consist of from 5 to 20 characters";
        private const string OnlyCharactersUnitName = "Unit name needs to consist only letters or whitespaces";
        private const string CountOfCharactersShortName = "Short Name needs to consist of from 1 to 5 characters";
        private const string OnlyCharactersShortName = "Short name needs to consist only letters or letter(s)/letter(s)";

        private readonly string existedUnitName = "Existed Unit Name";
        private readonly string existedShortName = "Ex/SN";
        private readonly string notExistedUnitName = "Not Existed Unit Name";
        private readonly string notExistedShortName = "N/SN";

        private UnitOfMeasuringViewModelValidator unitOfMeasuringViewModelValidator;

        [SetUp]
        protected override void Initialize()
        {
            base.Initialize();
            var mockUnitService = new Mock<IUnitOfMeasuringService>();
            unitOfMeasuringViewModelValidator = new UnitOfMeasuringViewModelValidator(mockUnitService.Object);
            mockUnitService.Setup(x => x.ExistsByName(
             It.Is<string>(i => i == existedUnitName),
             It.Is<string>(i => i == existedShortName))).Returns(true);
        }

        [Test]
        [Category("Existing OR Not Existing Unit Of Measuring")]
        public void ShoudHaveError_ExistingUnitOfMeasuring()
        {
            UnitOfMeasuringViewModel existedModel = new UnitOfMeasuringViewModel
            { Id = Guid.NewGuid(), UnitName = existedUnitName, ShortName = existedShortName };
            var result = unitOfMeasuringViewModelValidator.TestValidate(existedModel);
            result.ShouldHaveValidationErrorFor(x => x).WithErrorMessage(ExistedUnitOfMeasuring);
        }

        [Test]
        [Category("Existing OR Not Existing Unit Of Measuring")]
        public void ShoudNotHaveError_NotExistingUnitOfMeasuring()
        {
            UnitOfMeasuringViewModel notExistedModel = new UnitOfMeasuringViewModel
            { Id = Guid.NewGuid(), UnitName = notExistedUnitName, ShortName = notExistedShortName };
            var result = unitOfMeasuringViewModelValidator.TestValidate(notExistedModel);
            result.ShouldNotHaveValidationErrorFor(x => x);
        }

        [TestCaseSource(typeof(CorrectShortName))]
        [Category("Correct Short Name")]
        public void ShoudNotHaveError_CorrectShortName(UnitOfMeasuringViewModel model)
        {
            var result = unitOfMeasuringViewModelValidator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.ShortName);
        }

        [TestCaseSource(typeof(CorrectUnitName))]
        [Category("Correct Unit Name")]
        public void ShoudNotHaveError_CorrectUnitName(UnitOfMeasuringViewModel model)
        {
            var result = unitOfMeasuringViewModelValidator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.UnitName);
        }

        [TestCaseSource(typeof(InCorrectLengthUnitName))]
        [Category("InCorrect Unit Name")]
        public void ShoudHaveError_InCorrectLengthUnitName(UnitOfMeasuringViewModel model)
        {
            var result = unitOfMeasuringViewModelValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.UnitName).WithErrorMessage(CountOfCharactersUnitName);
        }

        [TestCaseSource(typeof(LetterAndCharactershUnitName))]
        [Category("InCorrect Unit Name")]
        public void ShoudHaveError_LetterAndCharactersUnitName(UnitOfMeasuringViewModel model)
        {
            var result = unitOfMeasuringViewModelValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.UnitName).WithErrorMessage(OnlyCharactersUnitName);
        }

        [TestCaseSource(typeof(LittleAndBigCharactershUnitName))]
        [Category("InCorrect Unit Name")]
        public void ShoudHaveError_LittleAndBigCharactersUnitName(UnitOfMeasuringViewModel model)
        {
            var result = unitOfMeasuringViewModelValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.UnitName)
                  .WithErrorMessage(CountOfCharactersUnitName)
                  .WithErrorMessage(OnlyCharactersUnitName);
        }

        [TestCaseSource(typeof(EmptyORManyLettersShortName))]
        [Category("InCorrect Short Name")]
        public void ShoudHaveError_EmptyOrManyLettersShortName(UnitOfMeasuringViewModel model)
        {
            var result = unitOfMeasuringViewModelValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ShortName)
                  .WithErrorMessage(CountOfCharactersShortName);
        }

        [TestCaseSource(typeof(DifferentCharactersSlashShortName))]
        [Category("InCorrect Short Name")]
        public void ShoudHaveError_DifferentCharactersSlashShortName(UnitOfMeasuringViewModel model)
        {
            var result = unitOfMeasuringViewModelValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ShortName)
                  .WithErrorMessage(OnlyCharactersShortName);
        }

        [TestCaseSource(typeof(DifferentCharactersSlashLengthShortName))]
        [Category("InCorrect Short Name")]
        public void ShoudHaveError_DifferentCharactersSlashLengthShortName(UnitOfMeasuringViewModel model)
        {
            var result = unitOfMeasuringViewModelValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ShortName)
                  .WithErrorMessage(OnlyCharactersShortName)
                  .WithErrorMessage(CountOfCharactersShortName);
        }
    }
}
