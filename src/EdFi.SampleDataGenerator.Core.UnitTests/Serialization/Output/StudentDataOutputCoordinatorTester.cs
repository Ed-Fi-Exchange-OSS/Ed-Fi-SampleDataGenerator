using System;
using EdFi.SampleDataGenerator.Core.Helpers;
using EdFi.SampleDataGenerator.Core.Serialization.Output;
using EdFi.SampleDataGenerator.Core.UnitTests.Config;
using EdFi.SampleDataGenerator.Core.UnitTests.DataGeneration;
using FakeItEasy;
using NUnit.Framework;
using Shouldly;

namespace EdFi.SampleDataGenerator.Core.UnitTests.Serialization.Output
{
    [TestFixture]
    public class StudentDataOutputCoordinatorTester : GeneratorTestBase
    {
        [Test]
        public void ShouldCallFactoryToCreateNewOutputService()
        {
            var callToFactoryCount = 0;
            var factoryFunc = new Func<IStudentDataOutputService>(() =>
            {
                ++callToFactoryCount;
                return MockStudentOutputService();
            });
            var sut = new StudentDataOutputCoordinator(factoryFunc);

            var sampleDataGeneratorConfig = GetSampleDataGeneratorConfig();
            sut.Configure(sampleDataGeneratorConfig);
            sut.WriteToOutput(new GeneratedStudentData(), TestSchoolProfile.Default, TestDataPeriod.Default, 0);

            callToFactoryCount.ShouldBe(1);
        }

        [Test]
        public void ShouldConfigureOutputServiceOnCreation()
        {
            StudentDataOutputConfiguration studentDataOutputConfiguration = null;

            var studentDataOutputService = A.Fake<IStudentDataOutputService>();

            Func<IStudentDataOutputService> outputServiceFactory = () => studentDataOutputService;
            var sut = new StudentDataOutputCoordinator(outputServiceFactory);

            var sampleDataGeneratorConfig = GetSampleDataGeneratorConfig();
            sut.Configure(sampleDataGeneratorConfig);

            sut.WriteToOutput(new GeneratedStudentData(), TestSchoolProfile.Default, TestDataPeriod.Default, 0);
            studentDataOutputConfiguration.ShouldNotBeNull();
        }

        [Test]
        public void ShouldCreateNewOutputServiceForNewSchoolProfile()
        {
            StudentDataOutputConfiguration studentDataOutputConfiguration = null;
            var studentDataOutputService = MockStudentOutputService();

            StudentDataOutputConfiguration otherStudentDataOutputConfiguration = null;
            var otherStudentDataOutputService = MockStudentOutputService();

            int factoryCallCount = 0;
            Func<IStudentDataOutputService> outputServiceFactory = () => ++factoryCallCount % 2 == 1
                ? studentDataOutputService
                : otherStudentDataOutputService;

            var sut = new StudentDataOutputCoordinator(outputServiceFactory);

            var sampleDataGeneratorConfig = GetSampleDataGeneratorConfig();
            sut.Configure(sampleDataGeneratorConfig);

            var testSchoolProfile1 = new TestSchoolProfile { SchoolName = "Test School 1", InitialStudentCount = 1 };
            sut.WriteToOutput(new GeneratedStudentData(), testSchoolProfile1, TestDataPeriod.Default, 0);
            studentDataOutputConfiguration.ShouldNotBeNull();
            studentDataOutputConfiguration.SchoolProfile.SchoolName.ShouldBe(testSchoolProfile1.SchoolName);


            var testSchoolProfile2 = new TestSchoolProfile { SchoolName = "Test School 2", InitialStudentCount = 1 };
            sut.WriteToOutput(new GeneratedStudentData(), testSchoolProfile2, TestDataPeriod.Default, 0);
            otherStudentDataOutputConfiguration.ShouldNotBeNull();
            otherStudentDataOutputConfiguration.SchoolProfile.SchoolName.ShouldBe(testSchoolProfile2.SchoolName);
        }

        [Test]
        public void ShouldCreateNewOutputServiceForNewDataPeriod()
        {
            StudentDataOutputConfiguration studentDataOutputConfiguration = null;
            var studentDataOutputService = MockStudentOutputService();

            StudentDataOutputConfiguration otherStudentDataOutputConfiguration = null;
            var otherStudentDataOutputService = MockStudentOutputService();

            int factoryCallCount = 0;
            Func<IStudentDataOutputService> outputServiceFactory = () => ++factoryCallCount % 2 == 1 
                ? studentDataOutputService 
                : otherStudentDataOutputService;

            var sut = new StudentDataOutputCoordinator(outputServiceFactory);

            var sampleDataGeneratorConfig = GetSampleDataGeneratorConfig();
            sut.Configure(sampleDataGeneratorConfig);

            var testDataPeriod1 = new TestDataPeriod
            {
                Name = "Test Data Period 1",
                StartDate = new DateTime(),
                EndDate = new DateTime()
            };

            sut.WriteToOutput(new GeneratedStudentData(), TestSchoolProfile.Default, testDataPeriod1, 0);
            studentDataOutputConfiguration.ShouldNotBeNull();
            studentDataOutputConfiguration.DataPeriod.Name.ShouldBe(testDataPeriod1.Name);


            var testDataPeriod2 = new TestDataPeriod
            {
                Name = "Test Data Period 2",
                StartDate = new DateTime(),
                EndDate = new DateTime()
            };

            sut.WriteToOutput(new GeneratedStudentData(), TestSchoolProfile.Default, testDataPeriod2, 0);
            otherStudentDataOutputConfiguration.ShouldNotBeNull();
            otherStudentDataOutputConfiguration.DataPeriod.Name.ShouldBe(testDataPeriod2.Name);
        }

        [Test]
        public void ShouldFlushOutputOnCallToFinalizeOutput()
        {
            var studentDataOutputService = A.Fake<IStudentDataOutputService>();

            Func<IStudentDataOutputService> outputServiceFactory = () => studentDataOutputService;
            var sut = new StudentDataOutputCoordinator(outputServiceFactory);

            var sampleDataGeneratorConfig = GetSampleDataGeneratorConfig();
            sut.Configure(sampleDataGeneratorConfig);

            sut.WriteToOutput(new GeneratedStudentData(), TestSchoolProfile.Default, TestDataPeriod.Default, 0);
            sut.FinalizeOutput(TestSchoolProfile.Default, TestDataPeriod.Default.Yield());
        }

        private IStudentDataOutputService MockStudentOutputService()
        {
            return A.Fake<IStudentDataOutputService>();
        }
    }
}
