using Xunit;

namespace Tzkt.Api.Tests.Utils;

public class RegexesTests
{
    [Theory]
    [InlineData("tz1gjaF81ZRRvdzjobyfVNsAeSC6PScjfQwN")] // ed25519
    [InlineData("tz2Rh3NYehvzkESdMoNGY9PYLzhz1kUj5Nr9")] // secp256k1
    [InlineData("tz3WEJYwJ6pPwVbSL8FrSoAXRmFHHZTuEnMA")] // p256
    [InlineData("tz4HVR6aty9KwsQFHh81C1G7gBdhxT8kuytm")] // bls
    [InlineData("tz5bPKDvBoDDurqNMU2DG4yzMu2PuVgzMtAY")] // ml-dsa-44 (post-quantum manager)
    [InlineData("tz6E6K4euitDcvMLakLBiNTsdFcprMvBUaRs")] // xmss (post-quantum consensus)
    [InlineData("KT1XFN25V4dpUcU3Rxntv3dh1Yr9DfM4tGkg")]
    [InlineData("sr1RYurGZtN8KNSpkMcCt9CgWeUaNkzsAfXf")]
    public void Address_AcceptsValidAddresses(string address)
    {
        Assert.Matches(Regexes.Address(), address);
    }

    [Theory]
    [InlineData("tz7bPKDvBoDDurqNMU2DG4yzMu2PuVgzMtAY")] // unknown prefix
    [InlineData("tz5bPKDvBoDDurqNMU2DG4yzMu2PuVgzMtA")]  // too short
    [InlineData("tz5bPKDvBoDDurqNMU2DG4yzMu2PuVgzMtAYZ")] // too long
    [InlineData("")]
    public void Address_RejectsInvalidAddresses(string address)
    {
        Assert.DoesNotMatch(Regexes.Address(), address);
    }
}
