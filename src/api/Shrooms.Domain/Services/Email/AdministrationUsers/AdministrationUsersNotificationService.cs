﻿using System.Data.Entity;
using System.Linq;
using Shrooms.Contracts.Constants;
using Shrooms.Contracts.DAL;
using Shrooms.Contracts.DataTransferObjects;
using Shrooms.Contracts.DataTransferObjects.EmailTemplateViewModels;
using Shrooms.Contracts.Infrastructure;
using Shrooms.Contracts.Infrastructure.Email;
using Shrooms.DataLayer.EntityModels.Models;
using Shrooms.Domain.Services.UserService;

namespace Shrooms.Domain.Services.Email.AdministrationUsers
{
    public class AdministrationUsersNotificationService : IAdministrationNotificationService
    {
        private readonly IDbSet<Organization> _organizationDbSet;
        private readonly IMailingService _mailingService;
        private readonly IApplicationSettings _appSettings;
        private readonly IMailTemplate _mailTemplate;
        private readonly IUserService _userService;

        public AdministrationUsersNotificationService(
            IUnitOfWork2 uow,
            IMailingService mailingService,
            IApplicationSettings appSettings,
            IMailTemplate mailTemplate,
            IUserService permissionService)
        {
            _organizationDbSet = uow.GetDbSet<Organization>();
            _mailingService = mailingService;
            _mailTemplate = mailTemplate;
            _appSettings = appSettings;
            _userService = permissionService;
        }

        public void SendConfirmedNotificationEmail(string userEmail, UserAndOrganizationDTO userAndOrg)
        {
            var organizationNameAndContent = _organizationDbSet
                    .Where(organization => organization.Id == userAndOrg.OrganizationId)
                    .Select(organization => new { organization.ShortName, organization.WelcomeEmail })
                    .FirstOrDefault();

            if (organizationNameAndContent == null)
            {
                return;
            }

            var mainPageUrl = _appSettings.ClientUrl;
            var userSettingsUrl = _appSettings.UserNotificationSettingsUrl(organizationNameAndContent.ShortName);
            var subject = string.Format(Resources.Common.NewUserConfirmedNotificationEmailSubject);

            var emailTemplateViewModel = new UserConfirmationEmailTemplateViewModel(userSettingsUrl, mainPageUrl, organizationNameAndContent.WelcomeEmail);

            var body = _mailTemplate.Generate(emailTemplateViewModel, EmailTemplateCacheKeys.UserConfirmation);

            _mailingService.SendEmail(new EmailDto(userEmail, subject, body));
        }

        public void NotifyAboutNewUser(ApplicationUser newUser, int orgId)
        {
            var userAdministrationEmails = _userService.GetUserEmailsWithPermission(AdministrationPermissions.ApplicationUser, orgId).ToList();

            if (!userAdministrationEmails.Any())
            {
                return;
            }

            var organizationName = _organizationDbSet
                .Where(organization => organization.Id == orgId)
                .Select(organization => organization.ShortName)
                .FirstOrDefault();

            var newUserProfileUrl = _appSettings.UserProfileUrl(organizationName, newUser.Id);
            var userSettingsUrl = _appSettings.UserNotificationSettingsUrl(organizationName);
            var subject = string.Format(Resources.Common.NewUserConfirmEmailSubject);

            var emailTemplateViewModel = new NotificationAboutNewUserEmailTemplateViewModel(
                userSettingsUrl,
                newUserProfileUrl,
                newUser.FullName);

            var body = _mailTemplate.Generate(emailTemplateViewModel, EmailTemplateCacheKeys.NotificationAboutNewUser);

            var emailDto = new EmailDto(userAdministrationEmails, subject, body);
            _mailingService.SendEmail(emailDto);
        }

        public void SendUserResetPasswordEmail(ApplicationUser user, string token, string organizationName)
        {
            var userSettingsUrl = _appSettings.UserNotificationSettingsUrl(organizationName);
            var resetUrl = _appSettings.ResetPasswordUrl(organizationName, user.UserName, token);

            var resetPasswordTemplateViewModel = new ResetPasswordTemplateViewModel(user.FullName, userSettingsUrl, resetUrl);
            var subject = string.Format(Resources.Common.UserResetPasswordEmailSubject);
            var content = _mailTemplate.Generate(resetPasswordTemplateViewModel, EmailTemplateCacheKeys.ResetPassword);

            _mailingService.SendEmail(new EmailDto(user.Email, subject, content));
        }

        public void SendUserVerificationEmail(ApplicationUser user, string token, string organizationName)
        {
            var userSettingsUrl = _appSettings.UserNotificationSettingsUrl(organizationName);
            var verifyUrl = _appSettings.VerifyEmailUrl(organizationName, user.UserName, token);

            var verifyEmailTemplateViewModel = new VerifyEmailTemplateViewModel(user.FullName, userSettingsUrl, verifyUrl);
            var subject = string.Format(Resources.Common.UserVerifyEmailSubject);
            var content = _mailTemplate.Generate(verifyEmailTemplateViewModel, EmailTemplateCacheKeys.VerifyEmail);

            _mailingService.SendEmail(new EmailDto(user.Email, subject, content));
        }
    }
}
