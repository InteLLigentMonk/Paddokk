using Microsoft.IdentityModel.Tokens;

namespace API.Security;

/// <summary>
/// Custom CryptoProviderFactory for EdDSA algorithm support
/// </summary>
public class EdDsaCryptoProviderFactory : CryptoProviderFactory
{
    public EdDsaCryptoProviderFactory() : base()
    {
    }

    public override SignatureProvider CreateForVerifying(SecurityKey key, string algorithm)
    {
        if (key is EdDsaSecurityKey edDsaKey && algorithm == "EdDSA")
        {
            return new EdDsaSignatureProvider(edDsaKey, algorithm);
        }

        return base.CreateForVerifying(key, algorithm);
    }

    public override SignatureProvider CreateForSigning(SecurityKey key, string algorithm)
    {
        throw new NotSupportedException("Signing is not supported in this implementation.");
    }

    public override bool IsSupportedAlgorithm(string algorithm, SecurityKey key)
    {
        if (algorithm == "EdDSA" && key is EdDsaSecurityKey)
        {
            return true;
        }

        return base.IsSupportedAlgorithm(algorithm, key);
    }
}
