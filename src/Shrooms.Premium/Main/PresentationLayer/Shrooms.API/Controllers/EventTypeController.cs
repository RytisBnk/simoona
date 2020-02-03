﻿using System;
using AutoMapper;
using Shrooms.API.Filters;
using Shrooms.Constants.Authorization.Permissions;
using Shrooms.DataTransferObjects.Models.Events;
using Shrooms.Domain.Services.Events.Utilities;
using Shrooms.DomainExceptions.Exceptions;
using Shrooms.DomainExceptions.Exceptions.Event;
using Shrooms.WebViewModels.Models.Events;
using System.Collections.Generic;
using System.Web.Http;

namespace Shrooms.API.Controllers.WebApi.EventTypeController
{
    [Authorize]
    [RoutePrefix("EventType")]
    public class EventTypeController : BaseController
    {
        private readonly IEventUtilitiesService _eventUtilitiesService;
        private readonly IMapper _mapper;

        public EventTypeController(IMapper mapper, IEventUtilitiesService eventUtilitiesService)
        {
            _mapper = mapper;
            _eventUtilitiesService = eventUtilitiesService;
        }

        [HttpGet]
        [PermissionAuthorize(Permission = AdministrationPermissions.Event)]
        [Route("Types")]
        public IHttpActionResult GetEventTypes()
        {
            var organizationId = GetUserAndOrganization().OrganizationId;
            var typeDtos = _eventUtilitiesService.GetEventTypes(organizationId);
            var result = _mapper.Map<IEnumerable<EventTypeDTO>, IEnumerable<EventTypeViewModel>>(typeDtos);
            return Ok(result);
        }

        [HttpGet]
        [PermissionAuthorize(Permission = AdministrationPermissions.Event)]
        [Route("Get")]
        public IHttpActionResult Get(int id)
        {
            if (id == 0)
            {
                return BadRequest("Invalid request");
            }

            var organizationId = GetUserAndOrganization().OrganizationId;

            try
            {
                var eventTypeDto = _eventUtilitiesService.GetEventType(organizationId, id);
                var eventTypeViewModel = _mapper.Map<EventTypeDTO, EventTypeViewModel>(eventTypeDto);

                return Ok(eventTypeViewModel);
            }
            catch (ValidationException e)
            {
                return BadRequestWithError(e);
            }
        }

        [HttpPost]
        [Route("Create")]
        [PermissionAuthorize(Permission = AdministrationPermissions.Event)]
        public IHttpActionResult Create(CreateEventTypeViewModel eventTypeViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var eventTypeDto = _mapper.Map<CreateEventTypeViewModel, CreateEventTypeDTO>(eventTypeViewModel);

            SetOrganizationAndUser(eventTypeDto);

            try
            {
                _eventUtilitiesService.CreateEventType(eventTypeDto);
            }
            catch (ValidationException e)
            {
                return BadRequestWithError(e);
            }

            return Ok();
        }

        [HttpPut]
        [Route("Update")]
        [PermissionAuthorize(Permission = AdministrationPermissions.Event)]
        public IHttpActionResult Update(UpdateEventTypeViewModel eventTypeViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var eventTypeDto = _mapper.Map<UpdateEventTypeViewModel, UpdateEventTypeDTO>(eventTypeViewModel);

            SetOrganizationAndUser(eventTypeDto);

            try
            {
                _eventUtilitiesService.UpdateEventType(eventTypeDto);
            }
            catch (ValidationException e)
            {
                return BadRequestWithError(e);
            }

            return Ok();
        }

        [HttpDelete]
        [Route("Delete")]
        [PermissionAuthorize(Permission = AdministrationPermissions.Event)]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                _eventUtilitiesService.DeleteEventType(id, GetUserAndOrganization());
                return Ok();
            }
            catch (EventException e)
            {
                return BadRequest(e.Message);
            }
            catch (ValidationException e)
            {
                return BadRequestWithError(e);
            }
        }

        [HttpGet]
        [Route("Groups")]
        [PermissionAuthorize(Permission = AdministrationPermissions.Event)]
        public IHttpActionResult GetSingleJoinGroupNames()
        {
            try
            {
                var groups = _eventUtilitiesService.GetEventTypesSingleJoinGroups(GetOrganizationId());

                return Ok(groups);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}