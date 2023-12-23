﻿using System.Text.RegularExpressions;
using Employee.SupportHub.Domain.DTOs.Requests;
using Employee.SupportHub.Domain.Exceptions;
using FluentValidation;
using FluentValidation.Results;

namespace Employee.SupportHub.Application.UseCases.Validators;

public partial class SignInValidator : AbstractValidator<RequestSignIn>
{
	public SignInValidator()
	{
		RuleFor(c => c.Password)
			.NotEmpty()
			.WithMessage(MessagesException.SENHA_NAO_INFORMADO)
			.MinimumLength(8)
			.WithMessage(MessagesException.SENHA_MINIMO_OITO_CARACTERES)
			.MaximumLength(16)
			.WithMessage(MessagesException.SENHA_MAXIMO_DEZESSEIS_CARACTERES)
			.Custom((password, validator) =>
			{
				if (!MyRegex().IsMatch(password))
					validator.AddFailure(new ValidationFailure(nameof(RequestSignIn.Password),
						MessagesException.SENHA_INVALIDA));
			});

		RuleFor(c => c.Email)
			.NotEmpty()
			.WithMessage(MessagesException.EMAIL_NAO_INFORMADO)
			.EmailAddress()
			.WithMessage(MessagesException.EMAIL_INVALIDO);
	}

	[GeneratedRegex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,16}$")]
	private static partial Regex MyRegex();
}