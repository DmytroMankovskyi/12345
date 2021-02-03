﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using EventsExpress.Core.DTOs;
using EventsExpress.Core.Extensions;
using EventsExpress.Core.IServices;
using EventsExpress.Core.Notifications;
using MediatR;

namespace EventsExpress.Core.NotificationHandlers
{
    public class BlockedEventHandler : INotificationHandler<BlockedEventMessage>
    {
        private readonly IEmailService _sender;
        private readonly IUserService _userService;
        private readonly IEventService _eventService;

        public BlockedEventHandler(
            IEmailService sender,
            IUserService userSrv,
            IEventService eventService)
        {
            _sender = sender;
            _userService = userSrv;
            _eventService = eventService;
        }

        public async Task Handle(BlockedEventMessage notification, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var userId in notification.UserIds)
                {
                    var email = _userService.GetById(userId).Email;
                    var even = _eventService.EventById(notification.Id);
                    string eventLink = $"{AppHttpContext.AppBaseUrl}/event/{notification.Id}/1";

                    await _sender.SendEmailAsync(new EmailDto
                    {
                        Subject = "Your event was blocked",
                        RecepientEmail = email,
                        MessageText = $"Dear {email}, your event was blocked for some reason. " +
                        $"To unblock it, edit this event, please: " +
                        $"\"<a href='{eventLink}'>{even.Title}</>\"",
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
