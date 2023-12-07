﻿using System.Net;
using SupportHub.Auth.Application.Services.Cryptography;
using SupportHub.Auth.Domain.Apis;
using SupportHub.Auth.Domain.Dtos.Requests.Companies;
using SupportHub.Auth.Domain.Dtos.Responses.Apis.Brasil;
using SupportHub.Auth.Domain.Entities;
using SupportHub.Auth.Domain.Exceptions;
using SupportHub.Auth.Domain.Repositories;
using SupportHub.Auth.Domain.ServicesExternal;
using SupportHub.Auth.Domain.Shared.Returns;

namespace SupportHub.Auth.Application.UseCases.Companies.SignUp;

public class SignUpUseCase(
    ICompanyRepository repository,
    IEncryptService encrypt,
    ISendGrid sendGrid,
    IBrasilApi brasilApi)
    : ISignUpUseCase
{
    public async Task ExecuteAsync(RequestSignUp request)
    {
        var validatorRequest = await new SignUpValidator().ValidateAsync(request);
        if (!validatorRequest.IsValid)
        {
            throw new ValidatorException(validatorRequest.Errors.Select(er => er.ErrorMessage).ToList());
        }

        var validateEmail = await repository.FindCompanyByEmailAsync(request.Email);
        if (validateEmail is not null)
            throw new CompanyException(new List<string> { MessagesException.EMAIL_JA_REGISTRADO });

        var validateCnpj = await repository.FindCompanyByCnpjAsync(request.Cnpj);
        if (validateCnpj is not null)
            throw new CompanyException(new List<string> { MessagesException.CNPJ_JA_REGISTRADO });

        BasicReturn<ResponseCnpj> returnCnpj = await brasilApi.ConsultaCnpj(request.Cnpj);
        if (returnCnpj.IsFailure)
        {
            throw new ValidatorException(new List<string> { returnCnpj.Error.Message });
        }

        if (request.Password != request.PasswordConfirmation)
            throw new CompanyException(new List<string> { MessagesException.SENHA_NAO_CONFERE });

        var code = encrypt.GenerateCode().ToUpper();

        var company = new Company
        {
            Cnpj = request.Cnpj,
            Email = request.Email,
            Password = encrypt.EncryptPassword(request.Password),
            Code = code,
        };

        await repository.CreateCompanyAsync(company);

        await sendGrid.SendSignUpAsync(request.Email, code);
    }
}