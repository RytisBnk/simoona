﻿using Shrooms.DataTransferObjects.Models;
using Shrooms.DataTransferObjects.Models.Events;
using Shrooms.Infrastructure.FireAndForget;
using Shrooms.Infrastructure.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shrooms.Premium.Main.BusinessLayer.Shrooms.Domain.Services.Notifications;
using Shrooms.API.Hubs;
using AutoMapper;
using Shrooms.WebViewModels.Models.Notifications;
using Shrooms;
using Shrooms.API;
using Shrooms.API.BackgroundWorkers;

namespace Shrooms.Premium.Main.PresentationLayer.Shrooms.API.BackgroundWorkers
{
    public class NewEventNotifier : IBackgroundWorker
    {
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public NewEventNotifier(INotificationService notificationService, IMapper mapper)
        {
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public void Notify(CreateEventDto eventDto, UserAndOrganizationHubDto userAndOrganizationHubDto)
        {
            var notification = _notificationService.CreateForEvent(userAndOrganizationHubDto, eventDto).GetAwaiter().GetResult();

            NotificationHub.SendNotificationToAllUsers(_mapper.Map<NotificationViewModel>(notification), userAndOrganizationHubDto);
        }
    }
}