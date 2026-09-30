# Patched Netezos

`Netezos.3.0.2-pq.1.nupkg` is Netezos v3.0.2 with `netezos-3.0.2-pq.patch` applied. The patch teaches
the Michelson schemas (`address`, `contract`, `key_hash`, `key`) the post-quantum prefixes (tz5/mdpk for
ML-DSA-44, tz6/xmpk for XMSS). Without it, optimizing any Michelson value holding such an address throws
`FormatException: Failed to map String into address`, and the indexer gets stuck on that block.

It is picked up through the `local` source in `nuget.config`. To rebuild:

    git clone https://github.com/baking-bad/netezos && cd netezos
    git checkout v3.0.2 && git apply ../tzkt/nuget/netezos-3.0.2-pq.patch
    dotnet pack Netezos/Netezos.csproj -c Release -p:Version=3.0.2-pq.1 -o ../tzkt/nuget

Remove the package, `nuget.config` and this folder once upstream Netezos supports these prefixes.
