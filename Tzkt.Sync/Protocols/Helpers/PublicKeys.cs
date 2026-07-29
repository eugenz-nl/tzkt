using Blake2Fast;
using Netezos.Encoding;
using Netezos.Keys;

namespace Tzkt.Sync.Protocols
{
    static class PublicKeys
    {
        // Post-quantum keys that Netezos doesn't know yet: ML-DSA-44 (mdpk) public keys
        // hash to tz5, XMSS (xmpk) public keys hash to tz6. The pkh is Blake2b-160 of the
        // raw key bytes (same scheme octez uses), mirroring the pattern in Blind.cs.
        static readonly byte[] tz5 = [6, 161, 169];
        static readonly byte[] tz6 = [6, 161, 171];

        public static string GetPublicKeyHash(string publicKey)
        {
            if (publicKey.StartsWith("mdpk"))
                return Hash(publicKey, tz5);
            if (publicKey.StartsWith("xmpk"))
                return Hash(publicKey, tz6);
            return PubKey.FromBase58(publicKey).Address;
        }

        static string Hash(string publicKey, byte[] prefix)
        {
            // Base58.Parse returns [prefix][payload] with the checksum stripped; mdpk and xmpk
            // both have a 4-byte prefix, so the raw key bytes start at offset 4.
            var raw = Base58.Parse(publicKey)[4..];
            return Base58.Convert(Blake2b.ComputeHash(20, raw), prefix);
        }
    }
}
