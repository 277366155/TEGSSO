// TEG.Framework.Security.SSO.SSOHelper
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
//using TEG.Framework.Security;

public class SSOHelper
{
	private static IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile("TEG.Framework.Security.Config.json", optional: true).Build();

	public static bool IsTokenValid(string tokenID, out List<string> loginInfoList)
	{
		loginInfoList = null;
		if (tokenID == null || tokenID.Length < 1)
		{
			return false;
		}
		string text = configuration["TEGSecurity:Secrect"];
		if (string.IsNullOrWhiteSpace(text))
		{
			text = Encryption.DefaultSecrect;
		}
		try
		{
			string text2 = Encryption.Decrypt(tokenID, text);
			if (text2 != null && text2.Length > 0)
			{
				loginInfoList = text2.Split(',').ToList();
				return true;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message + ex.StackTrace);
		}
		return false;
	}

	public static string GenerateToken(string userID, string loginName, string userName, string realClientIp = null)
	{
		if (userID == null || userID.Length < 1)
		{
			return string.Empty;
		}
		string text = configuration["TEGSecurity:Secrect"];
		if (string.IsNullOrWhiteSpace(text))
		{
			text = Encryption.DefaultSecrect;
		}
		object[] obj = new object[6]
		{
			userID,
			loginName,
			userName,
			string.IsNullOrWhiteSpace(realClientIp) ? "" : realClientIp,
			DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
			null
		};
		Guid guid = default(Guid);
		obj[5] = guid.ToString().Substring(0, 8);
		string content = string.Format("{0},{1},{2},{3},{4},{5}", obj);
		return Encryption.Encrypt(content, text);
	}

	public static string EncryptPasswordForAjax(string passwordEncByAjax, string loginname)
	{
		string content = Encryption.DecryptForAjax(passwordEncByAjax);
		return Encryption.Encrypt(content, Encryption.TEG_SECRET + loginname);
	}

	public static string EncryptPassword(string passwordToEncrypt, string loginname)
	{
		return Encryption.Encrypt(passwordToEncrypt, Encryption.TEG_SECRET + loginname);
	}

	public static string DecryptPassword(string encryptedPassword, string loginname)
	{
		return Encryption.Decrypt(encryptedPassword, Encryption.TEG_SECRET + loginname);
	}

	public string GetDecryptionValidKeysOfMachinekey(out string validationKey)
	{
		validationKey = CreateMachineKey(20);
		return CreateMachineKey(24);
	}

	protected string CreateMachineKey(int len)
	{
		byte[] array = new byte[len];
		new RNGCryptoServiceProvider().GetBytes(array);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append($"{array[i]:X2}");
		}
		return stringBuilder.ToString();
	}
}
