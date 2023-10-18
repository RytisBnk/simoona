using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Shrooms.Contracts.Constants;
using Shrooms.Contracts.DataTransferObjects;
using Shrooms.Contracts.DataTransferObjects.Models.Birthdays;
using Shrooms.Contracts.DataTransferObjects.Models.Kudos;
using Shrooms.Contracts.DataTransferObjects.Models.KudosBasket;
using Shrooms.Domain.Services.Banners;
using Shrooms.Domain.Services.Birthday;
using Shrooms.Domain.Services.Events;
using Shrooms.Domain.Services.Kudos;
using Shrooms.Domain.Services.KudosBaskets;
using Shrooms.Domain.Services.Permissions;
using Shrooms.Presentation.Common.Filters;
using Shrooms.Presentation.Common.Helpers;
using Shrooms.Presentation.WebViewModels.Models.Banners;
using Shrooms.Presentation.WebViewModels.Models.Birthday;
using Shrooms.Presentation.WebViewModels.Models.Events;
using Shrooms.Presentation.WebViewModels.Models.Users.Kudos;
using Shrooms.Presentation.WebViewModels.Models.Wall.Widgets;

namespace Shrooms.Presentation.Common.Controllers.Wall
{
    [Authorize]
    [RoutePrefix("WallWidgets")]
    public class WallWidgetsController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IKudosService _kudosService;
        private readonly IPermissionService _permissionService;
        private readonly IKudosBasketService _kudosBasketService;
        private readonly IBirthdayService _birthdayService;
        private readonly IEventWidgetService _eventWidgetService;
        private readonly IBannerWidgetService _bannerWidgetService;

        public WallWidgetsController(IMapper mapper,
            IKudosService kudosService,
            IPermissionService permissionService,
            IKudosBasketService kudosBasketService,
            IBirthdayService birthdayService,
            IEventWidgetService eventWidgetService,
            IBannerWidgetService bannerWidgetService)
        {
            _mapper = mapper;
            _kudosService = kudosService;
            _permissionService = permissionService;
            _kudosBasketService = kudosBasketService;
            _birthdayService = birthdayService;
            _eventWidgetService = eventWidgetService;
            _bannerWidgetService = bannerWidgetService;
        }

        [HttpGet]
        [PermissionAwareCacheOutputFilter(BasicPermissions.Kudos, BasicPermissions.Birthday, BasicPermissions.KudosBasket, BasicPermissions.Event, ServerTimeSpan = WebApiConstants.FiveMinutes)]
        public async Task<WidgetsViewModel> Get([FromUri] GetWidgetsViewModel getWidgetsViewModel)
        {
            var userAndOrganization = GetUserAndOrganization();

            return new WidgetsViewModel
            {
                KudosWidgetStats = await DefaultIfNotAuthorizedAsync(userAndOrganization, BasicPermissions.Kudos,
                    () => GetKudosWidgetStatsAsync(getWidgetsViewModel.KudosTabOneMonths, getWidgetsViewModel.KudosTabOneAmount, getWidgetsViewModel.KudosTabTwoMonths,
                        getWidgetsViewModel.KudosTabTwoAmount)),
                LastKudosLogRecords = await DefaultIfNotAuthorizedAsync(userAndOrganization, BasicPermissions.Kudos, GetLastKudosLogRecordsAsync),
                WeeklyBirthdays = await DefaultIfNotAuthorizedAsync(userAndOrganization, BasicPermissions.Birthday, GetWeeklyBirthdaysAsync),
                KudosBasketWidget = await DefaultIfNotAuthorizedAsync(userAndOrganization, BasicPermissions.KudosBasket, GetKudosBasketWidgetAsync),
                UpcomingEvents = await DefaultIfNotAuthorizedAsync(userAndOrganization, BasicPermissions.Event, GetUpcomingEventsAsync),
                Banners = await DefaultIfNotAuthorizedAsync(userAndOrganization, BasicPermissions.Wall, GetBannersAsync)
            };
        }

        private async Task<T> DefaultIfNotAuthorizedAsync<T>(UserAndOrganizationDto userAndOrganization, string permission, Func<Task<T>> valueFactory)
        {
            return await _permissionService.UserHasPermissionAsync(userAndOrganization, permission) ? await valueFactory() : default;
        }

        private async Task<IEnumerable<UpcomingEventWidgetViewModel>> GetUpcomingEventsAsync()
        {
            var events = await _eventWidgetService.GetUpcomingEventsAsync(GetOrganizationId());
            return _mapper.Map<IEnumerable<UpcomingEventWidgetViewModel>>(events);
        }

        private async Task<IEnumerable<BannerWidgetViewModel>> GetBannersAsync()
        {
            // var banners = await _bannerWidgetService.GetBannersAsync(GetOrganizationId());
            // return _mapper.Map<IEnumerable<BannerWidgetViewModel>>(banners);

            return new List<BannerWidgetViewModel>()
            {
                // new BannerWidgetViewModel()
                // {
                //     PictureId = "1234567890.png",
                //     Url = "https://www.google.com"
                // }
            };
        }

        private async Task<IEnumerable<WallKudosLogViewModel>> GetLastKudosLogRecordsAsync()
        {
            var userAndOrg = GetUserAndOrganization();
            var wallKudosLogsDto = await _kudosService.GetLastKudosLogsForWallAsync(userAndOrg);
            return _mapper.Map<IEnumerable<WallKudosLogDto>, IEnumerable<WallKudosLogViewModel>>(wallKudosLogsDto);
        }

        public async Task<KudosBasketWidgetViewModel> GetKudosBasketWidgetAsync()
        {
            var basket = await _kudosBasketService.GetKudosBasketWidgetAsync(GetUserAndOrganization());
            return basket == null ? null : _mapper.Map<KudosBasketDto, KudosBasketWidgetViewModel>(basket);
        }

        private async Task<IEnumerable<BirthdayViewModel>> GetWeeklyBirthdaysAsync()
        {
            var todayDate = DateTime.UtcNow;
            var birthdaysDto = await _birthdayService.GetWeeklyBirthdaysAsync(todayDate);
            var birthdays = _mapper.Map<IEnumerable<BirthdayDto>, IEnumerable<BirthdayViewModel>>(birthdaysDto);
            return birthdays;
        }

        private async Task<IEnumerable<KudosListBasicDataViewModel>> GetKudosWidgetStatsAsync(int tabOneMonths, int tabOneAmount, int tabTwoMonths, int tabTwoAmount)
        {
            var result = new List<KudosListBasicDataViewModel>
            {
                await CalculateStatsAsync(tabOneMonths, tabOneAmount),
                await CalculateStatsAsync(tabTwoMonths, tabTwoAmount)
            };

            return result;
        }

        private async Task<KudosListBasicDataViewModel> CalculateStatsAsync(int months, int amount)
        {
            var kudosStatsDto = await _kudosService.GetKudosStatsAsync(months, amount, User.Identity.GetOrganizationId());
            var stats = _mapper.Map<IEnumerable<KudosBasicDataDto>, IEnumerable<KudosBasicDataViewModel>>(kudosStatsDto);
            return new KudosListBasicDataViewModel
            {
                Users = stats,
                Months = months
            };
        }
    }
}
