using Loans.Domain.Applications;
using NUnit.Framework;
using Moq;
using Moq.Protected;
using System.Collections.Generic;
using System;

namespace Loans.Tests
{
    public class LoanApplicationProcessorShould
    {
        [Test]
        public void DeclineLowSalary()
        {
            LoanProduct product = new LoanProduct(99, "Loan", 5.25m);
            LoanAmount amount = new LoanAmount("USD",200_000);

            var application = new LoanApplication(42,
                                                    product,
                                                    amount,
                                                    "Sarah",
                                                    25,
                                                    "133 Pluralsight Drive, Draper, Utah",
                                                    64_999
                );

            var mockIdentifyVerifier = new Mock<IIdentityVerifier>();
            var mockCreditScorer = new Mock<ICreditScorer>();

            var sut = new LoanApplicationProcessor(mockIdentifyVerifier.Object, mockCreditScorer.Object);

            sut.Process(application);

            Assert.That(application.GetIsAccepted(), Is.False);

        }

        delegate void ValidateCallback(string applicationName,
                                        int applicantAge,
                                        string applicantAddress,
                                        ref IdentityVerificationStatus status);

        [Test]
        public void Accept()
        {
            LoanProduct product = new LoanProduct(99, "Loan", 5.25m);
            LoanAmount amount = new LoanAmount("USD", 200_000);

            var application = new LoanApplication(42,
                                                    product,
                                                    amount,
                                                    "Sarah",
                                                    25,
                                                    "133 Pluralsight Drive, Draper, Utah",
                                                    65_000
                );

            var mockIdentifyVerifier = new Mock<IIdentityVerifier>(MockBehavior.Strict);

            mockIdentifyVerifier.Setup(x => x.Initialize());

            mockIdentifyVerifier.Setup(x => x.Validate(
                "Sarah",
                25,
                "133 Pluralsight Drive, Draper, Utah"))
                .Returns(true);

            //bool isValidOutValue = true;
            //mockIdentifyVerifier.Setup(x => x.Validate(It.IsAny<string>(), 
            //                                            It.IsAny<int>(), 
            //                                            It.IsAny<string>(), 
            //                                            out isValidOutValue));

            //mockIdentifyVerifier
            //    .Setup(x => x.Validate("Sarah",
            //                            25,
            //                            "133 Pluralsight Drive, Draper, Utah",
            //                            ref It.Ref<IdentityVerificationStatus>.IsAny))
            //    .Callback(new ValidateCallback(
            //            (string applicationName,
            //            int applicantAge,
            //            string applicantAddress,
            //            ref IdentityVerificationStatus status) => 
            //            status = new IdentityVerificationStatus(true)
            //        ))
            //    ;

            var mockCreditScorer = new Mock<ICreditScorer>();

            var mockScoreValue = new Mock<ScoreValue>();
            mockScoreValue.Setup(x => x.Score).Returns(300);

            var mockScoreResult = new Mock<ScoreResult> { DefaultValue = DefaultValue.Mock };

            mockCreditScorer.SetupAllProperties();

            mockCreditScorer.Setup(x => x.ScoreResult.ScoreValue.Score).Returns(300);            

            //mockCreditScorer.SetupProperty(x => x.Count);

            //mockScoreResult.Setup(x => x.ScoreValue).Returns(mockScoreValue.Object);

            //mockCreditScorer.Setup(x => x.ScoreResult).Returns(mockScoreResult.Object);

            //mockCreditScorer.Setup(x => x.Score).Returns(300);

            var sut = new LoanApplicationProcessor(mockIdentifyVerifier.Object, mockCreditScorer.Object);

            sut.Process(application);

            mockCreditScorer.VerifyGet(x => x.ScoreResult.ScoreValue.Score, Times.Once);
            mockCreditScorer.VerifySet(x => x.Count = It.IsAny<int>(), Times.Once);

            Assert.That(application.GetIsAccepted(), Is.True);
            Assert.That(mockCreditScorer.Object.Count, Is.EqualTo(1));

        }

