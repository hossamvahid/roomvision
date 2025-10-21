using Microsoft.EntityFrameworkCore;
using Moq;
using log4net;
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
            var logMock = new Mock<ILog>();

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

                var repo = new AccountRepository(mapperMock.Object, context, logMock.Object);

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
            var logMock = new Mock<ILog>();

            using var context = new PgSqlContext(options);
            var repo = new AccountRepository(mapperMock.Object, context, logMock.Object);

            var result = await repo.GetByIdAsync(999);
            
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByEmail_ReturnsAccount_WhenFound()
        {
            var options = DatabaseTestHelper.CreateInMemoryContext();
            var mapperMock = new Mock<IGenericMapper>();
            var logMock = new Mock<ILog>();

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

                var repo = new AccountRepository(mapperMock.Object, context, logMock.Object);

                var result = await repo.GetByEmailAsync("jane@example.com");

                Assert.NotNull(result);
                Assert.Equal("jane@example.com", result.Email);
            }
        }

        [Fact]
        public async Task Add_SavesAccount_ReturnsTrue()
        {
            var options = DatabaseTestHelper.CreateInMemoryContext();
            var mapperMock = new Mock<IGenericMapper>();
            var logMock = new Mock<ILog>();

            mapperMock.Setup(m => m.Map<Account, AccountDbModel>(It.IsAny<Account>()))
                .Returns((Account s) => new AccountDbModel { Id = s.Id, Name = s.Name, Email = s.Email, Password = s.Password });

            using (var context = new PgSqlContext(options))
            {
                var repo = new AccountRepository(mapperMock.Object, context, logMock.Object);

                var account = new Account { Id = 0, Name = "New", Email = "new@example.com", Password = "p" };
                var result = await repo.AddAsync(account);

                Assert.True(result);
            }

            using (var context = new PgSqlContext(options))
            {
                var saved = await context.Accounts.FirstOrDefaultAsync(a => a.Email == "new@example.com");
                Assert.NotNull(saved);
            }
        }

        [Fact]
        public async Task Update_ModifiesAccount_ReturnsTrue()
        {
            var options = DatabaseTestHelper.CreateInMemoryContext();
            var mapperMock = new Mock<IGenericMapper>();
            var logMock = new Mock<ILog>();

            using (var context = new PgSqlContext(options))
            {
                context.Accounts.Add(new AccountDbModel { Id = 5, Name = "ToUpdate", Email = "up@example.com", Password = "x" });
                await context.SaveChangesAsync();
            }

            mapperMock.Setup(m => m.Map<Account, AccountDbModel>(It.IsAny<Account>()))
                .Returns((Account s) => new AccountDbModel { Id = s.Id, Name = s.Name, Email = s.Email, Password = s.Password });

            using (var context = new PgSqlContext(options))
            {
                var repo = new AccountRepository(mapperMock.Object, context, logMock.Object);

                var account = new Account { Id = 5, Name = "Updated", Email = "up@example.com", Password = "y" };
                var result = await repo.UpdateAsync(account);

                Assert.True(result);

                var saved = await context.Accounts.FirstOrDefaultAsync(a => a.Id == 5);
                Assert.NotNull(saved);
                Assert.Equal("Updated", saved.Name);
            }
        }

        [Fact]
        public async Task Delete_RemovesAccount_ReturnsTrueOrFalse()
        {
            var options = DatabaseTestHelper.CreateInMemoryContext();
            var mapperMock = new Mock<IGenericMapper>();
            var logMock = new Mock<ILog>();

            using (var context = new PgSqlContext(options))
            {
                context.Accounts.Add(new AccountDbModel { Id = 10, Name = "Del", Email = "del@example.com", Password = "z" });
                await context.SaveChangesAsync();
            }

            using (var context = new PgSqlContext(options))
            {
                var repo = new AccountRepository(mapperMock.Object, context, logMock.Object);

                var deleted = await repo.DeleteByIdAsync(10);
                Assert.True(deleted);

                var deletedAgain = await repo.DeleteByIdAsync(10);
                Assert.False(deletedAgain);
            }
        }

        
        [Fact]
        public async Task Add_ReturnsFalse_WhenDbThrowsException()
        {
            var mapperMock = new Mock<IGenericMapper>();
            var logMock = new Mock<ILog>();

            mapperMock.Setup(m => m.Map<Account, AccountDbModel>(It.IsAny<Account>()))
                .Returns(new AccountDbModel());

           
            var options = DatabaseTestHelper.CreateInMemoryContext();
            using var context = new TestPgSqlContext(options, shouldThrowOnSave: true);
            
            var repo = new AccountRepository(mapperMock.Object, context, logMock.Object);
            var account = new Account { Name = "Test", Email = "test@example.com" };

            var result = await repo.AddAsync(account);

            Assert.False(result);
            logMock.Verify(l => l.Error(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Update_ReturnsFalse_WhenDbThrowsException()
        {
            var mapperMock = new Mock<IGenericMapper>();
            var logMock = new Mock<ILog>();

            mapperMock.Setup(m => m.Map<Account, AccountDbModel>(It.IsAny<Account>()))
                .Returns(new AccountDbModel());

            var options = DatabaseTestHelper.CreateInMemoryContext();
            using var context = new TestPgSqlContext(options, shouldThrowOnSave: true);
            
            var repo = new AccountRepository(mapperMock.Object, context, logMock.Object);
            var account = new Account { Id = 1, Name = "Test", Email = "test@example.com" };

            var result = await repo.UpdateAsync(account);

            Assert.False(result);
            logMock.Verify(l => l.Error(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsFalse_WhenDbThrowsException()
        {
            var mapperMock = new Mock<IGenericMapper>();
            var logMock = new Mock<ILog>();

            var options = DatabaseTestHelper.CreateInMemoryContext();
            
            
            using (var setupContext = new PgSqlContext(options))
            {
                setupContext.Accounts.Add(new AccountDbModel { Id = 20, Name = "ToDelete", Email = "delete@example.com", Password = "pwd" });
                await setupContext.SaveChangesAsync();
            }

            using var context = new TestPgSqlContext(options, shouldThrowOnSave: true);
            var repo = new AccountRepository(mapperMock.Object, context, logMock.Object);

            var result = await repo.DeleteByIdAsync(20);

            Assert.False(result);
            logMock.Verify(l => l.Error(It.IsAny<string>()), Times.Once);
        }
    }
}