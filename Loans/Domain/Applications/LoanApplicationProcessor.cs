﻿using System;

namespace Loans.Domain.Applications
{
    public class LoanApplicationProcessor
    {
        private const decimal MinimumSalary = 65_000;
        private const int MinimumAge = 18;
        private const int MinimumCreditScore = 300;

        private readonly IIdentityVerifier _identityVerifier;
        private readonly ICreditScorer _creditScorer;
        

        public LoanApplicationProcessor(IIdentityVerifier identityVerifier, 
                                        ICreditScorer creditScorer)
        {
            _identityVerifier = 
                identityVerifier ?? throw new ArgumentNullException(nameof(identityVerifier));

            _creditScorer = 
                creditScorer ?? throw new ArgumentNullException(nameof(creditScorer));
        }

        public void Process(LoanApplication application)
        {
            if (application.GetApplicantSalary() < MinimumSalary)
            {
                application.Decline();
                return;
            }

            if (application.GetApplicantAge() < MinimumAge)
            {
                application.Decline();
                return;
            }

            _identityVerifier.Initialize();

            var isValidIdentity = _identityVerifier.Validate(application.GetApplicantName(),
                                                             application.GetApplicantAge(),
                                                             application.GetApplicantAddress());

            if (!isValidIdentity)
            {
                application.Decline();
                return;
            }



            //_identityVerifier.Validate(application.GetApplicantName(),
            //                                                 application.GetApplicantAge(),
            //                                                 application.GetApplicantAddress(),
            //                                                 out var isValidIdentity);

            //IdentityVerificationStatus status = null;

            //_identityVerifier.Validate(application.GetApplicantName(),
            //                                                 application.GetApplicantAge(),
            //                                                 application.GetApplicantAddress(),
            //                                                 ref status);

            //if (!status.Passed)
            //{
            //    application.Decline();
            //    return;
            //}


            try
            {
                _creditScorer.CalculateScore(application.GetApplicantName(),
                                                 application.GetApplicantAddress());
            }
            catch (Exception)
            {
                application.Decline();
                return; 
            }


            //if (_creditScorer.Score < MinimumCreditScore)
            //{
            //    application.Decline();
            //    return;
            //}

            _creditScorer.Count++;

            if (_creditScorer.ScoreResult.ScoreValue.Score < MinimumCreditScore)
            //if(true)
            {
                application.Decline();
                return;
            }

            application.Accept();
        }
    }
}
