using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math.EC;
using System.Text;

namespace API.Security;

/// <summary>
/// Custom SecurityKey for EdDSA (Ed25519) validation using BouncyCastle
/// </summary>
public class EdDsaSecurityKey : AsymmetricSecurityKey
{
    private readonly Ed25519PublicKeyParameters _publicKey;
    private readonly string _kid;

    public EdDsaSecurityKey(byte[] publicKeyBytes, string kid = "")
    {
        _publicKey = new Ed25519PublicKeyParameters(publicKeyBytes, 0);
        _kid = kid;
        KeyId = kid;
    }

    public Ed25519PublicKeyParameters PublicKey => _publicKey;

    public override int KeySize => 256; // Ed25519 uses 256-bit keys

    // Required overrides for AsymmetricSecurityKey
    [Obsolete("HasPrivateKey is deprecated, use PrivateKeyStatus instead.")]
    public override bool HasPrivateKey => false;

    public override PrivateKeyStatus PrivateKeyStatus => PrivateKeyStatus.DoesNotExist;

    /// <summary>
    /// Create EdDsaSecurityKey from JWK (JSON Web Key)
    /// </summary>
    public static EdDsaSecurityKey FromJwk(string x, string kid)
    {
        // Base64Url decode the x parameter (public key)
        var publicKeyBytes = Base64UrlEncoder.DecodeBytes(x);
        return new EdDsaSecurityKey(publicKeyBytes, kid);
    }
}
