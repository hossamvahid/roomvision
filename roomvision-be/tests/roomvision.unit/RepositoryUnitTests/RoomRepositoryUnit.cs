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
    public class RoomRepositoryUnitNew
    {
        [Fact]
        public async Task GetById_ReturnsRoom_WhenFound()
        {
            var options = DatabaseTestHelper.CreateInMemoryContext();
            var mapperMock = new Mock<IGenericMapper>();

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

                var repo = new RoomRepository(mapperMock.Object, context);

                var result = await repo.GetByIdAsync(1);

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

            using var context = new PgSqlContext(options);
            var repo = new RoomRepository(mapperMock.Object, context);

            var result = await repo.GetByIdAsync(999);
            
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByRoomName_ReturnsRoom_WhenFound()
        {
            var options = DatabaseTestHelper.CreateInMemoryContext();
            var mapperMock = new Mock<IGenericMapper>();

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

                var repo = new RoomRepository(mapperMock.Object, context);

                var result = await repo.GetByRoomNameAsync("Conference Room");

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

            using var context = new PgSqlContext(options);
            var repo = new RoomRepository(mapperMock.Object, context);

            var result = await repo.GetByRoomNameAsync("Nonexistent Room");
            
            Assert.Null(result);
        }

        [Fact]
        public async Task Add_SavesRoom()
        {
            var options = DatabaseTestHelper.CreateInMemoryContext();
            var mapperMock = new Mock<IGenericMapper>();

            mapperMock.Setup(m => m.Map<Room, RoomDbModel>(It.IsAny<Room>()))
                .Returns((Room s) => new RoomDbModel { Id = s.Id, RoomName = s.RoomName, Password = s.Password });

            using (var context = new PgSqlContext(options))
            {
                var repo = new RoomRepository(mapperMock.Object, context);

                var room = new Room { Id = 0, RoomName = "New Room", Password = "newpass" };
                await repo.AddAsync(room);
            }

            using (var context = new PgSqlContext(options))
            {
                var saved = await context.Rooms.FirstOrDefaultAsync(r => r.RoomName == "New Room");
                Assert.NotNull(saved);
                Assert.Equal("newpass", saved.Password);
            }
        }

        [Fact]
        public async Task Update_ModifiesRoom()
        {
            var options = DatabaseTestHelper.CreateInMemoryContext();
            var mapperMock = new Mock<IGenericMapper>();

            using (var context = new PgSqlContext(options))
            {
                context.Rooms.Add(new RoomDbModel { Id = 5, RoomName = "Old Room", Password = "oldpass" });
                await context.SaveChangesAsync();
            }

            mapperMock.Setup(m => m.Map<Room, RoomDbModel>(It.IsAny<Room>()))
                .Returns((Room s) => new RoomDbModel { Id = s.Id, RoomName = s.RoomName, Password = s.Password });

            using (var context = new PgSqlContext(options))
            {
                var repo = new RoomRepository(mapperMock.Object, context);

                var room = new Room { Id = 5, RoomName = "Updated Room", Password = "newpass" };
                await repo.UpdateAsync(room);

                var saved = await context.Rooms.FirstOrDefaultAsync(r => r.Id == 5);
                Assert.NotNull(saved);
                Assert.Equal("Updated Room", saved.RoomName);
            }
        }

        [Fact]
        public async Task Delete_RemovesRoom()
        {
            var options = DatabaseTestHelper.CreateInMemoryContext();
            var mapperMock = new Mock<IGenericMapper>();

            var roomToDelete = new Room { Id = 10, RoomName = "Delete Me", Password = "temp" };

            using (var context = new PgSqlContext(options))
            {
                context.Rooms.Add(new RoomDbModel { Id = 10, RoomName = "Delete Me", Password = "temp" });
                await context.SaveChangesAsync();
            }

            mapperMock.Setup(m => m.Map<Room, RoomDbModel>(It.IsAny<Room>()))
                .Returns((Room s) => new RoomDbModel { Id = s.Id, RoomName = s.RoomName, Password = s.Password });

            using (var context = new PgSqlContext(options))
            {
                var repo = new RoomRepository(mapperMock.Object, context);

                await repo.DeleteAsync(roomToDelete);

                var deleted = await context.Rooms.FirstOrDefaultAsync(r => r.Id == 10);
                Assert.Null(deleted);
            }
        }
    }
}