using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EdFi.SampleDataGenerator.Core.AutoMapper;
using EdFi.SampleDataGenerator.Core.Config;
using EdFi.SampleDataGenerator.Core.Config.SeedData;
using EdFi.SampleDataGenerator.Core.DataGeneration.Common;
using EdFi.SampleDataGenerator.Core.DataGeneration.Common.Mutators;
using EdFi.SampleDataGenerator.Core.DataGeneration.Coordination;
using EdFi.SampleDataGenerator.Core.DataGeneration.Generators;
using EdFi.SampleDataGenerator.Core.DataGeneration.Generators.Student;
using EdFi.SampleDataGenerator.Core.DataGeneration.InterchangeEntities;
using EdFi.SampleDataGenerator.Core.DataGeneration.Mutators;
using EdFi.SampleDataGenerator.Core.Entities;
using EdFi.SampleDataGenerator.Core.Helpers;
using EdFi.SampleDataGenerator.Core.Serialization.Output;
using EdFi.SampleDataGenerator.Core.UnitTests.Config;
using FakeItEasy;
using NUnit.Framework;
using Shouldly;
namespace EdFi.SampleDataGenerator.Core.UnitTests.DataGeneration.Coordination
{
    [TestFixture]
    public class StudentDataGenerationCoordinatorTester : GeneratorTestBase
    {
        [Test]
        public void ShouldSuccessfullyGenerateStudentData()
        {
            var randomNumberGenerator = new RandomNumberGenerator();
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperCoreProfile>()));
            GeneratedStudentData generatedGeneratedStudentData = null;
            var studentDataOutputCoordinator = A.Fake<IStudentDataOutputCoordinator>();
            var seedOutputService = new SeedDataOutputService();
            var mutationLogOutputService = A.Fake<IBufferedEntityOutputService<MutationLogEntry, MutationLogOutputConfiguration>>();
            
            
            var studentDataGenerationCoordinator = new StudentDataGenerationCoordinator
            (
                studentDataOutputCoordinator,
                seedOutputService,
                mutationLogOutputService,
                randomNumberGenerator,
                mapper,
                StudentDataGenerationCoordinator.DefaultGeneratorFactory,
                MutatorFactory.StudentMutatorFactory
            );
            var sampleDataGeneratorConfig = GetSampleDataGeneratorConfig();
            var globalDataGeneratorConfig = GetGlobalDataGeneratorConfig();
            var globalData = GetGlobalDataGeneratorContext(globalDataGeneratorConfig);
            var studentDataGeneratorConfig = GetStudentGeneratorConfig(globalData.GlobalData, sampleDataGeneratorConfig, globalDataGeneratorConfig);
            studentDataGenerationCoordinator.Configure(studentDataGeneratorConfig);
            studentDataGenerationCoordinator.Run(globalDataGeneratorConfig, globalData);
            generatedGeneratedStudentData.ShouldNotBeNull();
        }
        [Test]
        public void ShouldSuccessfullyGenerateSeedRecords()
        {
            var randomNumberGenerator = new RandomNumberGenerator();
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperCoreProfile>()));
            var studentDataOutputCoordinator = A.Fake<IStudentDataOutputCoordinator>();
            SeedRecord generatedSeedRecord = null;
            var seedOutputService = A.Fake<IBufferedEntityOutputService<SeedRecord, ISampleDataGeneratorConfig>>();
            
            var mutationLogOutputService = A.Fake<IBufferedEntityOutputService<MutationLogEntry, MutationLogOutputConfiguration>>();
            
            
            var studentDataGenerationCoordinator = new StudentDataGenerationCoordinator
            (
                studentDataOutputCoordinator,
                seedOutputService,
                mutationLogOutputService,
                randomNumberGenerator,
                mapper,
                StudentDataGenerationCoordinator.DefaultGeneratorFactory,
                MutatorFactory.StudentMutatorFactory
            );
            var sampleDataGeneratorConfig = GetSampleDataGeneratorConfig();
            sampleDataGeneratorConfig.OutputMode = OutputMode.Seed;
            sampleDataGeneratorConfig.SeedFilePath = "./Test.csv";
            var globalDataGeneratorConfig = GetGlobalDataGeneratorConfig(sampleDataGeneratorConfig);
            var globalData = GetGlobalDataGeneratorContext(globalDataGeneratorConfig);
            var studentDataGeneratorConfig = GetStudentGeneratorConfig(globalData.GlobalData, sampleDataGeneratorConfig, globalDataGeneratorConfig);
            studentDataGeneratorConfig.GlobalConfig = sampleDataGeneratorConfig;
            globalDataGeneratorConfig.GlobalConfig = sampleDataGeneratorConfig;
            studentDataGenerationCoordinator.Configure(studentDataGeneratorConfig);
            studentDataGenerationCoordinator.Run(globalDataGeneratorConfig, globalData);
            generatedSeedRecord.ShouldNotBeNull();
        }
        [Test]
        public void ShouldUseSeedDataWhenPresent()
        {
            var randomNumberGenerator = new RandomNumberGenerator();
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperCoreProfile>()));
            GeneratedStudentData generatedGeneratedStudentData = null;
            var studentDataOutputCoordinator = A.Fake<IStudentDataOutputCoordinator>();
            var seedOutputService = A.Fake<IBufferedEntityOutputService<SeedRecord, ISampleDataGeneratorConfig>>();
            
            
            var mutationLogOutputService = A.Fake<IBufferedEntityOutputService<MutationLogEntry, MutationLogOutputConfiguration>>();
            
            
            var studentDataGenerationCoordinator = new StudentDataGenerationCoordinator
            (
                studentDataOutputCoordinator,
                seedOutputService,
                mutationLogOutputService,
                randomNumberGenerator,
                mapper,
                StudentDataGenerationCoordinator.DefaultGeneratorFactory,
                MutatorFactory.StudentMutatorFactory
            );
            var birthDate = new DateTime(2017, 11, 15);
            var seedRecord = new SeedRecord
            {
                FirstName = "First",
                MiddleName = "Middle",
                LastName = "Last",
                BirthDate = birthDate,
                Gender = SexDescriptor.Male,
                GradeLevel = GradeLevelDescriptor.FirstGrade,
                Race = RaceDescriptor.NativeHawaiianPacificIslander,
                HispanicLatinoEthnicity = true,
                PerformanceIndex = 0.5,
                SchoolId = TestSchoolProfile.Default.SchoolId
            };
            var sampleDataGeneratorConfig = GetSampleDataGeneratorConfig();
            var globalDataGeneratorConfig = GetGlobalDataGeneratorConfig();
            globalDataGeneratorConfig.SeedRecords = new List<SeedRecord> { seedRecord };
            var globalData = GetGlobalDataGeneratorContext(globalDataGeneratorConfig);
            var studentDataGeneratorConfig = GetStudentGeneratorConfig(globalData.GlobalData, sampleDataGeneratorConfig, globalDataGeneratorConfig);
            studentDataGenerationCoordinator.Configure(studentDataGeneratorConfig);
            studentDataGenerationCoordinator.Run(globalDataGeneratorConfig, globalData);
            var student = generatedGeneratedStudentData?.StudentData?.Student;
            student.ShouldNotBeNull();
            var studentEdA = generatedGeneratedStudentData?.StudentEnrollmentData
                ?.StudentEducationOrganizationAssociation?.FirstOrDefault();
            studentEdA.ShouldNotBeNull();
            student.Name.FirstName.ShouldBe("First");
            student.Name.MiddleName.ShouldBe("Middle");
            student.Name.LastSurname.ShouldBe("Last");
            student.BirthData.BirthDate.ShouldBe(birthDate);
            studentEdA.Sex.ShouldBe(SexDescriptor.Male.GetStructuredCodeValue());
            studentEdA.Race.First().ShouldBe(RaceDescriptor.NativeHawaiianPacificIslander.GetStructuredCodeValue());
            studentEdA.HispanicLatinoEthnicity.ShouldBeTrue();
            var defaultSchoolReference = TestSchoolProfile.Default.GetSchoolReference();
            generatedGeneratedStudentData.StudentEnrollmentData.StudentSchoolAssociation.SchoolReference.ReferencesSameSchoolAs(defaultSchoolReference).ShouldBeTrue();
        }
        [Test]
        public void ShouldGenerateOutputForStudentDataOnStandardOutputMode()
        {
            var randomNumberGenerator = new RandomNumberGenerator();
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperCoreProfile>()));
            var seedDataSerializationService = A.Fake<ISeedDataSerializationService>();
            var mutationLogOutputService = A.Fake<IBufferedEntityOutputService<MutationLogEntry, MutationLogOutputConfiguration>>();
            
            
            var studentDataOutputCoordinator = A.Fake<IStudentDataOutputCoordinator>();
            var seedOutputService = new SeedDataOutputService(seedDataSerializationService);
            var studentDataGenerationCoordinator = new StudentDataGenerationCoordinator
            (
                studentDataOutputCoordinator,
                seedOutputService,
                mutationLogOutputService,
                randomNumberGenerator,
                mapper,
                StudentDataGenerationCoordinator.DefaultGeneratorFactory,
                MutatorFactory.StudentMutatorFactory
            );
            var sampleDataGeneratorConfig = GetSampleDataGeneratorConfig();
            sampleDataGeneratorConfig.OutputMode = OutputMode.Standard;
            var globalDataGeneratorConfig = GetGlobalDataGeneratorConfig();
            var globalData = GetGlobalDataGeneratorContext(globalDataGeneratorConfig);
            var studentDataGeneratorConfig = GetStudentGeneratorConfig(globalData.GlobalData, sampleDataGeneratorConfig, globalDataGeneratorConfig);
            studentDataGenerationCoordinator.Configure(studentDataGeneratorConfig);
            studentDataGenerationCoordinator.Run(globalDataGeneratorConfig, globalData);
            A.CallTo(() => seedDataSerializationService.Write(A<ISampleDataGeneratorConfig>._, A<IEnumerable<SeedRecord>>._)).MustNotHaveHappened();
        }
        [Test]
        public void ShouldGenerateOutputForSeedRecordsOnSeedOutputMode()
        {
            var randomNumberGenerator = new RandomNumberGenerator();
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperCoreProfile>()));
            var seedDataSerializationService = A.Fake<ISeedDataSerializationService>();
            var mutationLogOutputService = A.Fake<IBufferedEntityOutputService<MutationLogEntry, MutationLogOutputConfiguration>>();
            
            
            var studentDataOutputCoordinator = A.Fake<IStudentDataOutputCoordinator>();
            var seedOutputService = new SeedDataOutputService(seedDataSerializationService);
            var studentDataGenerationCoordinator = new StudentDataGenerationCoordinator
            (
                studentDataOutputCoordinator,
                seedOutputService,
                mutationLogOutputService,
                randomNumberGenerator,
                mapper,
                StudentDataGenerationCoordinator.DefaultGeneratorFactory,
                MutatorFactory.StudentMutatorFactory
            );
            var sampleDataGeneratorConfig = GetSampleDataGeneratorConfig();
            sampleDataGeneratorConfig.OutputMode = OutputMode.Seed;
            sampleDataGeneratorConfig.SeedFilePath = "./Test.csv";
            var globalDataGeneratorConfig = GetGlobalDataGeneratorConfig(sampleDataGeneratorConfig);
            var globalData = GetGlobalDataGeneratorContext(globalDataGeneratorConfig);
            var studentDataGeneratorConfig = GetStudentGeneratorConfig(globalData.GlobalData, sampleDataGeneratorConfig, globalDataGeneratorConfig);
            studentDataGenerationCoordinator.Configure(studentDataGeneratorConfig);
            studentDataGenerationCoordinator.Run(globalDataGeneratorConfig, globalData);
            A.CallTo(() => seedDataSerializationService.Write(A<ISampleDataGeneratorConfig>._, A<IEnumerable<SeedRecord>>._)).MustHaveHappenedOnceExactly();
        }
        [Test]
        public void ShouldNotRunMutatorsWithOnlyOneDataPeriod()
        {
            var randomNumberGenerator = new RandomNumberGenerator();
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperCoreProfile>()));
            GeneratedStudentData generatedGeneratedStudentData = null;
            var studentDataOutputCoordinator = A.Fake<IStudentDataOutputCoordinator>();
            var seedOutputService = A.Fake<IBufferedEntityOutputService<SeedRecord, ISampleDataGeneratorConfig>>();
            
            
            var mutationLogOutputService = A.Fake<IBufferedEntityOutputService<MutationLogEntry, MutationLogOutputConfiguration>>();
            
            
            var mutator = A.Fake<IStudentMutator>();
            StudentDataGenerationCoordinator.MutatorFactoryDelegate mutatorFactory = rng => mutator.Yield();
            var studentDataGenerationCoordinator = new StudentDataGenerationCoordinator
            (
                studentDataOutputCoordinator,
                seedOutputService,
                mutationLogOutputService,
                randomNumberGenerator,
                mapper,
                StudentDataGenerationCoordinator.DefaultGeneratorFactory,
                mutatorFactory
            );
            var sampleDataGeneratorConfig = GetSampleDataGeneratorConfig();
            var globalDataGeneratorConfig = GetGlobalDataGeneratorConfig(sampleDataGeneratorConfig);
            sampleDataGeneratorConfig.TimeConfig.DataClockConfig.DataPeriods.Count().ShouldBe(1);
            var globalData = GetGlobalDataGeneratorContext(globalDataGeneratorConfig);
            var studentDataGeneratorConfig = GetStudentGeneratorConfig(globalData.GlobalData, sampleDataGeneratorConfig, globalDataGeneratorConfig);
            studentDataGenerationCoordinator.Configure(studentDataGeneratorConfig);
            studentDataGenerationCoordinator.Run(globalDataGeneratorConfig, globalData);
            generatedGeneratedStudentData.ShouldNotBeNull();
        }
        [Test]
        public void ShouldRunMutatorsOnAllDataPeriodsAfterFirst()
        {
            var randomNumberGenerator = new RandomNumberGenerator();
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperCoreProfile>()));
            GeneratedStudentData generatedGeneratedStudentData = null;
            var studentDataOutputCoordinator = A.Fake<IStudentDataOutputCoordinator>();
            var seedOutputService = A.Fake<IBufferedEntityOutputService<SeedRecord, ISampleDataGeneratorConfig>>();
            
            
            var mutationLogOutputService = A.Fake<IBufferedEntityOutputService<MutationLogEntry, MutationLogOutputConfiguration>>();
            
            
            var mutator = A.Fake<IStudentMutator>();
            
            StudentDataGenerationCoordinator.MutatorFactoryDelegate mutatorFactory = rng => mutator.Yield();
            var studentDataGenerationCoordinator = new StudentDataGenerationCoordinator
            (
                studentDataOutputCoordinator,
                seedOutputService,
                mutationLogOutputService,
                randomNumberGenerator,
                mapper,
                StudentDataGenerationCoordinator.DefaultGeneratorFactory,
                mutatorFactory
            );
            var sampleDataGeneratorConfig = GetSampleDataGeneratorConfig();
            var globalDataGeneratorConfig = GetGlobalDataGeneratorConfig(sampleDataGeneratorConfig);
            var timeConfig = TestTimeConfig.Default;
            timeConfig.DataClockConfig = new TestDataClockConfig
            {
                DataPeriods = new List<TestDataPeriod>
                {
                    new TestDataPeriod { StartDate = new DateTime(2016, 8, 23), EndDate = new DateTime(2016, 8, 24), Name = "Test1"},
                    new TestDataPeriod { StartDate = new DateTime(2016, 8, 25), EndDate = new DateTime(2016, 8, 26), Name = "Test2"},
                    new TestDataPeriod { StartDate = new DateTime(2016, 8, 27), EndDate = new DateTime(2016, 8, 28), Name = "Test3"},
                },
                StartDate = new DateTime(2016, 8, 23),
                EndDate = new DateTime(2016, 8, 28)
            };
            sampleDataGeneratorConfig.TimeConfig = timeConfig;
            var dataPeriod1 = timeConfig.DataClockConfig.DataPeriods.ToList()[0];
            var dataPeriod2 = timeConfig.DataClockConfig.DataPeriods.ToList()[1];
            globalDataGeneratorConfig.GlobalConfig = sampleDataGeneratorConfig;
            var globalData = GetGlobalDataGeneratorContext(globalDataGeneratorConfig);
            var studentDataGeneratorConfig = GetStudentGeneratorConfig(globalData.GlobalData, sampleDataGeneratorConfig, globalDataGeneratorConfig);
            studentDataGenerationCoordinator.Configure(studentDataGeneratorConfig);
            studentDataGenerationCoordinator.Run(globalDataGeneratorConfig, globalData);
            generatedGeneratedStudentData.ShouldNotBeNull();
        }
        [Test]
        public void ShouldLogMutations()
        {
            var randomNumberGenerator = new RandomNumberGenerator();
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperCoreProfile>()));
            GeneratedStudentData generatedGeneratedStudentData = null;
            var studentDataOutputCoordinator = A.Fake<IStudentDataOutputCoordinator>();
            var seedOutputService = A.Fake<IBufferedEntityOutputService<SeedRecord, ISampleDataGeneratorConfig>>();
            
            
            var mutationLogOutputService = A.Fake<IBufferedEntityOutputService<MutationLogEntry, MutationLogOutputConfiguration>>();
            
            
            var mutator = A.Fake<IStudentMutator>();
            mutator.Setup(x => x.InterchangeEntity).Returns(InterchangeEntity.Student);
            mutator.Setup(x => x.Entity).Returns(StudentEntity.Student);
            mutator.Setup(x => x.EntityField).Returns(StudentField.Name);
            StudentDataGenerationCoordinator.MutatorFactoryDelegate mutatorFactory = rng => mutator.Yield();
            var studentDataGenerationCoordinator = new StudentDataGenerationCoordinator
            (
                studentDataOutputCoordinator,
                seedOutputService,
                mutationLogOutputService,
                randomNumberGenerator,
                mapper,
                StudentDataGenerationCoordinator.DefaultGeneratorFactory,
                mutatorFactory
            );
            var sampleDataGeneratorConfig = GetSampleDataGeneratorConfig();
            var globalDataGeneratorConfig = GetGlobalDataGeneratorConfig(sampleDataGeneratorConfig);
            var timeConfig = TestTimeConfig.Default;
            timeConfig.DataClockConfig = new TestDataClockConfig
            {
                DataPeriods = new List<TestDataPeriod>
                {
                    new TestDataPeriod { StartDate = new DateTime(2016, 8, 23), EndDate = new DateTime(2016, 8, 24), Name = "Test1"},
                    new TestDataPeriod { StartDate = new DateTime(2016, 8, 25), EndDate = new DateTime(2016, 8, 26), Name = "Test2"},
                    new TestDataPeriod { StartDate = new DateTime(2016, 8, 27), EndDate = new DateTime(2016, 8, 28), Name = "Test3"},
                },
                StartDate = new DateTime(2016, 8, 23),
                EndDate = new DateTime(2016, 8, 28)
            };
            sampleDataGeneratorConfig.TimeConfig = timeConfig;
            var globalData = GetGlobalDataGeneratorContext(globalDataGeneratorConfig);
            var studentDataGeneratorConfig = GetStudentGeneratorConfig(globalData.GlobalData, sampleDataGeneratorConfig, globalDataGeneratorConfig);
            studentDataGenerationCoordinator.Configure(studentDataGeneratorConfig);
            studentDataGenerationCoordinator.Run(globalDataGeneratorConfig, globalData);
            generatedGeneratedStudentData.ShouldNotBeNull();
        }
        [Test]
        public void ShouldGroupOutputBySchoolAndDataPeriod()
        {
            var randomNumberGenerator = new RandomNumberGenerator();
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperCoreProfile>()));
            var seedDataSerializationService = A.Fake<ISeedDataSerializationService>();
            var mutationLogOutputService = A.Fake<IBufferedEntityOutputService<MutationLogEntry, MutationLogOutputConfiguration>>();
            
            
            var studentDataOutputCoordinator = A.Fake<IStudentDataOutputCoordinator>();
            var seedOutputService = new SeedDataOutputService(seedDataSerializationService);
            var studentDataGenerationCoordinator = new StudentDataGenerationCoordinator
            (
                studentDataOutputCoordinator,
                seedOutputService,
                mutationLogOutputService,
                randomNumberGenerator,
                mapper,
                StudentDataGenerationCoordinator.DefaultGeneratorFactory,
                MutatorFactory.StudentMutatorFactory
            );
            var sampleDataGeneratorConfig = GetSampleDataGeneratorConfig();
            var firstSchoolProfile = TestSchoolProfile.GetSchoolProfile("Eastwood Elementary", 123456, new[] { GradeLevelDescriptor.FirstGrade, GradeLevelDescriptor.SecondGrade, }, 2);
            var secondSchoolProfile = TestSchoolProfile.GetSchoolProfile("Westwood Middle", 12346, new[] { GradeLevelDescriptor.ThirdGrade, GradeLevelDescriptor.FourthGrade, }, 2);
            sampleDataGeneratorConfig.DistrictProfiles = new IDistrictProfile[]
            {
                new TestDistrictProfile
                {
                    DistrictName = "Test District",
                    LocationInfo = TestLocationInfo.Default,
                    SchoolProfiles = new ISchoolProfile[]
                    {
                        firstSchoolProfile,
                        secondSchoolProfile,
                    }
                }
            };
            sampleDataGeneratorConfig.TimeConfig = new TestTimeConfig
            {
                DataClockConfig = new TestDataClockConfig
                {
                    StartDate = new DateTime(2016, 8, 22),
                    EndDate = new DateTime(2017, 5, 8),
                    DataPeriods = new IDataPeriod[]
                    {
                        new TestDataPeriod
                        {
                            Name = "2016-2017 Fall Term",
                            StartDate = new DateTime(2016, 8, 22),
                            EndDate = new DateTime(2016, 12, 31),
                        },
                        new TestDataPeriod
                        {
                            Name = "2016-2017 Spring Term",
                            StartDate = new DateTime(2017, 1, 1),
                            EndDate = new DateTime(2017, 5, 8),
                        }
                    }
                },
                SchoolCalendarConfig = TestSchoolCalendarConfig.Default
            };
            sampleDataGeneratorConfig.OutputMode = OutputMode.Standard;
            var globalDataGeneratorConfig = GetGlobalDataGeneratorConfig(sampleDataGeneratorConfig);
            var globalData = GetGlobalDataGeneratorContext(globalDataGeneratorConfig);
            var studentDataGeneratorConfig = GetStudentGeneratorConfig(globalData.GlobalData, sampleDataGeneratorConfig, globalDataGeneratorConfig);
            studentDataGenerationCoordinator.Configure(studentDataGeneratorConfig);
            studentDataGenerationCoordinator.Run(globalDataGeneratorConfig, globalData);
            A.CallTo(() => studentDataOutputCoordinator.FinalizeOutput(A<ISchoolProfile>._, A<IEnumerable<IDataPeriod>>._)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => studentDataOutputCoordinator.FinalizeOutput(A<ISchoolProfile>.That.Matches(sp => sp.SchoolId == firstSchoolProfile.SchoolId), A<IEnumerable<IDataPeriod>>.That.Matches(dp => dp.Count() == sampleDataGeneratorConfig.TimeConfig.DataClockConfig.DataPeriods.Count()))).MustHaveHappenedOnceExactly();
            A.CallTo(() => studentDataOutputCoordinator.FinalizeOutput(A<ISchoolProfile>.That.Matches(sp => sp.SchoolId == secondSchoolProfile.SchoolId), A<IEnumerable<IDataPeriod>>.That.Matches(dp => dp.Count() == sampleDataGeneratorConfig.TimeConfig.DataClockConfig.DataPeriods.Count()))).MustHaveHappenedOnceExactly();
        }
    }
}