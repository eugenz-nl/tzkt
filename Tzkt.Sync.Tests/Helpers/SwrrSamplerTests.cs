using Tzkt.Sync.Protocols;

namespace Tzkt.Sync.Tests.Helpers
{
    internal class SwrrSamplerTests
    {
        public static void Run()
        {
            // mirrors `Swrr_sampler.get_baker`: idx = (cycle_position + 3 * round) mod length
            var bakers = new[] { 10, 11, 12, 13, 14 };
            var sampler = new SwrrSampler(bakers);

            Assert(sampler, 0, 0, 10);  // (0 + 0) % 5 = 0
            Assert(sampler, 3, 0, 13);  // (3 + 0) % 5 = 3
            Assert(sampler, 0, 1, 13);  // (0 + 3) % 5 = 3
            Assert(sampler, 4, 1, 12);  // (4 + 3) % 5 = 2
            Assert(sampler, 2, 7, 13);  // (2 + 21) % 5 = 3
        }

        static void Assert(SwrrSampler sampler, int cyclePosition, int round, int expectedBaker)
        {
            var baker = sampler.GetBaker(cyclePosition, round);
            if (baker != expectedBaker)
                throw new Exception($"SwrrSampler.GetBaker({cyclePosition}, {round}) returned {baker} instead of {expectedBaker}");
        }
    }
}
