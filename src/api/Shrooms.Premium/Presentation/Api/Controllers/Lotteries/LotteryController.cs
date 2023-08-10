using AutoMapper;
using Shrooms.Contracts.Constants;
using Shrooms.Contracts.Exceptions;
using Shrooms.Domain.Extensions;
using Shrooms.Premium.DataTransferObjects.Models.Lotteries;
using Shrooms.Premium.Domain.DomainExceptions.Lotteries;
using Shrooms.Premium.Domain.Services.Lotteries;
using Shrooms.Premium.Presentation.WebViewModels.Lotteries;
using Shrooms.Presentation.Common.Controllers;
using Shrooms.Presentation.Common.Filters;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi.OutputCache.V2;

namespace Shrooms.Premium.Presentation.Api.Controllers.Lotteries
{
    [Authorize]
    [RoutePrefix("Lottery")]
    public class LotteryController : BaseController
    {
        private readonly IMapper _mapper;

        private readonly ILotteryService _lotteryService;
        private readonly ILotteryExportService _lotteryExportService;

        public LotteryController(IMapper mapper, ILotteryService lotteryService, ILotteryExportService lotteryExportService)
        {
            _mapper = mapper;
            _lotteryService = lotteryService;
            _lotteryExportService = lotteryExportService;
        }

        [Route("All")]
        public async Task<IHttpActionResult> GetAllLotteries()
        {
            var lotteriesDto = await _lotteryService.GetLotteriesAsync(GetUserAndOrganization());

            var result = _mapper.Map<IEnumerable<LotteryDetailsDto>, IEnumerable<LotteryDetailsViewModel>>(lotteriesDto);

            return Ok(result);
        }

        [HttpGet]
        [Route("Paged")]
        [PermissionAuthorize(Permission = AdministrationPermissions.Lottery)]
        public async Task<IHttpActionResult> GetPagedLotteries([FromUri] LotteryListingArgsViewModel lotteryArgsViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            lotteryArgsViewModel ??= new LotteryListingArgsViewModel();

            var lotteryArgsDto = _mapper.Map<LotteryListingArgsViewModel, LotteryListingArgsDto>(lotteryArgsViewModel);
            var pagedLotteriesDto = await _lotteryService.GetPagedLotteriesAsync(lotteryArgsDto, GetUserAndOrganization());
            var pagedLotteriesViewModel = _mapper.Map<IEnumerable<LotteryDetailsDto>, IEnumerable<LotteryDetailsViewModel>>(pagedLotteriesDto);

            return Ok(pagedLotteriesDto.ToPagedViewModel(pagedLotteriesViewModel, lotteryArgsDto));
        }

        [HttpGet]
        [Route("{id}/Details")]
        public async Task<IHttpActionResult> GetLottery(int id, bool includeBuyer = false)
        {
            var lottery = await _lotteryService.GetLotteryDetailsAsync(id, includeBuyer, GetUserAndOrganization());

            if (lottery == null)
            {
                return NotFound();
            }

            var lotteryViewModel = _mapper.Map<LotteryDetailsDto, LotteryDetailsViewModel>(lottery);

            return Ok(lotteryViewModel);
        }

        [HttpPost]
        [Route("Create")]
        [InvalidateCacheOutput("Get", typeof(LotteryWidgetController))]
        public async Task<IHttpActionResult> CreateLottery(CreateLotteryViewModel createViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createLotteryDto = _mapper.Map<CreateLotteryViewModel, LotteryDto>(createViewModel);

                await _lotteryService.CreateLotteryAsync(createLotteryDto, GetUserAndOrganization());

                return Ok(createViewModel);
            }
            catch (LotteryException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPatch]
        [Route("{id}/Abort")]
        [InvalidateCacheOutput("Get", typeof(LotteryWidgetController))]
        public async Task<IHttpActionResult> Abort(int id)
        {
            var success = await _lotteryService.AbortLotteryAsync(id, GetUserAndOrganization());

            if (!success)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        [Route("Enter")]
        public async Task<IHttpActionResult> BuyLotteryTicket(BuyLotteryTicketsViewModel lotteryTickets)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var buyLotteryTicket = _mapper.Map<BuyLotteryTicketsViewModel, BuyLotteryTicketsDto>(lotteryTickets);

                await _lotteryService.BuyLotteryTicketsAsync(buyLotteryTicket, GetUserAndOrganization());

                return Ok();
            }
            catch (LotteryException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequestWithError(ex);
            }
        }

        [HttpPatch]
        [Route("{id}/Refund")]
        public async Task<IHttpActionResult> RefundParticipants(int id)
        {
            await _lotteryService.RefundParticipantsAsync(id, GetUserAndOrganization());

            return Ok();
        }

        [HttpGet]
        [Route("{id}/Status")]
        public async Task<IHttpActionResult> GetStatus(int id)
        {
            var status = await _lotteryService.GetLotteryStatusAsync(id, GetUserAndOrganization());

            return Ok(status);
        }

        [HttpPut]
        [Route("UpdateDrafted")]
        [InvalidateCacheOutput("Get", typeof(LotteryWidgetController))]
        public async Task<IHttpActionResult> UpdateDrafted(EditDraftedLotteryViewModel editLotteryViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var editDraftedLotteryDto = _mapper.Map<EditDraftedLotteryViewModel, LotteryDto>(editLotteryViewModel);

                await _lotteryService.EditDraftedLotteryAsync(editDraftedLotteryDto, GetUserAndOrganization());

                return Ok(editLotteryViewModel);
            }
            catch (LotteryException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPatch]
        [Route("UpdateStarted")]
        [InvalidateCacheOutput("Get", typeof(LotteryWidgetController))]
        public async Task<IHttpActionResult> UpdateStarted(EditStartedLotteryViewModel editLotteryViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var editStartedLotteryDto = _mapper.Map<EditStartedLotteryViewModel, EditStartedLotteryDto>(editLotteryViewModel);

                await _lotteryService.EditStartedLotteryAsync(editStartedLotteryDto, GetUserAndOrganization());

                return Ok(editLotteryViewModel);
            }
            catch (LotteryException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPatch]
        [Route("{id}/Finish")]
        [InvalidateCacheOutput("Get", typeof(LotteryWidgetController))]
        public async Task<IHttpActionResult> FinishLottery(int id)
        {
            try
            {
                await _lotteryService.FinishLotteryAsync(id, GetUserAndOrganization());

                return Ok();
            }
            catch (LotteryException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("{id}/Stats")]
        public async Task<IHttpActionResult> LotteryStats(int id)
        {
            var lotteryStats = await _lotteryService.GetLotteryStatsAsync(id, GetUserAndOrganization());

            if (lotteryStats == null)
            {
                return NotFound();
            }

            return Ok(lotteryStats);
        }

        [HttpGet]
        [PermissionAuthorize(Permission = AdministrationPermissions.Lottery)]
        [Route("Export")]
        public async Task<IHttpActionResult> Export(int lotteryId)
        {
            try
            {
                var content = await _lotteryExportService.ExportParticipantsAsync(lotteryId, GetUserAndOrganization());
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = content
                };

                return ResponseMessage(result);
            }
            catch (LotteryException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
