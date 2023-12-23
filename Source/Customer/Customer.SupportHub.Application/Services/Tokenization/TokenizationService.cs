﻿using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Customer.SupportHub.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Customer.SupportHub.Application.Services.Tokenization;

public class TokenizationService(IConfiguration configuration) : ITokenizationService
{
	public Guid ValidateToken(string token)
	{
		try
		{
			new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Jwt_Secret"]!)),
				ValidateIssuer = false,
				ValidateAudience = false,
				ValidateLifetime = true
			}, out var validatedToken);

			var jwtToken = (JwtSecurityToken)validatedToken;

			if (jwtToken.ValidTo < DateTime.UtcNow)
				throw new SecurityTokenExpiredException(MessageException.TOKEN_EXPIRADO);

			return new Guid(jwtToken.Claims.First(x => x.Type == "id").Value);
		}
		catch (SecurityTokenExpiredException)
		{
			throw new ExceptionDefault(new List<string> { MessageException.TOKEN_EXPIRADO });
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException(ex.Message);
		}
	}

	public string GenerateToken(string id)
	{
		try
		{
			var tokenHandler = new JwtSecurityTokenHandler();

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim("id", id)
				}),
				Expires = DateTime.UtcNow.Add(TimeSpan.Parse(configuration["Jwt_Expiry"]!, CultureInfo.CurrentCulture)),
				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Jwt_Secret"]!)),
					SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);

			return tokenHandler.WriteToken(token);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException(ex.Message);
		}
	}

	public string GenerateRefreshToken()
	{
		var salt = new byte[32];
		using var random = RandomNumberGenerator.Create();
		random.GetBytes(salt);
		return Convert.ToBase64String(salt);
	}

	public static void DeleteRefreshToken(string refreshToken)
	{
		var salt = new byte[32];
		using var random = RandomNumberGenerator.Create();
		random.GetBytes(salt);
		Convert.ToBase64String(salt);
	}

	public static void UpdateRefreshToken(string refreshToken)
	{
		var salt = new byte[32];
		using var random = RandomNumberGenerator.Create();
		random.GetBytes(salt);
		Convert.ToBase64String(salt);
	}
}