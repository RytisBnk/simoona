﻿using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Shrooms.Contracts.DataTransferObjects;
using Shrooms.Contracts.DataTransferObjects.Wall.Posts;
using Shrooms.Contracts.Infrastructure;
using Shrooms.Contracts.ViewModels.Notifications;
using Shrooms.Domain.Services.Email.Posting;
using Shrooms.Domain.Services.Notifications;
using Shrooms.Domain.Services.UserService;
using Shrooms.Presentation.Api.Hubs;

namespace Shrooms.Presentation.Api.BackgroundWorkers
{
    public class NewPostNotifier : IBackgroundWorker
    {
        private readonly IPostNotificationService _postNotificationService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public NewPostNotifier(IPostNotificationService postNotificationService, IUserService userService, INotificationService notificationService, IMapper mapper)
        {
            _postNotificationService = postNotificationService;
            _userService = userService;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public async Task NotifyAsync(NewlyCreatedPostDto createdPost, UserAndOrganizationHubDto userAndOrganizationHubDto)
        {
            await _postNotificationService.NotifyAboutNewPostAsync(createdPost);

            var membersToNotify = (await _userService.GetWallUserAppNotificationEnabledIdsAsync(createdPost.User.UserId, createdPost.WallId)).ToList();

            var notificationDto = await _notificationService.CreateForPostAsync(userAndOrganizationHubDto, createdPost, createdPost.WallId, membersToNotify);

            var notificationViewModel = _mapper.Map<NotificationViewModel>(notificationDto);
            NotificationHub.SendNotificationToParticularUsers(notificationViewModel, userAndOrganizationHubDto, membersToNotify);
            NotificationHub.SendWallNotification(createdPost.WallId, membersToNotify, createdPost.WallType, userAndOrganizationHubDto);
        }
    }
}
