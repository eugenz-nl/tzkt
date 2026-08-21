using Tzkt.Data.Models;

namespace Tzkt.Sync.Protocols.Proto25
{
    partial class ProtoActivator(ProtocolHandler proto) : Proto24.ProtoActivator(proto)
    {
        protected override void UpgradeParameters(Protocol protocol, Protocol prev) { }

        protected override Task MigrateContext(AppState state) => Task.CompletedTask;

        protected override Task RevertContext(AppState state) => Task.CompletedTask;

        protected override async Task<(IEnumerable<RightsGenerator.BR>, IEnumerable<RightsGenerator.AR>)> GetRights(Protocol protocol, List<Account> accounts, Cycle cycle)
        {
            var raw = await Proto.Node.GetAsync($"chains/main/blocks/1/context/raw/json/cycle/{cycle.Index}");
            var swrr = SwrrSampler.Create(Proto, raw);
            if (swrr == null)
                return await base.GetRights(protocol, accounts, cycle);

            // attestation rights are not affected by SWRR, so they are still drawn with the alias
            // sampler, but its state is no longer stored in the context, so it can't be validated
            var bakers = accounts
                .Where(x => x is Data.Models.Delegate d && d.BakingPower != 0)
                .Select(x => (x as Data.Models.Delegate)!);

            var sampler = GetSampler(bakers.Select(x => (x.Id, x.BakingPower)));

            return (
                RightsGenerator.GetBakingRights(swrr, protocol, cycle),
                await RightsGenerator.GetAttestationRightsAsync(sampler, protocol, cycle));
        }
    }
}