        [Test]
        public void InitializeIdentifyVerifier()
        {
            LoanProduct product = new LoanProduct(99, "Loan", 5.25m);
            LoanAmount amount = new LoanAmount("USD", 200_000);

            var application = new LoanApplication(42,
                                                    product,
                                                    amount,
                                                    "Sarah",
                                                    25,
                                                    "133 Pluralsight Drive, Draper, Utah",
                                                    65_000
                );

            var mockIdentifyVerifier = new Mock<IIdentityVerifier>();
            mockIdentifyVerifier.Setup(x => x.Validate(
                "Sarah",
                25,
                "133 Pluralsight Drive, Draper, Utah"))
                .Returns(true);

            var mockCreditScorer = new Mock<ICreditScorer>();

            var mockScoreValue = new Mock<ScoreValue>();
            mockScoreValue.Setup(x => x.Score).Returns(300);

            var mockScoreResult = new Mock<ScoreResult> { DefaultValue = DefaultValue.Mock };

            mockCreditScorer.SetupAllProperties();

            mockCreditScorer.Setup(x => x.ScoreResult.ScoreValue.Score).Returns(300);            

            var sut = new LoanApplicationProcessor(mockIdentifyVerifier.Object, mockCreditScorer.Object);

            sut.Process(application);

            mockIdentifyVerifier.Verify(x => x.Initialize());

            mockIdentifyVerifier.Verify(x => x.Validate(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()));

            mockIdentifyVerifier.VerifyNoOtherCalls();


        }

        [Test]
        public void Calculate()
        {
            LoanProduct product = new LoanProduct(99, "Loan", 5.25m);
            LoanAmount amount = new LoanAmount("USD", 200_000);

            var application = new LoanApplication(42,
                                                    product,
                                                    amount,
                                                    "Sarah",
                                                    25,
                                                    "133 Pluralsight Drive, Draper, Utah",
                                                    65_000
                );

            var mockIdentifyVerifier = new Mock<IIdentityVerifier>();
            mockIdentifyVerifier.Setup(x => x.Validate(
                "Sarah",
                25,
                "133 Pluralsight Drive, Draper, Utah"))
                .Returns(true);

            var mockCreditScorer = new Mock<ICreditScorer>();

            var mockScoreValue = new Mock<ScoreValue>();
            mockScoreValue.Setup(x => x.Score).Returns(300);

            var mockScoreResult = new Mock<ScoreResult> { DefaultValue = DefaultValue.Mock };

            mockCreditScorer.SetupAllProperties();

            mockCreditScorer.Setup(x => x.ScoreResult.ScoreValue.Score).Returns(300);

            var sut = new LoanApplicationProcessor(mockIdentifyVerifier.Object, mockCreditScorer.Object);

            sut.Process(application);

            //mockCreditScorer.Verify(x => x.CalculateScore("Sarah", "133 Pluralsight Drive, Draper, Utah"));

            //mockCreditScorer.Verify(x => x.CalculateScore(It.IsAny<string>(), It.IsAny<string>()));

            mockCreditScorer.Verify(x => x.CalculateScore("Sarah", 
                    "133 Pluralsight Drive, Draper, Utah"
                    ), Times.Once);
        }


