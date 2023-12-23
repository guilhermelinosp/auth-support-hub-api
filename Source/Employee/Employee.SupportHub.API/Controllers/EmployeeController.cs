using System.Net;
using Employee.SupportHub.API.Controllers.Abstract;
using Employee.SupportHub.Application.UseCases.ForgotPassword;
using Employee.SupportHub.Application.UseCases.ForgotPassword.Confirmation;
using Employee.SupportHub.Application.UseCases.SignIn;
using Employee.SupportHub.Application.UseCases.SignOut;
using Employee.SupportHub.Domain.DTOs.Requests;
using Employee.SupportHub.Domain.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Employee.SupportHub.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[ProducesResponseType<BaseActionResult<ResponseException>>(StatusCodes.Status400BadRequest)]
[ProducesResponseType<BaseActionResult<ResponseDefault>>(StatusCodes.Status200OK)]
public class EmployeeController(
	ISignInUseCase signIn,
	ISignOutUseCase signOut,
	IResetPasswordUseCase resetPassword,
	IForgotPasswordUseCase forgotPassword) : Controller
{
	[HttpPost("signin")]
	public async Task<BaseActionResult<ResponseToken>> SignInRequest([FromBody] RequestSignIn request)
	{
		var response = await signIn.ExecuteAsync(request);
		return new BaseActionResult<ResponseToken>(HttpStatusCode.OK, response);
	}

	[HttpGet("signout")]
	public async Task<BaseActionResult<ResponseDefault>> SignOutRequest()
	{
		var token = Request.Headers.Authorization.ToString().Split(" ")[1];
		var response = await signOut.ExecuteAsync(token);
		return new BaseActionResult<ResponseDefault>(HttpStatusCode.OK, response);
	}

	[HttpPost("forgot-password")]
	public async Task<BaseActionResult<ResponseDefault>> ForgotPasswordRequest([FromBody] RequestForgotPassword request)
	{
		var response = await forgotPassword.ExecuteAsync(request);
		return new BaseActionResult<ResponseDefault>(HttpStatusCode.OK, response);
	}

	[HttpPost("forgot-password/{accountId}/{otp}")]
	public async Task<BaseActionResult<ResponseDefault>> ResetPasswordRequest([FromBody] RequestResetPassword request,
		[FromRoute] string accountId, [FromRoute] string otp)
	{
		var responde = await resetPassword.ExecuteAsync(request, accountId, otp);
		return new BaseActionResult<ResponseDefault>(HttpStatusCode.OK, responde);
	}
};