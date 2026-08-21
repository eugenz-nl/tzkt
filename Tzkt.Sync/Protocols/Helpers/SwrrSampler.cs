using System.Text.Json;

namespace Tzkt.Sync.Protocols
{
    /// <summary>
    /// Stake-weighted round-robin (SWRR) baker selection.
    /// When SWRR is enabled (`swrr_new_baker_lottery_enable`), the protocol doesn't draw block
    /// proposers with the alias sampler anymore. Instead, at the end of each cycle it precomputes
    /// the round 0 baker of every level of the target cycle and stores that array in the context
    /// (`/cycle/{cycle}/selected_bakers`), the baker of a given (level, round) being
    /// `selected_bakers[(cycle_position + 3 * round) mod length]`.
    /// </summary>
    class SwrrSampler
    {
        public int Length => Bakers.Length;

        readonly int[] Bakers;

        internal SwrrSampler(int[] bakers) => Bakers = bakers;

        public int GetBaker(int cyclePosition, int round)
            => Bakers[(cyclePosition + 3 * round) % Bakers.Length];

        /// <summary>
        /// Returns the SWRR selection of the given cycle, or `null` if the cycle's proposers
        /// were not drawn by SWRR (feature disabled, or cycle precomputed before its activation),
        /// in which case the alias sampler must be used instead.
        /// </summary>
        public static async Task<SwrrSampler?> CreateAsync(ProtocolHandler proto, int level, int cycle)
            => Create(proto, await proto.Node.GetAsync($"chains/main/blocks/{level}/context/raw/json/cycle/{cycle}"));

        /// <summary>
        /// Same as `CreateAsync`, but for an already fetched `/cycle/{cycle}` raw context.
        /// </summary>
        public static SwrrSampler? Create(ProtocolHandler proto, JsonElement rawCycle)
        {
            if (rawCycle.OptionalArray("selected_bakers") is not JsonElement selected)
                return null;

            var bakers = new int[selected.GetArrayLength()];
            var index = 0;

            foreach (var consensusPk in selected.EnumerateArray())
            {
                // `delegate` is omitted when the baker doesn't use a separate consensus key
                var address = consensusPk.OptionalString("delegate")
                    ?? PublicKeys.GetPublicKeyHash(consensusPk.RequiredString("consensus_pk"));

                bakers[index++] = proto.Cache.Accounts.GetExistingDelegate(address).Id;
            }

            return bakers.Length > 0 ? new SwrrSampler(bakers) : null;
        }
    }
}
