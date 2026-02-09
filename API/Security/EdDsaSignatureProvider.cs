using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Linq;

namespace API.Security;

/// <summary>
/// Custom SignatureProvider for EdDSA (Ed25519) using BouncyCastle
/// </summary>
public class EdDsaSignatureProvider : SignatureProvider
{
    private readonly EdDsaSecurityKey _key;

    public EdDsaSignatureProvider(EdDsaSecurityKey key, string algorithm) 
        : base(key, algorithm)
    {
        _key = key;
    }

    public override byte[] Sign(byte[] input)
    {
        throw new NotSupportedException("Signing is not supported for validation-only scenarios.");
    }

    // IdentityModel may call this overload (no offsets) — keep diagnostic logs and verify
    public override bool Verify(byte[] input, byte[] signature)
    {
        try
        {
            var signer = new Ed25519Signer();
            signer.Init(false, _key.PublicKey);
            signer.BlockUpdate(input, 0, input.Length);
            var result = signer.VerifySignature(signature);

            return result;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    // IdentityModel calls this overload with offsets — implement it and delegate to the above
    public override bool Verify(byte[] input, int inputOffset, int inputLength, byte[] signature, int signatureOffset, int signatureLength)
    {
        try
        {
            if (input == null || signature == null)
                return false;

            // If the offsets/lengths already point to the whole arrays, avoid copying
            byte[] inputSegment = (inputOffset == 0 && inputLength == input.Length) 
                ? input 
                : input.Skip(inputOffset).Take(inputLength).ToArray();

            byte[] sigSegment = (signatureOffset == 0 && signatureLength == signature.Length) 
                ? signature 
                : signature.Skip(signatureOffset).Take(signatureLength).ToArray();

            return Verify(inputSegment, sigSegment);
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    protected override void Dispose(bool disposing)
    {
        // Nothing to dispose
    }
}
