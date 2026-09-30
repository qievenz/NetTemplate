using Moq;
using NetTemplate.Application.Services;
using NetTemplate.Core.Entities;
using NetTemplate.Core.Exceptions;
using NetTemplate.Core.Repositories;
using NUnit.Framework;

namespace NetTemplate.Tests
{
    [TestFixture]
    public class DataServiceTests
    {
        private Mock<IDataRepository> _repositoryMock = null!;
        private DataService _service = null!;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IDataRepository>();
            _service = new DataService(_repositoryMock.Object);
        }

        [Test]
        public void GetDataByIdAsync_WithInvalidId_ThrowsInvalidInputException()
        {
            // Act & Assert
            Assert.ThrowsAsync<InvalidInputException>(async () => await _service.GetDataByIdAsync(0));
        }

        [Test]
        public async Task GetDataByIdAsync_WithValidId_ReturnsData()
        {
            // Arrange
            var expectedData = new Data { Id = 1, Description = "Test Description" };
            _repositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(expectedData);

            // Act
            var result = await _service.GetDataByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(1));
            Assert.That(result.Description, Is.EqualTo("Test Description"));
        }
    }
}
