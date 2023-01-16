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
        private Mock<IDataAccess> _brandRepoMock;
        private Mock<IUserInputValidation> _validationMock;
        private Domain _domain;

        [SetUp]
        public void Setup()
        {
            _validationMock = new Mock<IUserInputValidation>();
            _brandRepoMock = new Mock<IDataAccess>();
            _domain = new Domain(_validationMock.Object, _brandRepoMock.Object);
        }

        #region CreateRecord tests

        [Test]
        public void CreateRecord_NullBrandName_ThrowsArgumentNullException()
        {
            // Arrange
            string brandName = null;
            bool flag = true;

            // Act
            TestDelegate act = () => _domain.CreateRecord(brandName, flag);

            // Assert
            var ex = Assert.Throws<ArgumentNullException>(act);
            Assert.That(ex.Message, Is.EqualTo("Value cannot be null."));
        }

        [Test]
        public void CreateRecord_BrandNameIsNotNull_CallsCheckIfBrandNameIsEmpty()
        {
            // Arrange
            string brandName = "Brand A";
            bool flag = true;

            // Act
            _domain.CreateRecord(brandName, flag);

            // Assert
            _validationMock.Verify(x => x.CheckIfBrandNameIsEmpty(brandName), Times.Once);
        }

        [Test]
        public void CreateRecord_BrandNameIsEmpty_ThrowsEmptyBrandNameException()
        {
            // Arrange
            string brandName = "";
            bool flag = true;
            _validationMock.Setup(x => x.CheckIfBrandNameIsEmpty(brandName))
                .Throws(new EmptyBrandNameException("Brand name cannot be empty."));
            _brandRepoMock.Setup(x => x.ReadBrandNames()).Returns(new List<string>());

            // Act
            TestDelegate act = () => _domain.CreateRecord(brandName, flag);

            // Assert
            var ex = Assert.Throws<EmptyBrandNameException>(act);
            Assert.That(ex.Message, Is.EqualTo("Brand name cannot be empty."));
        }

        [Test]
        public void CreateRecord_BrandNameIsNotNullOrEmpty_CallsCheckIfBrandNameHasInvalidCharacters()
        {
            // Arrange
            string brandName = "Brand A";
            bool flag = true;
            _brandRepoMock.Setup(x => x.ReadBrandNames()).Returns(new List<string>());

            // Act
            _domain.CreateRecord(brandName, flag);

            // Assert
            _validationMock.Verify(x => x.CheckIfBrandNameHasInvalidCharacters(brandName), Times.Once);
        }

        [Test]
        public void CreateRecord_BrandNameHasSymbolsExceptDash_ThrowsInvalidBrandNameException()
        {
            // Arrange
            string brandName = "Brand A#";
            bool flag = true;
            _validationMock.Setup(x => x.CheckIfBrandNameHasInvalidCharacters(brandName))
                .Throws(new InvalidBrandNameException("Brand name can only contain letters, numbers and dashes."));

            // Act and Assert
            var ex = Assert.Throws<InvalidBrandNameException>(() => _domain.CreateRecord(brandName, flag));
            _validationMock.Verify(x => x.CheckIfBrandNameHasInvalidCharacters(brandName), Times.Once);
            StringAssert.AreEqualIgnoringCase("Brand name can only contain letters, numbers and dashes.", ex.Message);
        }

        [Test]
        public void CreateRecord_BrandNameIsOk_CallsReadBrandNames()
        {
            // Arrange
            string brandName = "Brand A";
            bool flag = true;

            // Act 
            _domain.CreateRecord(brandName, flag);

            // Assert
            _brandRepoMock.Verify(x => x.ReadBrandNames(), Times.Once);
        }

        [Test]
        public void CreateRecord_BrandNameIsOk_CallsCheckIfBrandNameAlreadyExists()
        {
            // Arrange
            string brandName = "Brand A";
            bool flag = true;
            _brandRepoMock.Setup(x => x.ReadBrandNames()).Returns(new List<string>());

            // Act 
            _domain.CreateRecord(brandName, flag);

            // Assert
            _validationMock.Verify(x => x.CheckIfBrandNameAlreadyExists(brandName, It.IsAny<List<string>>()), Times.Once);
        }

        [Test]
        public void CreateRecord_BrandNameIsOKButAlreadyExists_ThrowsNameAlreadyExistsException()
        {
            // Arrange
            string brandName = "Brand A";
            bool flag = true;
            _brandRepoMock.Setup(x => x.ReadBrandNames()).Returns(new List<string> { brandName });
            _validationMock.Setup(x => x.CheckIfBrandNameAlreadyExists(brandName, It.IsAny<List<string>>()))
                .Throws(new NameAlreadyExistsException("Brand name already exists."));

            // Act and Assert
            var ex = Assert.Throws<NameAlreadyExistsException>(() => _domain.CreateRecord(brandName, flag));
            _validationMock.Verify(x => x.CheckIfBrandNameAlreadyExists(brandName, It.IsAny<List<string>>()), Times.Once);
            StringAssert.AreEqualIgnoringCase("Brand name already exists.", ex.Message);
        }

        [Test]
        public void CreateRecord_BrandNameIsOk_CallsCreateRecord()
        {
            // Arrange
            string brandName = "Brand A";
            bool flag = true;
            _brandRepoMock.Setup(x => x.ReadBrandNames()).Returns(new List<string>());
            _brandRepoMock.Setup(x => x.CreateRecord(It.IsAny<Brand>())).Returns(1);

            // Act 
            _domain.CreateRecord(brandName, flag);

            // Assert
            _brandRepoMock.Verify(x => x.CreateRecord(It.Is<Brand>(b => b.Name == brandName && b.IsEnabled == flag)), Times.Once);
        }

        [Test]
        public void CreateRecord_BrandNameIsOk_CreatesRecord()
        {
            // Arrange
            string brandName = "Brand A";
            bool flag = true;
            _brandRepoMock.Setup(x => x.ReadBrandNames()).Returns(new List<string>());
            _brandRepoMock.Setup(x => x.CreateRecord(It.IsAny<Brand>())).Returns(1);

            // Act 
            int rowsAffected = _domain.CreateRecord(brandName, flag);

            // Assert
            Assert.That(rowsAffected, Is.EqualTo(1));
        }

        #endregion

        #region ReadRecord tests

        [Test]
        public void ReadRecord_CallsCheckIfIDIsInCorrectFormat()
        {
            // Arrange
            string id = "1";
            _brandRepoMock.Setup(x => x.ReadIDs()).Returns(new List<int> { 1 });
            _brandRepoMock.Setup(x => x.ReadRecord(It.IsAny<int>())).Returns(new List<Brand>());

            // Act 
            _domain.ReadRecord(id);

            // Assert
            _validationMock.Verify(x => x.CheckIfIDIsInCorrectFormat(id), Times.Once);
        }

        [Test]
        public void ReadRecord_EmptyID_ThrowsInvalidIDFormatException()
        {
            // Arrange
            string id = "";
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat(id))
                .Throws(new InvalidIDFormatException("ID cannot be empty."));

            // Act and Assert
            var ex = Assert.Throws<InvalidIDFormatException>(() => _domain.ReadRecord(id));
            _validationMock.Verify(x => x.CheckIfIDIsInCorrectFormat(id), Times.Once);
            StringAssert.AreEqualIgnoringCase("ID cannot be empty.", ex.Message);
        }

        [Test]
        public void ReadRecord_NonDigitID_ThrowsInvalidIDFormatException()
        {
            // Arrange
            string id = "a";
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat(id))
                .Throws(new InvalidIDFormatException("ID must be a digit."));

            // Act and Assert
            var ex = Assert.Throws<InvalidIDFormatException>(() => _domain.ReadRecord(id));
            _validationMock.Verify(x => x.CheckIfIDIsInCorrectFormat(id), Times.Once);
            StringAssert.AreEqualIgnoringCase("ID must be a digit.", ex.Message);
        }

        [Test]
        public void ReadRecord_NonIntID_ThrowsInvalidIDFormatException()
        {
            // Arrange
            string id = "1.1";
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat(id))
                .Throws(new InvalidIDFormatException("ID must be an integer."));

            // Act and Assert
            var ex = Assert.Throws<InvalidIDFormatException>(() => _domain.ReadRecord(id));
            _validationMock.Verify(x => x.CheckIfIDIsInCorrectFormat(id), Times.Once);
            StringAssert.AreEqualIgnoringCase("ID must be an integer.", ex.Message);
        }

        [Test]
        public void ReadRecord_NegativeIntID_ThrowsInvalidIDFormatException()
        {
            // Arrange
            string id = "-1";
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat(id))
                .Throws(new InvalidIDFormatException("ID must be a positive integer."));

            // Act and Assert
            var ex = Assert.Throws<InvalidIDFormatException>(() => _domain.ReadRecord(id));
            _validationMock.Verify(x => x.CheckIfIDIsInCorrectFormat(id), Times.Once);
            StringAssert.AreEqualIgnoringCase("ID must be a positive integer.", ex.Message);
        }

        [Test]
        public void ReadRecord_IDIsOk_CallsReadIDs()
        {
            // Arrange
            string id = "1";

            // Act 
            _domain.ReadRecord(id);

            // Assert
            _brandRepoMock.Verify(x => x.ReadIDs(), Times.Once);
        }

        //[Test]
        //public void ReadRecord_IDIsOkButNotFoundInDB_ThrowsNonExistentIDException()
        //{
        //    // Arrange
        //    string id = "1";
        //    int convertedID = int.Parse(id);
        //    List<int> ids = new List<int> { 2 };
        //    _brandRepoMock.Setup(x => x.ReadIDs()).Returns(ids);
        //    _validationMock.Setup(x => x.CheckIfIDExists(convertedID, ids))
        //        .Throws(new NonExistentIDException("ID was not found in the DB"));

        //    // Act and Assert
        //    var ex = Assert.Throws<NonExistentIDException>(() => _domain.ReadRecord(id));
        //    _validationMock.Verify(x => x.CheckIfIDExists(convertedID, ids), Times.Once);
        //    StringAssert.AreEqualIgnoringCase("ID was not found in the DB.", ex.Message);

        //}

        [Test]
        public void ReadRecord_IDIsOkButNotFoundInDB_ThrowsNonExistentIDException()
        {
            // Arrange
            _validationMock.Setup(x => x.CheckIfIDExists(1, new List<int> { 2 }))
                .Throws(new NonExistentIDException("ID was not found in the DB"));
            _brandRepoMock.Setup(r => r.ReadIDs()).Returns(new List<int> { 2 });

            // Act and Assert
            Assert.Throws<NonExistentIDException>(() => _domain.ReadRecord("1"));

        }





        #endregion

        #region UpdateRecord tests


        #endregion

        #region DeleteRecord tests


        #endregion

        #region ConfirmOneRecordWasAffected tests


        #endregion

    }
}
