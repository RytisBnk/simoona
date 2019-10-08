﻿using NSubstitute;
using NUnit.Framework;
using Shrooms.DataLayer.DAL;
using Shrooms.DataTransferObjects.Models.Lotteries;
using Shrooms.Domain.Services.Lotteries;
using Shrooms.EntityModels.Models;
using Shrooms.EntityModels.Models.Lotteries;
using Shrooms.UnitTests.Extensions;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Shrooms.Premium.UnitTests.DomainService.LotteryServices
{
    [TestFixture]
    public class ParticipantServiceTests
    {
        private IParticipantService _participantService;

        private IUnitOfWork2 _unitOfWork;
        private IDbSet<LotteryParticipant> _lotteryParticipants;

        [SetUp]
        public void TestInitializer()
        {
            _unitOfWork = Substitute.For<IUnitOfWork2>();
            _lotteryParticipants = _unitOfWork.MockDbSet<LotteryParticipant>();

            _participantService = new ParticipantService(_unitOfWork);
        }

        [TestCase(1, 6)]
        [TestCase(1000, 0)]
        public void GetParticipantsId_AnyLotteryId_ReturnsParticipantIds(int lotteryId, int participantsCount)
        {
            _lotteryParticipants.SetDbSetData(GetParticipants());

            var result = _participantService.GetParticipantsId(lotteryId);

            Assert.AreEqual(result.Count(), participantsCount);
        }

        [TestCase(5, 2)]
        [TestCase(1000, 0)]
        public void GetParticipantsCounted(int lotteryId, int tickets)
        {
            _lotteryParticipants.SetDbSetData(GetParticipants());

            var result = _participantService.GetParticipantsCounted(lotteryId);

            Assert.That(result, Is.All.Matches<LotteryParticipantDTO>(x => x.Tickets == tickets));
        }

        private IEnumerable<LotteryParticipant> GetParticipants()
        {
            var user1 = new ApplicationUser { Id = "1", FirstName = "Paul", LastName = "Giraffe" };
            var user2 = new ApplicationUser { Id = "2", FirstName = "Tom", LastName = "Bear" };
            var user3 = new ApplicationUser { Id = "3", FirstName = "John", LastName = "Zebra" };

            return new List<LotteryParticipant>
            {
                new LotteryParticipant { UserId = "1", LotteryId = 1, User = user1 },
                new LotteryParticipant { UserId = "1", LotteryId = 1, User = user1 },
                new LotteryParticipant { UserId = "1", LotteryId = 1, User = user1 },
                new LotteryParticipant { UserId = "1", LotteryId = 1, User = user1 },
                new LotteryParticipant { UserId = "1", LotteryId = 5, User = user1 },
                new LotteryParticipant { UserId = "1", LotteryId = 5, User = user1 },
                new LotteryParticipant { UserId = "2", LotteryId = 1, User = user2 },
                new LotteryParticipant { UserId = "2", LotteryId = 5, User = user2 },
                new LotteryParticipant { UserId = "2", LotteryId = 5, User = user2 },
                new LotteryParticipant { UserId = "3", LotteryId = 1, User = user3 },
                new LotteryParticipant { UserId = "3", LotteryId = 2, User = user3 },
                new LotteryParticipant { UserId = "3", LotteryId = 3, User = user3 },
                new LotteryParticipant { UserId = "3", LotteryId = 5, User = user3 },
                new LotteryParticipant { UserId = "3", LotteryId = 5, User = user3 }
            };
        }
    }
}
