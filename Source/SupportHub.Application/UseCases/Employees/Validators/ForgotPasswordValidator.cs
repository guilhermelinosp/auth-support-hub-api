﻿using FluentValidation;
using SupportHub.Domain.DTOs.Requests;
using SupportHub.Domain.Exceptions;

namespace SupportHub.Application.UseCases.Employees.Validators;

public class ForgotPasswordValidator : AbstractValidator<RequestForgotPassword>
{
	public ForgotPasswordValidator()
	{
		RuleFor(e => e.Email)
			.NotEmpty()
			.WithMessage(MessagesException.EMAIL_NAO_INFORMADO)
			.EmailAddress()
			.WithMessage(MessagesException.EMAIL_INVALIDO);
	}
}