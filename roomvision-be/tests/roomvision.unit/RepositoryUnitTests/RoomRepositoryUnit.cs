using System;
using System.Threading;
using System.Threading.Tasks;
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
    public class RoomRepositoryUnit
    {
        [Fact]
        public async Task GetById_ReturnsRoom_WhenFound()
        {
            var options = DatabaseTestHelper.CreateInMemoryContext();
            var mapperMock = new Mock<IGenericMapper>();
            var logMock = new Mock<ILog>();

            using (var context = new PgSqlContext(options))
            {
                var dbModel = new RoomDbModel { Id = 1, RoomName = "Meeting Room", Password = "secret123" };
                context.Rooms.Add(dbModel);
                await context.SaveChangesAsync();
            }

            using (var context = new PgSqlContext(options))
            {
                mapperMock.Setup(m => m.Map<RoomDbModel, Room>(It.IsAny<RoomDbModel>()))
                    .Returns((RoomDbModel s) => new Room { Id = s.Id, RoomName = s.RoomName, Password = s.Password });

                var repo = new RoomRepository(mapperMock.Object, context, logMock.Object);

                var result = await repo.GetById(1);

                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
                Assert.Equal("Meeting Room", result.RoomName);
            }
        }

        [Fact]
        public async Task GetById_ReturnsNull_WhenNotFound()
        {
            var options = DatabaseTestHelper.CreateInMemoryContext();
            var mapperMock = new Mock<IGenericMapper>();
            var logMock = new Mock<ILog>();

            using var context = new PgSqlContext(options);
            var repo = new RoomRepository(mapperMock.Object, context, logMock.Object);

            var result = await repo.GetById(999);
            
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByRoomName_ReturnsRoom_WhenFound()
        {
            var options = DatabaseTestHelper.CreateInMemoryContext();
            var mapperMock = new Mock<IGenericMapper>();
            var logMock = new Mock<ILog>();

            using (var context = new PgSqlContext(options))
            {
                var dbModel = new RoomDbModel { Id = 2, RoomName = "Conference Room", Password = "conf456" };
                context.Rooms.Add(dbModel);
                await context.SaveChangesAsync();
            }

            using (var context = new PgSqlContext(options))
            {
                mapperMock.Setup(m => m.Map<RoomDbModel, Room>(It.IsAny<RoomDbModel>()))
                    .Returns((RoomDbModel s) => new Room { Id = s.Id, RoomName = s.RoomName, Password = s.Password });

                var repo = new RoomRepository(mapperMock.Object, context, logMock.Object);

                var result = await repo.GetByRoomName("Conference Room");

                Assert.NotNull(result);
                Assert.Equal("Conference Room", result.RoomName);
                Assert.Equal(2, result.Id);
            }
        }

        [Fact]
        public async Task GetByRoomName_ReturnsNull_WhenNotFound()
        {
            var options = DatabaseTestHelper.CreateInMemoryContext();
            var mapperMock = new Mock<IGenericMapper>();
            var logMock = new Mock<ILog>();

            using var context = new PgSqlContext(options);
            var repo = new RoomRepository(mapperMock.Object, context, logMock.Object);

            var result = await repo.GetByRoomName("Nonexistent Room");
            
            Assert.Null(result);
        }

        [Fact]
        public async Task Add_SavesRoom_ReturnsTrue()
        {
            var options = DatabaseTestHelper.CreateInMemoryContext();
            var mapperMock = new Mock<IGenericMapper>();
            var logMock = new Mock<ILog>();

            mapperMock.Setup(m => m.Map<Room, RoomDbModel>(It.IsAny<Room>()))
                .Returns((Room s) => new RoomDbModel { Id = s.Id, RoomName = s.RoomName, Password = s.Password });

            using (var context = new PgSqlContext(options))
            {
                var repo = new RoomRepository(mapperMock.Object, context, logMock.Object);

                var room = new Room { Id = 0, RoomName = "New Room", Password = "newpass" };
                var result = await repo.AddAsync(room);

                Assert.True(result);
            }

            using (var context = new PgSqlContext(options))
            {
                var saved = await context.Rooms.FirstOrDefaultAsync(r => r.RoomName == "New Room");
                Assert.NotNull(saved);
            }
        }

        [Fact]
        public async Task Update_ModifiesRoom_ReturnsTrue()
        {
            var options = DatabaseTestHelper.CreateInMemoryContext();
            var mapperMock = new Mock<IGenericMapper>();
            var logMock = new Mock<ILog>();

            using (var context = new PgSqlContext(options))
            {
                context.Rooms.Add(new RoomDbModel { Id = 5, RoomName = "Old Room", Password = "oldpass" });
                await context.SaveChangesAsync();
            }

            mapperMock.Setup(m => m.Map<Room, RoomDbModel>(It.IsAny<Room>()))
                .Returns((Room s) => new RoomDbModel { Id = s.Id, RoomName = s.RoomName, Password = s.Password });

            using (var context = new PgSqlContext(options))
            {
                var repo = new RoomRepository(mapperMock.Object, context, logMock.Object);

                var room = new Room { Id = 5, RoomName = "Updated Room", Password = "newpass" };
                var result = await repo.UpdateAsync(room);

                Assert.True(result);

                var saved = await context.Rooms.FirstOrDefaultAsync(r => r.Id == 5);
                Assert.NotNull(saved);
                Assert.Equal("Updated Room", saved.RoomName);
            }
        }

        [Fact]
        public async Task Delete_RemovesRoom_ReturnsTrueOrFalse()
        {
            var options = DatabaseTestHelper.CreateInMemoryContext();
            var mapperMock = new Mock<IGenericMapper>();
            var logMock = new Mock<ILog>();

            using (var context = new PgSqlContext(options))
            {
                context.Rooms.Add(new RoomDbModel { Id = 10, RoomName = "Delete Me", Password = "temp" });
                await context.SaveChangesAsync();
            }

            using (var context = new PgSqlContext(options))
            {
                var repo = new RoomRepository(mapperMock.Object, context, logMock.Object);

                var deleted = await repo.DeleteAsync(10);
                Assert.True(deleted);

                var deletedAgain = await repo.DeleteAsync(10);
                Assert.False(deletedAgain);
            }
        }

       
        [Fact]
        public async Task Add_ReturnsFalse_WhenDbThrowsException()
        {
            var mapperMock = new Mock<IGenericMapper>();
            var logMock = new Mock<ILog>();

            mapperMock.Setup(m => m.Map<Room, RoomDbModel>(It.IsAny<Room>()))
                .Returns(new RoomDbModel());

            var options = DatabaseTestHelper.CreateInMemoryContext();
            using var context = new TestPgSqlContext(options, shouldThrowOnSave: true);
            
            var repo = new RoomRepository(mapperMock.Object, context, logMock.Object);
            var room = new Room { RoomName = "Test Room", Password = "test" };

            var result = await repo.AddAsync(room);

            Assert.False(result);
            logMock.Verify(l => l.Error(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Update_ReturnsFalse_WhenDbThrowsException()
        {
            var mapperMock = new Mock<IGenericMapper>();
            var logMock = new Mock<ILog>();

            mapperMock.Setup(m => m.Map<Room, RoomDbModel>(It.IsAny<Room>()))
                .Returns(new RoomDbModel());

            var options = DatabaseTestHelper.CreateInMemoryContext();
            using var context = new TestPgSqlContext(options, shouldThrowOnSave: true);
            
            var repo = new RoomRepository(mapperMock.Object, context, logMock.Object);
            var room = new Room { Id = 1, RoomName = "Test Room", Password = "test" };

            var result = await repo.UpdateAsync(room);

            Assert.False(result);
            logMock.Verify(l => l.Error(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsFalse_WhenDbThrowsException()
        {
            var mapperMock = new Mock<IGenericMapper>();
            var logMock = new Mock<ILog>();

            var options = DatabaseTestHelper.CreateInMemoryContext();
            
            // First add a room, then use TestContext that throws on save
            using (var setupContext = new PgSqlContext(options))
            {
                setupContext.Rooms.Add(new RoomDbModel { Id = 20, RoomName = "ToDelete", Password = "temp" });
                await setupContext.SaveChangesAsync();
            }

            using var context = new TestPgSqlContext(options, shouldThrowOnSave: true);
            var repo = new RoomRepository(mapperMock.Object, context, logMock.Object);

            var result = await repo.DeleteAsync(20);

            Assert.False(result);
            logMock.Verify(l => l.Error(It.IsAny<string>()), Times.Once);
        }
    }
}