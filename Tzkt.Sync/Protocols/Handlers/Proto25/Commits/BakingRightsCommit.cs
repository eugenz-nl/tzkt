using Tzkt.Data.Models;

namespace Tzkt.Sync.Protocols.Proto25
{
    class BakingRightsCommit(ProtocolHandler protocol) : Proto19.BakingRightsCommit(protocol)
    {
        protected override async Task<IEnumerable<RightsGenerator.BR>> GetCurrentBakingRights(Block block, Cycle cycle)
        {
            var swrr = await SwrrSampler.CreateAsync(Proto, block.Level, block.Cycle);
            if (swrr == null)
                return await base.GetCurrentBakingRights(block, cycle);

            return RightsGenerator.GetBakingRights(swrr, cycle, block.Level, block.BlockRound + 1);
        }

        protected override async Task<(IEnumerable<RightsGenerator.BR>, IEnumerable<RightsGenerator.AR>)> GetFutureRights(Block block, Cycle futureCycle, Dictionary<int, long> selectedStakes)
        {
            var raw = await Proto.Node.GetAsync($"chains/main/blocks/{block.Level}/context/raw/json/cycle/{futureCycle.Index}");
            var swrr = SwrrSampler.Create(Proto, raw);
            if (swrr == null)
                return await base.GetFutureRights(block, futureCycle, selectedStakes);

            Logger.LogDebug("Cycle {cycle} baking rights are drawn from the SWRR selection", futureCycle.Index);

            // attestation rights are not affected by SWRR, so they are still drawn with the alias
            // sampler, but its state is no longer stored in the context, so it can't be validated
            var sampler = GetSampler(
                selectedStakes.Where(x => x.Value > 0).Select(x => (x.Key, x.Value)),
                block.Level == Context.Protocol.FirstCycleLevel);

            return (
                RightsGenerator.GetBakingRights(swrr, Context.Protocol, futureCycle),
                await RightsGenerator.GetAttestationRightsAsync(sampler, Context.Protocol, futureCycle));
        }
    }
}