        [Test]
        public void DeclineWhenCreditScoreError()
        {
            LoanProduct product = new LoanProduct(99, "Loan", 5.25m);
            LoanAmount amount = new LoanAmount("USD", 200_000);

            var application = new LoanApplication(42,
                                                    product,
                                                    amount,
                                                    "Sarah",
                                                    25,
                                                    "133 Pluralsight Drive, Draper, Utah",
                                                    65_000
                );

            var mockIdentifyVerifier = new Mock<IIdentityVerifier>();            

            mockIdentifyVerifier.Setup(x => x.Validate(
                "Sarah",
                25,
                "133 Pluralsight Drive, Draper, Utah"))
                .Returns(true);

            var mockCreditScorer = new Mock<ICreditScorer>();

            mockCreditScorer.Setup(x => x.ScoreResult.ScoreValue.Score).Returns(300);

            mockCreditScorer.Setup(x => x.CalculateScore(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new InvalidOperationException("Test Exception"));

            var sut = new LoanApplicationProcessor(mockIdentifyVerifier.Object, mockCreditScorer.Object);

            sut.Process(application);

            Assert.That(application.GetIsAccepted(), Is.False);            
        }

        interface IIdentityVerifierServiceGatewayProtectedMembers
        {
            DateTime GetCurrentTime();
            bool CallService(string applicantName, int applicantAge, string applicantAddress);
        }

        [Test]
        public void AcceptsUsingPartial()
        {
            LoanProduct product = new LoanProduct(99, "Loan", 5.25m);
            LoanAmount amount = new LoanAmount("USD", 200_000);

            var application = new LoanApplication(42,
                                                    product,
                                                    amount,
                                                    "Sarah",
                                                    25,
                                                    "133 Pluralsight Drive, Draper, Utah",
                                                    65_000
                );

            var mockIdentifyVerifier = new Mock<IdentityVerifierServiceGateway>();

            mockIdentifyVerifier.Protected()
                .As<IIdentityVerifierServiceGatewayProtectedMembers>()
                .Setup(x => x.CallService(It.IsAny<string>(),
                                            It.IsAny<int>(),
                                            It.IsAny<string>()))
                .Returns(true);

            //mockIdentifyVerifier.Protected().Setup<bool>("CallService",
            //    "Sarah",
            //    25,
            //    "133 Pluralsight Drive, Draper, Utah")
            //    .Returns(true);

            //mockIdentifyVerifier.Setup(x => x.CallService(
            //    "Sarah",
            //    25,
            //    "133 Pluralsight Drive, Draper, Utah"))
            //    .Returns(true);

            var expectedTime = new DateTime(2000, 1, 1);

            //mockIdentifyVerifier.Setup(x => x.GetCurrentTime()).Returns(expectedTime);

            //mockIdentifyVerifier.Protected().Setup<DateTime>("GetCurrentTime").Returns(expectedTime);

            mockIdentifyVerifier.Protected()
                .As<IIdentityVerifierServiceGatewayProtectedMembers>()
                .Setup(x => x.GetCurrentTime())
                .Returns(expectedTime);

            var mockCreditScorer = new Mock<ICreditScorer>();

            mockCreditScorer.Setup(x => x.ScoreResult.ScoreValue.Score).Returns(300);            

            var sut = new LoanApplicationProcessor(mockIdentifyVerifier.Object, mockCreditScorer.Object);

            sut.Process(application);

            Assert.That(application.GetIsAccepted(), Is.True);
            Assert.That(mockIdentifyVerifier.Object.LastCheckTime, Is.EqualTo(expectedTime));
        }

        [Test]
        public void AcceptsUsingPartialWithDependencies()
        {
            LoanProduct product = new LoanProduct(99, "Loan", 5.25m);
            LoanAmount amount = new LoanAmount("USD", 200_000);

            var application = new LoanApplication(42,
                                                    product,
                                                    amount,
                                                    "Sarah",
                                                    25,
                                                    "133 Pluralsight Drive, Draper, Utah",
                                                    65_000
                );

            var expectedTime = new DateTime(2000, 1, 1);

            var mockNowProvider = new Mock<INowProvider>();
            mockNowProvider.Setup(x => x.GetNow()).Returns(expectedTime);

            var mockIdentifyVerifier = new Mock<IdentityVerifierServiceGateway>(mockNowProvider.Object);

            mockIdentifyVerifier.Protected()
                .As<IIdentityVerifierServiceGatewayProtectedMembers>()
                .Setup(x => x.CallService(It.IsAny<string>(),
                                            It.IsAny<int>(),
                                            It.IsAny<string>()))
                .Returns(true);           

            var mockCreditScorer = new Mock<ICreditScorer>();

            mockCreditScorer.Setup(x => x.ScoreResult.ScoreValue.Score).Returns(300);

            var sut = new LoanApplicationProcessor(mockIdentifyVerifier.Object, mockCreditScorer.Object);

            sut.Process(application);

            Assert.That(application.GetIsAccepted(), Is.True);
            Assert.That(mockIdentifyVerifier.Object.LastCheckTime, Is.EqualTo(expectedTime));
        }


        [Test]
        public void NullReturnExample()
        {
            var mock = new Mock<INullExample>();

            mock.Setup(x => x.SomeMethod())
                .Returns<string>(null);
            string mockReturnValue = mock.Object.SomeMethod();
            Assert.That(mockReturnValue, Is.Null);
        }

    }

    public interface INullExample
    {
        string SomeMethod();
    }
}
