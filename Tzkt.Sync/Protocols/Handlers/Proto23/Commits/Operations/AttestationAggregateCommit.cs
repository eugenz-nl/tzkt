using System.Text.Json;

namespace Tzkt.Sync.Protocols.Proto23
{
    class AttestationAggregateCommit(ProtocolHandler protocol) : ProtocolCommit(protocol)
    {
        public IEnumerable<(string, string, long, string?)> ExtractAttestations(JsonElement op, JsonElement content)
        {
            var res = new List<(string, string, long, string?)>();

            var opHash = op.RequiredString("hash");

            // The per-attester DAL bitset lives in the *content* committee ([{ slot, dal_attestation? }, ...]),
            // positionally aligned with the metadata committee (apply.ml builds the result committee by
            // folding over the op committee, preserving order). Plain members have no dal_attestation.
            var dalBits = content.RequiredArray("committee").EnumerateArray()
                .Select(x => x.OptionalString("dal_attestation"))
                .ToList();

            var i = 0;
            foreach (var c in content.Required("metadata").RequiredArray("committee").EnumerateArray())
            {
                var baker = Cache.Accounts.GetExistingDelegate(c.RequiredString("delegate"));
                var power = GetPower(c);
                res.Add((opHash, baker.Address, power, i < dalBits.Count ? dalBits[i] : null));
                i++;
            }

            return res;
        }

        protected virtual long GetPower(JsonElement c)
        {
            return c.RequiredInt64("consensus_power");
        }
    }
}
