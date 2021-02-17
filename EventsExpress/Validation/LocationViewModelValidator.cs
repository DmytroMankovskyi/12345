﻿namespace EventsExpress.Validation.Base
{
    using System;
    using EventsExpress.Db.Enums;
    using EventsExpress.ViewModels.Base;
    using FluentValidation;

    public class LocationViewModelValidator : AbstractValidator<LocationViewModel>
    {
        public LocationViewModelValidator()
        {
            RuleFor(x => x.Type).IsInEnum().WithMessage("Field Location Type is required!");
            When(location => location.Type == LocationType.Map, () =>
            {
                RuleFor(x => x.Latitude).NotEmpty().WithMessage("Field is required!");
                RuleFor(x => x.Longitude).NotEmpty().WithMessage("Field is required!");
            }).Otherwise(() =>
            {
                RuleFor(x => x.OnlineMeeting).Must(LinkMustBeAUri)
               .WithMessage("Link '{PropertyValue}' must be a valid URI. eg: http://www.SomeWebSite.com.au");
            });
        }

        private bool LinkMustBeAUri(string link)
        {
            if (string.IsNullOrWhiteSpace(link))
            {
                return false;
            }

            return Uri.TryCreate(link, UriKind.Absolute, out Uri outUri)
                   && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
