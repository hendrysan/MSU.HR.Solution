﻿using MSU.HR.Models.Entities;
using MSU.HR.Services.Interfaces;
using MSU.HR.Services.Repositories;

namespace MSU.HR.UnitTests.Services
{
    public class BankTest : BaseContextInjection
    {
        private readonly IBank _bank;
        private Guid fakeId = Guid.Parse("b0b0b0b0-b0b0-b0b0-b0b0-b0b0b0b0b0b0");
        public BankTest()
        {
            _bank = new BankRepository(_context, _httpContextAccessor, _logError);
        }

        [Fact]
        public async Task GetBank()
        {
            // Act
            var bank = await _bank.GetBanksAsync();

            // Assert
            Assert.NotNull(bank);
            Assert.NotEmpty(bank);
        }

        [Fact]
        public async Task GetBankById()
        {
            Thread.Sleep(5000);
            // Arrange
            var bankId = fakeId;

            // Act
            var bank = await _bank.GetBankAsync(bankId);

            // Assert
            Assert.NotNull(bank);
            Assert.Equal(bankId, bank.Id);
        }

        [Fact]
        public async Task CreateBank()
        {
            //Arrage
            var bank = new Bank()
            {
                Id = fakeId,
                Code = "UT",
                Name = "Unit Test",
            };

            // Act
            var result = await _bank.CreateAsync(bank);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result);

        }
    }
}
