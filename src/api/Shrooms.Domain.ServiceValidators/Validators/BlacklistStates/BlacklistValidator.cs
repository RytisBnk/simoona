﻿using Shrooms.Contracts.Constants;
using Shrooms.Contracts.DAL;
using Shrooms.Contracts.DataTransferObjects;
using Shrooms.Contracts.Enums;
using Shrooms.Contracts.Exceptions;
using Shrooms.DataLayer.EntityModels.Models;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Shrooms.Domain.ServiceValidators.Validators.BlacklistUsers
{
    public class BlacklistValidator : IBlacklistValidator
    {
        private readonly IDbSet<BlacklistUser> _blacklistStatesDbSet;
        private readonly IDbSet<ApplicationUser> _applicationUsersDbSet;

        public BlacklistValidator(IUnitOfWork2 uow)
        {
            _blacklistStatesDbSet = uow.GetDbSet<BlacklistUser>();
            _applicationUsersDbSet = uow.GetDbSet<ApplicationUser>();
        }

        public void CheckIfBlacklistUserExists(BlacklistUser blacklistUser)
        {
            if (blacklistUser == null)
            {
                throw new ValidationException(ErrorCodes.BlacklistStateNotFound, "Blacklist state not found");
            }
        }

        public async Task CheckIfUserExistsAsync(string userId, UserAndOrganizationDto userOrg)
        {
            if (!await _applicationUsersDbSet.AnyAsync(user => user.Id == userId && user.OrganizationId == userOrg.OrganizationId))
            {
                throw new ValidationException(ErrorCodes.UserNotFound, "User not found");
            }
        }

        public async Task CheckIfUserIsAlreadyBlacklistedAsync(string userId, UserAndOrganizationDto userOrg)
        {
            if (await _blacklistStatesDbSet.AnyAsync(blacklist => blacklist.UserId == userId &&
                                                                  blacklist.Status == BlacklistStatus.Active))
            {
                throw new ValidationException(ErrorCodes.DuplicatesIntolerable, "User is already blacklisted");
            }
        }
    }
}
