﻿using SupportHub.Domain.DTOs.Requests.Companies;
using SupportHub.Domain.DTOs.Responses;

namespace SupportHub.Application.UseCases.Companies.SignIn;

public interface ISignInUseCase : IApplicationInjection
{
	Task<ResponseDefault> ExecuteAsync(RequestSignIn request);
}