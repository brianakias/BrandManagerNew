using BrandManagerNew;
using BrandManagerNew.Exceptions;
using BrandManagerNew.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace BrandManagerTests
{
    public class Tests
    {
        private Mock<IDataAccess> mockedRepo;
        private Mock<IUserInputValidation> mockedValidation;
        private Domain domain;

        [SetUp]
        public void Setup()
        {
            mockedRepo = new Mock<IDataAccess>();
            mockedValidation = new Mock<IUserInputValidation>();
            domain = new Domain(mockedValidation.Object, mockedRepo.Object);
        }

        [Test]
        public void PrepareObjectForInsertion_ShouldReturnPassedBrand()
        {
            // Arrange
            Brand brand = new Brand("Armani", true);
            mockedRepo.Setup(x => x.ReadBrandNames()).Returns(new List<string>());

            // Act
            Brand result = domain.PrepareObjectForInsertion(brand.Name, brand.IsEnabled);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(brand.Name, Is.EqualTo(result.Name));
                Assert.That(brand.IsEnabled, Is.EqualTo(result.IsEnabled));
                mockedValidation.Verify(x => x.CheckIfBrandNameHasInvalidCharacters(brand.Name), Times.Once);
                mockedValidation.Verify(x => x.CheckIfBrandNameIsEmpty(brand.Name), Times.Once);
                mockedValidation.Verify(x => x.CheckIfBrandNameAlreadyExists(brand.Name, It.IsAny<List<string>>()), Times.Once);
                mockedRepo.Verify(x => x.ReadBrandNames(), Times.Once);
            });
        }

        [Test]
        public void PrepareObjectForInsertion_WhenBrandNameAlreadyExists_ShouldThrowNameAlreadyExistsException()
        {
            // Arrange
            Brand brand = new Brand("Dior", true);
            List<string> brandNames = new List<string>() { "Dior", "Avene" };
            mockedRepo.Setup(x => x.ReadBrandNames()).Returns(brandNames);
            mockedValidation.Setup(x => x.CheckIfBrandNameAlreadyExists(brand.Name, It.IsAny<List<string>>())).Throws(new NameAlreadyExistsException($"A brand with name '{brand.Name}' already exists."));

            // Act & Assert
            Assert.Multiple(() =>
            {
                Assert.Throws<NameAlreadyExistsException>(() => domain.PrepareObjectForInsertion(brand.Name, brand.IsEnabled));
                mockedValidation.Verify(x => x.CheckIfBrandNameHasInvalidCharacters(brand.Name), Times.Once);
                mockedValidation.Verify(x => x.CheckIfBrandNameIsEmpty(brand.Name), Times.Once);
                mockedValidation.Verify(x => x.CheckIfBrandNameAlreadyExists(brand.Name, It.IsAny<List<string>>()), Times.Once);
                mockedRepo.Verify(x => x.ReadBrandNames(), Times.Once);
            });
        }

        [Test]
        public void PrepareObjectForInsertion_WhenBrandNameIsEmpty_ShouldThrowEmptyBrandNameException()
        {
            // Arrange
            Brand brand = new Brand("", true);
            mockedRepo.Setup(x => x.ReadBrandNames()).Returns(new List<string>());
            mockedValidation.Setup(x => x.CheckIfBrandNameIsEmpty(brand.Name)).Throws(new EmptyBrandNameException("Brand name cannot be empty"));

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.Throws<EmptyBrandNameException>(() => domain.PrepareObjectForInsertion(brand.Name, brand.IsEnabled));
                mockedValidation.Verify(x => x.CheckIfBrandNameIsEmpty(brand.Name), Times.Once);
                mockedRepo.Verify(x => x.ReadBrandNames(), Times.Never);
                mockedValidation.Verify(x => x.CheckIfBrandNameHasInvalidCharacters(brand.Name), Times.Never);
                mockedValidation.Verify(x => x.CheckIfBrandNameAlreadyExists(brand.Name, It.IsAny<List<string>>()), Times.Never);
            });
        }

        [Test]
        public void PrepareObjectForInsertion_WhenBrandNameIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            Brand brand = new Brand(null, true);
            mockedRepo.Setup(x => x.ReadBrandNames()).Returns(new List<string>());

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentNullException>(() => domain.PrepareObjectForInsertion(brand.Name, brand.IsEnabled));
                mockedValidation.Verify(x => x.CheckIfBrandNameIsEmpty(brand.Name), Times.Never);
                mockedRepo.Verify(x => x.ReadBrandNames(), Times.Never);
                mockedValidation.Verify(x => x.CheckIfBrandNameHasInvalidCharacters(brand.Name), Times.Never);
                mockedValidation.Verify(x => x.CheckIfBrandNameAlreadyExists(brand.Name, It.IsAny<List<string>>()), Times.Never);
            });
        }

        [Test]
        public void PrepareObjectForInsertion_WhenInvalidBrandNameIsPassed_ShouldThrowInvalidBrandNameException()
        {
            // Arrange
            Brand brand = new Brand("Brand1", true);
            mockedValidation.Setup(x => x.CheckIfBrandNameHasInvalidCharacters(brand.Name)).Throws(new InvalidBrandNameException("Invalid brand name"));

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.Throws<InvalidBrandNameException>(() => domain.PrepareObjectForInsertion(brand.Name, brand.IsEnabled));
                mockedValidation.Verify(x => x.CheckIfBrandNameIsEmpty(brand.Name), Times.Once);
                mockedValidation.Verify(x => x.CheckIfBrandNameHasInvalidCharacters(brand.Name), Times.Once);
                mockedRepo.Verify(x => x.ReadBrandNames(), Times.Never);
                mockedValidation.Verify(x => x.CheckIfBrandNameAlreadyExists(brand.Name, It.IsAny<List<string>>()), Times.Never);
            });
        }

        [Test]
        public void PrepareObjectForUpdating_WhenValidIDIsPassed_BrandObjectReturned()
        {
            // Arrange
            Brand brand = new Brand(1, "Dior", true);
            mockedRepo.Setup(x => x.ReadIDs()).Returns(new List<int> { 1 });
            mockedRepo.Setup(x => x.ReadBrandNames()).Returns(new List<string>());

            // Act
            Brand result = domain.PrepareObjectForUpdating(brand.Id, brand.Name, brand.IsEnabled);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(brand, Is.Not.Null);
                Assert.That(brand.Id, Is.EqualTo(result.Id));
                Assert.That(brand.Name, Is.EqualTo(result.Name));
                Assert.That(brand.IsEnabled, Is.EqualTo(result.IsEnabled));
                mockedValidation.Verify(x => x.CheckIfIDExists(brand.Id, It.IsAny<List<int>>()), Times.Once);
                mockedValidation.Verify(x => x.CheckIfBrandNameHasInvalidCharacters(brand.Name), Times.Once);
                mockedRepo.Verify(x => x.ReadIDs(), Times.Once);
                mockedRepo.Verify(x => x.ReadBrandNames(), Times.Once);
                mockedValidation.Verify(x => x.CheckIfBrandNameAlreadyExists(brand.Name, It.IsAny<List<string>>()), Times.Once);
                mockedValidation.Verify(x => x.CheckIfBrandNameIsEmpty(brand.Name), Times.Once);

            });

        }

        [Test]
        public void PrepareObjectForUpdating_WhenIdIsInvalid_ShouldThrowNonExistentIDException()
        {
            // Arrange
            Brand brand = new Brand(20, "Dior", true);
            mockedRepo.Setup(x => x.ReadIDs()).Returns(new List<int> { 1, 2, 3 });
            mockedRepo.Setup(x => x.ReadBrandNames()).Returns(new List<string> { "Dior", "Decleor", "Caudalie" });
            mockedValidation.Setup(x => x.CheckIfIDExists(brand.Id, It.IsAny<List<int>>())).Throws(new NonExistentIDException("Invalid ID passed"));

            // Act and Assert
            Assert.Throws<NonExistentIDException>(() => domain.PrepareObjectForUpdating(brand.Id, brand.Name, brand.IsEnabled));
            mockedValidation.Verify(x => x.CheckIfIDExists(brand.Id, It.IsAny<List<int>>()), Times.Once);
            mockedRepo.Verify(x => x.ReadIDs(), Times.Once);
        }

        [Test]
        public void PrepareObjectForUpdating_WhenBrandNameIsEmpty_ShouldThrowEmptyBrandNameException()
        {
            // Arrange
            Brand brand = new Brand(1, "", true);
            mockedRepo.Setup(x => x.ReadIDs()).Returns(new List<int> { 1, 2 });
            mockedValidation.Setup(x => x.CheckIfBrandNameIsEmpty(brand.Name)).Throws(new EmptyBrandNameException("Brand name cannot be empty"));

            // Act and Assert
            Assert.Throws<EmptyBrandNameException>(() => domain.PrepareObjectForUpdating(brand.Id, brand.Name, brand.IsEnabled));
            mockedValidation.Verify(x => x.CheckIfBrandNameIsEmpty(brand.Name), Times.Once);
        }

        [Test]
        public void PrepareObjectForReadingOrDeletion_ValidId_IdReturned()
        {
            // Arrange
            int id = 1;
            mockedRepo.Setup(x => x.ReadIDs()).Returns(new List<int> { 1 });

            // Act
            int returnedId = domain.PrepareObjectForReadingOrDeletion(id);

            // Assert
            Assert.That(id, Is.EqualTo(returnedId));
            mockedValidation.Verify(x => x.CheckIfIDExists(id, It.IsAny<List<int>>()), Times.Once);
            mockedRepo.Verify(x => x.ReadIDs(), Times.Once);
        }

        [Test]
        public void PrepareObjectForReadingOrDeletion_WhenIdIsInvalid_ShouldThrowNonExistentIDException()
        {
            // Arrange
            int id = 10;
            mockedRepo.Setup(x => x.ReadIDs()).Returns(new List<int> { 1, 2 });
            mockedValidation.Setup(x => x.CheckIfIDExists(id, It.IsAny<List<int>>())).Throws(new NonExistentIDException("ID passed was invalid"));
            Domain domain = new Domain(mockedValidation.Object, mockedRepo.Object);

            // Act and Assert
            Assert.Throws<NonExistentIDException>(() => domain.PrepareObjectForReadingOrDeletion(id));
            mockedValidation.Verify(x => x.CheckIfIDExists(id, It.IsAny<List<int>>()), Times.Once);
        }

        [Test]
        public void ConfirmOneRecordWasAffected_WhenRecordsAffectedIsNotOne_ShouldThrowUnexpectedRecordsAffectedException()
        {
            // Arrange
            int recordsAffected = 2;

            // Act and Assert
            var ex = Assert.Throws<UnexpectedRecordsAffectedException>(() => domain.ConfirmOneRecordWasAffected(recordsAffected));
            StringAssert.Contains("Wrong number of records affected. Expected: 1 record, actual: 2", ex.Message);
        }

    }
}
