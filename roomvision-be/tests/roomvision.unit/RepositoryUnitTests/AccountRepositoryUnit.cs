using Microsoft.EntityFrameworkCore;
using Moq;
using roomvision.domain.Entities;
using roomvision.domain.Interfaces.Mappers;
using roomvision.infrastructure.Contexts;
using roomvision.infrastructure.Models;
using roomvision.infrastructure.Repositories;
using roomvision.unit.TestHelpers;

namespace roomvision.unit.RepositoryUnitTests
{
    public class AccountRepositoryUnitTest
    {
        [Fact]
        public async Task GetById_ReturnsAccount_WhenFound()
        {
            var options = DatabaseTestHelper.CreateInMemoryContext();
            var mapperMock = new Mock<IGenericMapper>();

            using (var context = new PgSqlContext(options))
            {
                var dbModel = new AccountDbModel { Id = 1, Name = "John", Email = "john@example.com", Password = "secret" };
                context.Accounts.Add(dbModel);
                await context.SaveChangesAsync();
            }

            using (var context = new PgSqlContext(options))
            {
                mapperMock.Setup(m => m.Map<AccountDbModel, Account>(It.IsAny<AccountDbModel>()))
                    .Returns((AccountDbModel s) => new Account { Id = s.Id, Name = s.Name, Email = s.Email, Password = s.Password });

                var repo = new AccountRepository(mapperMock.Object, context);

                var result = await repo.GetByIdAsync(1);

                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
                Assert.Equal("john@example.com", result.Email);
            }
        }

        [Fact]
        public async Task GetById_ReturnsNull_WhenNotFound()
        {
            var options = DatabaseTestHelper.CreateInMemoryContext();
            var mapperMock = new Mock<IGenericMapper>();

            using var context = new PgSqlContext(options);
            var repo = new AccountRepository(mapperMock.Object, context);

            var result = await repo.GetByIdAsync(999);
            
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByEmail_ReturnsAccount_WhenFound()
        {
            var options = DatabaseTestHelper.CreateInMemoryContext();
            var mapperMock = new Mock<IGenericMapper>();

            using (var context = new PgSqlContext(options))
            {
                var dbModel = new AccountDbModel { Id = 2, Name = "Jane", Email = "jane@example.com", Password = "pwd" };
                context.Accounts.Add(dbModel);
                await context.SaveChangesAsync();
            }

            using (var context = new PgSqlContext(options))
            {
                mapperMock.Setup(m => m.Map<AccountDbModel, Account>(It.IsAny<AccountDbModel>()))
                    .Returns((AccountDbModel s) => new Account { Id = s.Id, Name = s.Name, Email = s.Email, Password = s.Password });

                var repo = new AccountRepository(mapperMock.Object, context);

                var result = await repo.GetByEmailAsync("jane@example.com");

                Assert.NotNull(result);
                Assert.Equal("jane@example.com", result.Email);
            }
        }

        [Fact]
        public async Task GetByEmail_ReturnsNull_WhenNotFound()
        {
            var options = DatabaseTestHelper.CreateInMemoryContext();
            var mapperMock = new Mock<IGenericMapper>();

            using var context = new PgSqlContext(options);
            var repo = new AccountRepository(mapperMock.Object, context);

            var result = await repo.GetByEmailAsync("nonexistent@example.com");
            
            Assert.Null(result);
        }

        [Fact]
        public async Task Add_SavesAccount()
        {
            var options = DatabaseTestHelper.CreateInMemoryContext();
            var mapperMock = new Mock<IGenericMapper>();

            mapperMock.Setup(m => m.Map<Account, AccountDbModel>(It.IsAny<Account>()))
                .Returns((Account s) => new AccountDbModel { Id = s.Id, Name = s.Name, Email = s.Email, Password = s.Password });

            using (var context = new PgSqlContext(options))
            {
                var repo = new AccountRepository(mapperMock.Object, context);

                var account = new Account { Id = 0, Name = "New", Email = "new@example.com", Password = "p" };
                await repo.AddAsync(account);
            }

            using (var context = new PgSqlContext(options))
            {
                var saved = await context.Accounts.FirstOrDefaultAsync(a => a.Email == "new@example.com");
                Assert.NotNull(saved);
                Assert.Equal("New", saved.Name);
            }
        }

        [Fact]
        public async Task Update_ModifiesAccount()
        {
            var options = DatabaseTestHelper.CreateInMemoryContext();
            var mapperMock = new Mock<IGenericMapper>();

            using (var context = new PgSqlContext(options))
            {
                context.Accounts.Add(new AccountDbModel { Id = 5, Name = "ToUpdate", Email = "up@example.com", Password = "x" });
                await context.SaveChangesAsync();
            }

            mapperMock.Setup(m => m.Map<Account, AccountDbModel>(It.IsAny<Account>()))
                .Returns((Account s) => new AccountDbModel { Id = s.Id, Name = s.Name, Email = s.Email, Password = s.Password });

            using (var context = new PgSqlContext(options))
            {
                var repo = new AccountRepository(mapperMock.Object, context);

                var account = new Account { Id = 5, Name = "Updated", Email = "up@example.com", Password = "y" };
                await repo.UpdateAsync(account);

                var saved = await context.Accounts.FirstOrDefaultAsync(a => a.Id == 5);
                Assert.NotNull(saved);
                Assert.Equal("Updated", saved.Name);
            }
        }

        [Fact]
        public async Task Delete_RemovesAccount()
        {
            var options = DatabaseTestHelper.CreateInMemoryContext();
            var mapperMock = new Mock<IGenericMapper>();

            var accountToDelete = new Account { Id = 10, Name = "Del", Email = "del@example.com", Password = "z" };

            using (var context = new PgSqlContext(options))
            {
                context.Accounts.Add(new AccountDbModel { Id = 10, Name = "Del", Email = "del@example.com", Password = "z" });
                await context.SaveChangesAsync();
            }

            mapperMock.Setup(m => m.Map<Account, AccountDbModel>(It.IsAny<Account>()))
                .Returns((Account s) => new AccountDbModel { Id = s.Id, Name = s.Name, Email = s.Email, Password = s.Password });

            using (var context = new PgSqlContext(options))
            {
                var repo = new AccountRepository(mapperMock.Object, context);

                await repo.DeleteByIdAsync(accountToDelete);

                var deleted = await context.Accounts.FirstOrDefaultAsync(a => a.Id == 10);
                Assert.Null(deleted);
            }
        }
    }
}