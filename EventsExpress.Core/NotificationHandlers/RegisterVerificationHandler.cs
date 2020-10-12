﻿using System;
using System.Collections.Generic;
using System.Text;
using EventsExpress.Core.DTOs;
using EventsExpress.Core.IServices;
using EventsExpress.Core.Notifications;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using EventsExpress.Core.Infrastructure;
using EventsExpress.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace EventsExpress.Core.NotificationHandlers
{
    public class RegisterVerificationHandler :INotificationHandler<RegisterVerificationMessage>
    {
        private readonly IEmailService _sender;
        private readonly ICacheHelper _cacheHepler;
        private readonly ILogger<RegisterVerificationHandler> _logger;



        public RegisterVerificationHandler
            (
            IEmailService sender,
            ICacheHelper cacheHepler,
            ILogger<RegisterVerificationHandler> logger
           
            )
        {
            _sender = sender;
            _cacheHepler = cacheHepler;
            _logger = logger;
            

        }
        public async Task Handle(RegisterVerificationMessage notification, CancellationToken cancellationToken)
        {
            var token = Guid.NewGuid().ToString();
            string theEmailLink = $"<a \" target=\"_blank\" href=\"{AppHttpContext.AppBaseUrl}/authentication/{notification.User.Id}/{token}\">link</a>";
               
            
            _cacheHepler.Add(new CacheDTO
            {
                UserId = notification.User.Id,
                Token = token
            });

            try
            {
                await _sender.SendEmailAsync(new EmailDTO
                {
                    Subject = "EventExpress registration",
                    RecepientEmail = notification.User.Email,
                    MessageText = $"For  confirm your email please follow the {theEmailLink}   "
                });

                var x = _cacheHepler.GetValue(notification.User.Id);

            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
