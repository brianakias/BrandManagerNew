using BrandManagerNew;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace BrandManagerTests
{
    public class Tests
    {
        private Mock<IDataAccess> brandMemoryRepository;
        private List<Brand> brands;
        private Domain domain;
        UserInputValidation validation;

        [SetUp]
        public void Setup()
        {
            brands = new List<Brand>
            {
                new Brand("Dior", true),
                new Brand("COSRX", false),
                new Brand("Joop", false),
                new Brand("REN", true),
            };

            brandMemoryRepository = new Mock<IDataAccess>();
            validation = new UserInputValidation();
            domain = new Domain(validation, brandMemoryRepository.Object);
        }

        [Test]
        public void ReadRecords_ReturnsAllBrands()
        {
            brandMemoryRepository.Setup(x => x.ReadRecords()).Returns(brands);
        }

        // write the test to read all brands using MOQ and confirm it behaves as expected
        // then check all the code paths in the domain class and declare the tests for the rest of the crud methods & ConfirmOneRecordWasAffected
        // then write the code inside the tests
    }
}
