// TEG.Framework.Security.Encryption
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

public class Encryption
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct Operation
	{
		public static readonly int KeySize = 24;

		public static readonly byte[] UnicodeOrderPrefix = new byte[2]
		{
			255,
			254
		};

		public static readonly byte[] UnicodeReversePrefix = new byte[2]
		{
			254,
			255
		};
	}

	internal static readonly string DefaultSecrect = "S3c7rEC6";

	internal static readonly string TEG_SECRET = "TEG China";

	internal static readonly string AjaxEncryptKey = "T1E2G3c4";

	internal static readonly string AjaxEncryptVi = "1q2w30oN";

	public static string Encrypt(string content, string secret)
	{
		if (content == null || secret == null || content.Length == 0 || secret.Length == 0)
		{
			throw new ArgumentNullException("Invalid Argument");
		}
		byte[] key = GetKey(secret);
		byte[] bytes = Encoding.Unicode.GetBytes(content);
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(bytes, 0, bytes.Length);
		byte[] source = Crypt(memoryStream.ToArray(), key);
		return Encoding.ASCII.GetString(Base64Encode(source));
	}

	public static string Decrypt(string content, string secret)
	{
		if (content == null || secret == null || content.Length == 0 || secret.Length == 0)
		{
			throw new ArgumentNullException("Invalid Argument");
		}
		byte[] key = GetKey(secret);
		byte[] source = Base64Decode(Encoding.ASCII.GetBytes(content));
		byte[] array = Decrypt(source, key);
		string text = null;
		byte[] array2 = array;
		byte[] array3 = new byte[Operation.UnicodeReversePrefix.Length];
		Array.Copy(array2, array3, 2);
		if (CompareByteArrays(Operation.UnicodeReversePrefix, array3))
		{
			byte b = 0;
			for (int i = 0; i < array2.Length - 1; i += 2)
			{
				b = array2[i];
				array2[i] = array2[i + 1];
				array2[i + 1] = b;
			}
		}
		return Encoding.Unicode.GetString(array2);
	}

	private static byte[] Crypt(byte[] source, byte[] key)
	{
		if (source.Length == 0 || source == null || key == null || key.Length == 0)
		{
			throw new ArgumentException("Invalid Argument");
		}
		TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
		tripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
		ICryptoTransform cryptoTransform = tripleDESCryptoServiceProvider.CreateEncryptor(key, null);
		return cryptoTransform.TransformFinalBlock(source, 0, source.Length);
	}

	private static byte[] Decrypt(byte[] source, byte[] key)
	{
		if (source.Length == 0 || source == null || key == null || key.Length == 0)
		{
			throw new ArgumentNullException("Invalid Argument");
		}
		TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
		tripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
		ICryptoTransform cryptoTransform = tripleDESCryptoServiceProvider.CreateDecryptor(key, null);
		byte[] outputBuffer = new byte[source.Length + 8];
		int num = cryptoTransform.TransformBlock(source, 0, source.Length, outputBuffer, 0);
		outputBuffer = cryptoTransform.TransformFinalBlock(source, 0, source.Length);
		outputBuffer = cryptoTransform.TransformFinalBlock(source, 0, source.Length);
		num = outputBuffer.Length;
		byte[] array = new byte[num];
		Array.Copy(outputBuffer, array, num);
		return array;
	}

	public static byte[] Base64Encode(byte[] source)
	{
		if (source == null || source.Length == 0)
		{
			throw new ArgumentException("source is not valid");
		}
		ToBase64Transform toBase64Transform = new ToBase64Transform();
		MemoryStream memoryStream = new MemoryStream();
		int i;
		byte[] array;
		for (i = 0; i + 3 < source.Length; i += 3)
		{
			array = toBase64Transform.TransformFinalBlock(source, i, 3);
			memoryStream.Write(array, 0, array.Length);
		}
		array = toBase64Transform.TransformFinalBlock(source, i, source.Length - i);
		memoryStream.Write(array, 0, array.Length);
		return memoryStream.ToArray();
	}

	public static byte[] Base64Decode(byte[] source)
	{
		if (source == null || source.Length == 0)
		{
			throw new ArgumentException("source is not valid");
		}
		FromBase64Transform fromBase64Transform = new FromBase64Transform();
		MemoryStream memoryStream = new MemoryStream();
		int i;
		byte[] array;
		for (i = 0; i + 4 < source.Length; i += 4)
		{
			array = fromBase64Transform.TransformFinalBlock(source, i, 4);
			memoryStream.Write(array, 0, array.Length);
		}
		array = fromBase64Transform.TransformFinalBlock(source, i, source.Length - i);
		memoryStream.Write(array, 0, array.Length);
		return memoryStream.ToArray();
	}

	private static byte[] GetKey(string secret)
	{
		if (secret == null || secret.Length == 0)
		{
			throw new ArgumentException("Secret is not valid");
		}
        ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
        ////byte[] array = Hash(aSCIIEncoding.GetBytes(secret));
        ////byte[] array2 = new byte[Operation.KeySize];
        ////if (array.Length < Operation.KeySize)
        ////{
        ////	Array.Copy(array, 0, array2, 0, array.Length);
        ////	for (int i = array.Length; i < Operation.KeySize; i++)
        ////	{
        ////		array2[i] = 0;
        ////	}
        ////}
        ////else
        ////{
        ////	Array.Copy(array, 0, array2, 0, Operation.KeySize);
        ////}
        ////return array2;
        MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
        byte[] keyhash = hashmd5.ComputeHash(aSCIIEncoding.GetBytes(secret));
        hashmd5 = null;
        return keyhash;
    }

	private static bool CompareByteArrays(byte[] source, byte[] dest)
	{
		if (source == null || dest == null)
		{
			throw new ArgumentException("source or dest is not valid");
		}
		bool result = true;
		if (source.Length != dest.Length)
		{
			return false;
		}
		if (source.Length == 0)
		{
			return true;
		}
		for (int i = 0; i < source.Length; i++)
		{
			if (source[i] != dest[i])
			{
				result = false;
				break;
			}
		}
		return result;
	}

	private static byte[] Hash(byte[] source)
	{
		if (source == null || source.Length == 0)
		{
			throw new ArgumentException("source is not valid");
		}
		MD5 mD = MD5.Create();
		return mD.ComputeHash(source);
	}

	public static string EncryptForAjax(string strToEncrypt)
	{
		return EncryptForAjax(strToEncrypt, AjaxEncryptKey, AjaxEncryptVi);
	}

	internal static string DecryptForAjax(string strToDecrypt)
	{
		return DecryptForAjax(strToDecrypt, AjaxEncryptKey, AjaxEncryptVi);
	}

	private static string EncryptForAjax(string pToEncrypt, string sKey, string vi)
	{
		if (string.IsNullOrWhiteSpace(pToEncrypt) || string.IsNullOrWhiteSpace(sKey) || string.IsNullOrWhiteSpace(vi))
		{
			throw new ArgumentException("Param can't be null");
		}
		if (sKey.Length != 8)
		{
			throw new ArgumentException("Key must be 8");
		}
		using (DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider())
		{
			byte[] bytes = Encoding.UTF8.GetBytes(pToEncrypt);
			dESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(sKey);
			dESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes(vi);
			dESCryptoServiceProvider.Mode = CipherMode.CBC;
			dESCryptoServiceProvider.Padding = PaddingMode.PKCS7;
			MemoryStream memoryStream = new MemoryStream();
			using (CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write))
			{
				cryptoStream.Write(bytes, 0, bytes.Length);
				cryptoStream.FlushFinalBlock();
				cryptoStream.Close();
			}
			string result = Convert.ToBase64String(memoryStream.ToArray());
			memoryStream.Close();
			return result;
		}
	}

	private static string DecryptForAjax(string pToDecrypt, string sKey, string vi)
	{
		if (string.IsNullOrWhiteSpace(pToDecrypt) || string.IsNullOrWhiteSpace(sKey) || string.IsNullOrWhiteSpace(vi))
		{
			throw new ArgumentException("Param can't be null");
		}
		if (sKey.Length != 8)
		{
			throw new ArgumentException("Key must be 8");
		}
		byte[] array = Convert.FromBase64String(pToDecrypt);
		using (DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider())
		{
			dESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(sKey);
			dESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes(vi);
			dESCryptoServiceProvider.Mode = CipherMode.CBC;
			dESCryptoServiceProvider.Padding = PaddingMode.PKCS7;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write))
				{
					cryptoStream.Write(array, 0, array.Length);
					cryptoStream.FlushFinalBlock();
					cryptoStream.Close();
				}
				string @string = Encoding.UTF8.GetString(memoryStream.ToArray());
				memoryStream.Close();
				return @string;
			}
		}
	}
}
