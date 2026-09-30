using Netezos;
using Netezos.Contracts;
using Netezos.Encoding;

namespace Tzkt.Sync.Tests.Helpers
{
    // Netezos is patched (see nuget/netezos-3.0.2-pq.patch) to encode post-quantum addresses and keys
    // in Michelson values. Implicit addresses are 0x00 ‖ tag ‖ hash, key hashes tag ‖ hash and keys
    // tag ‖ raw key, with tags matching octez: 4 = ML-DSA-44 (tz5/mdpk), 5 = XMSS (tz6/xmpk).
    internal class PqMichelsonTests
    {
        const string Tz5 = "tz5WvQ6VBYhhzsPiy5i8h3RgpGwhwwFNUt8y";
        const string Tz6 = "tz6E6K4euitDcvMLakLBiNTsdFcprMvBUaRs";
        const string Xmpk = "xmpkHxeMnW6QPwN4eh3RPHdhd7XwDGFiMMbRqYkmKDp6eDa8UQ7d";

        public static void Run()
        {
            // FA1.2-like origination from block 179829, whose %admin is a tz5 address; used to wedge the indexer
            var schema = Schema.Create((MichelinePrim)Micheline.FromJson("""
                {"prim":"pair","args":[
                    {"prim":"big_map","args":[{"prim":"address"},{"prim":"nat"}]},
                    {"prim":"big_map","args":[{"prim":"pair","args":[{"prim":"address"},{"prim":"address"}]},{"prim":"nat"}]},
                    {"prim":"address","annots":["%admin"]},
                    {"prim":"nat","annots":["%total_supply"]}]}
                """)!);
            var storage = Micheline.FromJson($$"""
                {"prim":"Pair","args":[[],[],{"string":"{{Tz5}}"},{"int":"0"}]}
                """)!;
            // optimized pairs are right combs: Pair [] (Pair [] (Pair admin 0))
            var optimized = (MichelinePrim)schema.Optimize(storage);
            var comb = (MichelinePrim)((MichelinePrim)optimized.Args![1]).Args![1];
            var admin = Hex.Convert(((MichelineBytes)comb.Args![0]).Value);
            if (admin != "0004" + Hex.Convert(Base58.Parse(Tz5, 3)))
                throw new Exception($"Invalid optimized tz5 address: {admin}");
            if (!schema.Humanize(optimized).Contains($"\"admin\":\"{Tz5}\""))
                throw new Exception($"Invalid humanized storage: {schema.Humanize(optimized)}");

            RoundTrip("""{"prim":"address"}""", Tz5, "0004");
            RoundTrip("""{"prim":"address"}""", Tz6, "0005");
            RoundTrip("""{"prim":"address"}""", $"{Tz5}%transfer", "0004");
            RoundTrip("""{"prim":"contract","args":[{"prim":"unit"}]}""", $"{Tz6}%default", "0005");
            RoundTrip("""{"prim":"key_hash"}""", Tz5, "04");
            RoundTrip("""{"prim":"key_hash"}""", Tz6, "05");
            RoundTrip("""{"prim":"key"}""", Xmpk, "05");
            RoundTrip("""{"prim":"key"}""", Base58.Convert(new byte[1312], Prefixes.mdpk), "04");
        }

        static void RoundTrip(string type, string value, string tag)
        {
            var schema = Schema.Create((MichelinePrim)Micheline.FromJson(type)!);
            var bytes = (MichelineBytes)schema.Optimize(new MichelineString(value));
            if (!Hex.Convert(bytes.Value).StartsWith(tag))
                throw new Exception($"Invalid {type} tag for {value}: {Hex.Convert(bytes.Value)}");
            var humanized = schema.Humanize(bytes);
            if (humanized != $"\"{value}\"")
                throw new Exception($"Invalid {type} round-trip: expected {value}, got {humanized}");
        }
    }
}
