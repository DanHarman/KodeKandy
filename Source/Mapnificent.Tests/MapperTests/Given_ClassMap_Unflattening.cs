using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;

namespace KodeKandy.Mapnificent.Tests.MapperTests
{
    [TestFixture]
    public class Given_ClassMap_Unflattening
    {
        public class Dto
        {
            public string P1 { get; set; }
            public string P2 { get; set; }
            public string P3 { get; set; }
        }

        public class SubDomainModel
        {
            public string P1 { get; set; }
            public string P2 { get; set; }
        }

        public class DomainModel
        {
            public SubDomainModel SubModel { get; set; }
            public string P3 { get; set; }
        }

        [Test]
        public void When_Mapping_Then_Unflattens()
        {
            var dto = new Dto
            {
                P1 = "A",
                P2 = "B",
                P3 = "C"
            };

            // Arrange
            var mapper = new Mapper();
            mapper.BuildClassMap<Dto, SubDomainModel>();
            mapper.BuildClassMap<Dto, DomainModel>().For(t => t.SubModel, o => o.From(x => x));

            // Act
            var model = mapper.Map<Dto, DomainModel>(dto);

            // Assert
            Assert.AreEqual("A", model.SubModel.P1);
            Assert.AreEqual("B", model.SubModel.P2);
            Assert.AreEqual("C", model.P3);
        }

        [Test]
        public void When_Mapping_Then_Can_Project_Inline()
        {
            var dto = new Dto
            {
                P1 = "A",
                P2 = "B",
                P3 = "C"
            };

            // Arrange
            var mapper = new Mapper();
            mapper.BuildClassMap<Dto, SubDomainModel>();
            mapper.BuildClassMap<Dto, DomainModel>().For(t => t.P3, o => o.From(x => x.P2, y => y + "Hello"));

            // Act
            var model = mapper.Map<Dto, DomainModel>(dto);
            
            // Assert
          //  Assert.AreEqual("A", model.SubModel.P1);
          //  Assert.AreEqual("B", model.SubModel.P2);
            Assert.AreEqual("BHello", model.P3);
        }
    }
}