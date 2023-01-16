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
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(act);
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
            EmptyBrandNameException ex = Assert.Throws<EmptyBrandNameException>(act);
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
            InvalidBrandNameException ex = Assert.Throws<InvalidBrandNameException>(() => _domain.CreateRecord(brandName, flag));
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
            NameAlreadyExistsException ex = Assert.Throws<NameAlreadyExistsException>(() => _domain.CreateRecord(brandName, flag));
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
            InvalidIDFormatException ex = Assert.Throws<InvalidIDFormatException>(() => _domain.ReadRecord(id));
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
            InvalidIDFormatException ex = Assert.Throws<InvalidIDFormatException>(() => _domain.ReadRecord(id));
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
            InvalidIDFormatException ex = Assert.Throws<InvalidIDFormatException>(() => _domain.ReadRecord(id));
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
            InvalidIDFormatException ex = Assert.Throws<InvalidIDFormatException>(() => _domain.ReadRecord(id));
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

        [Test]
        public void ReadRecord_IDIsOkButNotFoundInDB_ThrowsNonExistentIDException()
        {
            // Arrange
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat(It.IsAny<string>())).Returns(1);
            _brandRepoMock.Setup(x => x.ReadIDs()).Returns(new List<int> { 2 });
            _validationMock.Setup(x => x.CheckIfIDExists(It.IsAny<int>(), It.IsAny<List<int>>())).Throws(new NonExistentIDException(""));
            var domain = new Domain(_validationMock.Object, _brandRepoMock.Object);

            // Act and Assert
            Assert.Throws<NonExistentIDException>(() => domain.ReadRecord("1"));
        }

        [Test]
        public void ReadRecord_IDIsOkAndFoundInDB_CallsReadRecord()
        {
            // Arrange
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat(It.IsAny<string>())).Returns(1);
            _brandRepoMock.Setup(x => x.ReadIDs()).Returns(new List<int> { 1 });
            _brandRepoMock.Setup(x => x.ReadRecord(It.IsAny<int>()));

            // Act
            _domain.ReadRecord("1");

            // Assert
            _brandRepoMock.Verify(x => x.ReadRecord(1), Times.Once);
        }

        [Test]
        public void ReadRecord_IDIsOkAndFoundInDB_ReadsRecord()
        {
            // Arrange
            List<Brand> expectedRecord = new List<Brand> { new Brand(1, "TestBrand", true) };
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat(It.IsAny<string>())).Returns(1);
            _brandRepoMock.Setup(x => x.ReadIDs()).Returns(new List<int> { 1 });
            _brandRepoMock.Setup(x => x.ReadRecord(1)).Returns(expectedRecord);

            // Act
            List<Brand> actualRecord = _domain.ReadRecord("1");

            // Assert
            Assert.That(actualRecord, Is.EqualTo(expectedRecord));
        }


        #endregion

        #region UpdateRecord tests

        [Test]
        public void UpdateRecord_CallsCheckIfIDIsInCorrectFormat()
        {
            // Arrange
            _brandRepoMock.Setup(x => x.ReadIDs()).Returns(new List<int> { 1 });
            _brandRepoMock.Setup(x => x.UpdateRecord(It.IsAny<Brand>())).Returns(1);

            // Act
            _domain.UpdateRecord("1", "TestBrand", true);

            // Assert
            _validationMock.Verify(x => x.CheckIfIDIsInCorrectFormat("1"), Times.Once);
        }

        [Test]
        public void UpdateRecord_EmptyID_ThrowsInvalidIDFormatException()
        {
            // Arrange
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat("")).Throws(new InvalidIDFormatException(""));

            // Act and Assert
            Assert.Throws<InvalidIDFormatException>(() => _domain.UpdateRecord("", "TestBrand", true));
        }

        [Test]
        public void UpdateRecord_NonDigitID_ThrowsInvalidIDFormatException()
        {
            // Arrange
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat("a")).Throws(new InvalidIDFormatException(""));

            // Act and Assert
            Assert.Throws<InvalidIDFormatException>(() => _domain.UpdateRecord("a", "TestBrand", true));
        }

        [Test]
        public void UpdateRecord_NonIntID_ThrowsInvalidIDFormatException()
        {
            // Arrange
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat("1.1")).Throws(new InvalidIDFormatException(""));

            // Act and Assert
            Assert.Throws<InvalidIDFormatException>(() => _domain.UpdateRecord("1.1", "TestBrand", true));
        }

        [Test]
        public void UpdateRecord_NegativeIntID_ThrowsInvalidIDFormatException()
        {
            // Arrange
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat("-1")).Throws(new InvalidIDFormatException(""));

            // Act and Assert
            Assert.Throws<InvalidIDFormatException>(() => _domain.UpdateRecord("-1", "TestBrand", true));
        }

        [Test]
        public void UpdateRecord_IDIsOk_CallsReadIDs()
        {
            // Arrange
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat(It.IsAny<string>())).Returns(1);
            _brandRepoMock.Setup(x => x.UpdateRecord(It.IsAny<Brand>())).Returns(1);

            // Act
            _domain.UpdateRecord("1", "TestBrand", true);

            // Assert
            _brandRepoMock.Verify(x => x.ReadIDs(), Times.Once);
        }

        [Test]
        public void UpdateRecord_IDIsOkButNotFoundInDB_ThrowsNonExistentIDException()
        {
            // Arrange
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat(It.IsAny<string>())).Returns(1);
            _brandRepoMock.Setup(x => x.ReadIDs()).Returns(new List<int> { 2 });
            _validationMock.Setup(x => x.CheckIfIDExists(1, It.IsAny<List<int>>())).Throws(new NonExistentIDException(""));

            // Act and Assert
            Assert.Throws<NonExistentIDException>(() => _domain.UpdateRecord("1", "TestBrand", true));
        }

        [Test]
        public void UpdateRecord_IDIsOkAndFoundInDBAndNullBrandName_ThrowsArgumentNullException()
        {
            // Arrange
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat(It.IsAny<string>())).Returns(1);
            _brandRepoMock.Setup(x => x.ReadIDs()).Returns(new List<int> { 1 });

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => _domain.UpdateRecord("1", null, true));
        }

        [Test]
        public void UpdateRecord_IDIsOkAndFoundInDBAndBrandNameIsNotNull_CallsCheckIfBrandNameIsEmpty()
        {
            // Arrange
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat(It.IsAny<string>())).Returns(1);
            _brandRepoMock.Setup(x => x.ReadIDs()).Returns(new List<int> { 1 });
            _brandRepoMock.Setup(x => x.UpdateRecord(It.IsAny<Brand>())).Returns(1);

            // Act
            _domain.UpdateRecord("1", "TestBrand", true);

            // Assert
            _validationMock.Verify(x => x.CheckIfBrandNameIsEmpty("TestBrand"), Times.Once);
        }

        [Test]
        public void UpdateRecord_IDIsOkAndFoundInDBAndBrandNameIsEmpty_ThrowsEmptyBrandNameException()
        {
            // Arrange
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat(It.IsAny<string>())).Returns(1);
            _brandRepoMock.Setup(x => x.ReadIDs()).Returns(new List<int> { 1 });
            _validationMock.Setup(x => x.CheckIfBrandNameIsEmpty("")).Throws(new EmptyBrandNameException(""));

            // Act and Assert
            Assert.Throws<EmptyBrandNameException>(() => _domain.UpdateRecord("1", "", true));
        }

        [Test]
        public void UpdateRecord_IDIsOkAndFoundInDBAndBrandNameIsNotNullOrEmpty_CallsCheckIfBrandNameHasInvalidCharacters()
        {
            // Arrange
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat(It.IsAny<string>())).Returns(1);
            _brandRepoMock.Setup(x => x.ReadIDs()).Returns(new List<int> { 1 });
            _brandRepoMock.Setup(x => x.UpdateRecord(It.IsAny<Brand>())).Returns(1);

            // Act
            _domain.UpdateRecord("1", "TestBrand", true);

            // Assert
            _validationMock.Verify(x => x.CheckIfBrandNameHasInvalidCharacters("TestBrand"), Times.Once);
        }

        [Test]
        public void UpdateRecord_IDIsOkAndFoundInDBAndBrandNameHasSymbolsExceptDash_ThrowsInvalidBrandNameException()
        {
            // Arrange
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat(It.IsAny<string>())).Returns(1);
            _brandRepoMock.Setup(x => x.ReadIDs()).Returns(new List<int> { 1 });
            _validationMock.Setup(x => x.CheckIfBrandNameHasInvalidCharacters("TestBrand!")).Throws(new InvalidBrandNameException(""));

            // Act and Assert
            Assert.Throws<InvalidBrandNameException>(() => _domain.UpdateRecord("1", "TestBrand!", true));
        }

        [Test]
        public void UpdateRecord_IDIsOkAndFoundInDBAndBrandNameIsOk_CallsReadBrandNames()
        {
            // Arrange
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat(It.IsAny<string>())).Returns(1);
            _brandRepoMock.Setup(x => x.ReadIDs()).Returns(new List<int> { 1 });
            _brandRepoMock.Setup(x => x.UpdateRecord(It.IsAny<Brand>())).Returns(1);

            // Act
            _domain.UpdateRecord("1", "TestBrand-Test", true);

            // Assert
            _brandRepoMock.Verify(x => x.ReadBrandNames(), Times.Once);
        }

        [Test]
        public void UpdateRecord_IDIsOkAndFoundInDBAndBrandNameIsOk_CallsCheckIfBrandNameAlreadyExists()
        {
            // Arrange
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat(It.IsAny<string>())).Returns(1);
            _brandRepoMock.Setup(x => x.ReadIDs()).Returns(new List<int> { 1 });
            _brandRepoMock.Setup(x => x.ReadBrandNames()).Returns(new List<string> { "TestBrand" });
            _brandRepoMock.Setup(x => x.UpdateRecord(It.IsAny<Brand>())).Returns(1);

            // Act
            _domain.UpdateRecord("1", "TestBrand-Test", true);

            // Assert
            _validationMock.Verify(x => x.CheckIfBrandNameAlreadyExists("TestBrand-Test", new List<string> { "TestBrand" }), Times.Once);
        }

        [Test]
        public void UpdateRecord_IDIsOkAndFoundInDBAndBrandNameIsOKButAlreadyExists_ThrowsNameAlreadyExistsException()
        {
            // Arrange
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat(It.IsAny<string>())).Returns(1);
            _brandRepoMock.Setup(x => x.ReadIDs()).Returns(new List<int> { 1 });
            _brandRepoMock.Setup(x => x.ReadBrandNames()).Returns(new List<string> { "TestBrand" });
            _validationMock.Setup(x => x.CheckIfBrandNameAlreadyExists("TestBrand", new List<string> { "TestBrand" })).Throws(new NameAlreadyExistsException(""));

            // Act and Assert
            Assert.Throws<NameAlreadyExistsException>(() => _domain.UpdateRecord("1", "TestBrand", true));
        }

        [Test]
        public void UpdateRecord_IDIsOkAndFoundInDBAndBrandNameIsOk_CallsUpdateRecord()
        {
            // Arrange
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat(It.IsAny<string>())).Returns(1);
            _brandRepoMock.Setup(x => x.ReadIDs()).Returns(new List<int> { 1 });
            _brandRepoMock.Setup(x => x.UpdateRecord(It.IsAny<Brand>())).Returns(1);

            // Act
            _domain.UpdateRecord("1", "TestBrand-Test", true);

            // Assert
            _brandRepoMock.Verify(x => x.UpdateRecord(It.IsAny<Brand>()), Times.Once);
        }

        [Test]
        public void UpdateRecord_IDIsOkAndFoundInDBAndBrandNameIsOk_UpdatesRecord()
        {
            // Arrange
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat(It.IsAny<string>())).Returns(1);
            _brandRepoMock.Setup(x => x.ReadIDs()).Returns(new List<int> { 1 });
            _brandRepoMock.Setup(x => x.UpdateRecord(It.IsAny<Brand>())).Returns(1);

            // Act
            int rowsAffected = _domain.UpdateRecord("1", "TestBrand", true);

            // Assert
            Assert.That(rowsAffected, Is.EqualTo(1));
        }

        #endregion

        #region DeleteRecord tests

        [Test]
        public void DeleteRecord_CallsCheckIfIDIsInCorrectFormat()
        {
            // Arrange
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat(It.IsAny<string>())).Returns(1);
            _brandRepoMock.Setup(x => x.ReadIDs()).Returns(new List<int> { 1 });

            // Act
            _domain.DeleteRecord("1");

            // Assert
            _validationMock.Verify(x => x.CheckIfIDIsInCorrectFormat("1"), Times.Once);
        }

        [Test]
        public void DeleteRecord_EmptyID_ThrowsInvalidIDFormatException()
        {
            // Arrange
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat("")).Throws(new InvalidIDFormatException(""));

            // Act and Assert
            Assert.Throws<InvalidIDFormatException>(() => _domain.DeleteRecord(""));
        }

        [Test]
        public void DeleteRecord_NonDigitID_ThrowsInvalidIDFormatException()
        {
            // Arrange
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat("abc")).Throws(new InvalidIDFormatException(""));

            // Act and Assert
            Assert.Throws<InvalidIDFormatException>(() => _domain.DeleteRecord("abc"));
        }

        [Test]
        public void DeleteRecord_NonIntID_ThrowsInvalidIDFormatException()
        {
            // Arrange
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat("1.5")).Throws(new InvalidIDFormatException(""));

            // Act and Assert
            Assert.Throws<InvalidIDFormatException>(() => _domain.DeleteRecord("1.5"));
        }

        [Test]
        public void DeleteRecord_NegativeIntID_ThrowsInvalidIDFormatException()
        {
            // Arrange
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat("-1")).Throws(new InvalidIDFormatException(""));

            // Act and Assert
            Assert.Throws<InvalidIDFormatException>(() => _domain.DeleteRecord("-1"));
        }

        [Test]
        public void DeleteRecord_IDIsOk_CallsReadIDs()
        {
            // Arrange
            int validID = 1;
            string id = validID.ToString();
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat(id)).Returns(validID);

            // Act
            _domain.DeleteRecord(id);

            // Assert
            _brandRepoMock.Verify(x => x.ReadIDs(), Times.Once);
        }

        [Test]
        public void DeleteRecord_IDIsOkButNotFoundInDB_ThrowsNonExistentIDException()
        {
            // Arrange
            int validID = 1;
            string id = validID.ToString();
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat(id)).Returns(validID);
            _validationMock.Setup(x => x.CheckIfIDExists(validID, It.IsAny<List<int>>())).Throws(new NonExistentIDException(""));

            // Act and Assert
            Assert.Throws<NonExistentIDException>(() => _domain.DeleteRecord(id));
        }

        [Test]
        public void DeleteRecord_IDIsOkAndFoundInDB_CallsDeleteRecord()
        {
            // Arrange
            int validID = 1;
            string id = validID.ToString();
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat(id)).Returns(validID);
            _validationMock.Setup(x => x.CheckIfIDExists(validID, It.IsAny<List<int>>()));

            // Act
            _domain.DeleteRecord(id);

            // Assert
            _brandRepoMock.Verify(x => x.DeleteRecord(validID), Times.Once);
        }

        [Test]
        public void DeleteRecord_IDIsOkAndFoundInDB_DeletesRecord()
        {
            // Arrange
            int validID = 1;
            string id = validID.ToString();
            _validationMock.Setup(x => x.CheckIfIDIsInCorrectFormat(id)).Returns(validID);
            _validationMock.Setup(x => x.CheckIfIDExists(validID, It.IsAny<List<int>>()));
            _brandRepoMock.Setup(x => x.DeleteRecord(validID)).Returns(1);

            // Act
            int result = _domain.DeleteRecord(id);

            // Assert
            Assert.That(result, Is.EqualTo(1));
        }

        #endregion
    }
}
