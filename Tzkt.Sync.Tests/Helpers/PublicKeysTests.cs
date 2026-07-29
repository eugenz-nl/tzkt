using Tzkt.Sync.Protocols;

namespace Tzkt.Sync.Tests.Helpers
{
    internal class PublicKeysTests
    {
        public static void Run()
        {
            // ML-DSA-44 (mdpk) public key hashes to tz5. The key below is the one revealed by
            // tz5bPKDvBoDDurqNMU2DG4yzMu2PuVgzMtAY, so its pkh must equal that very address.
            const string mdpk = "mdpk2yQXtPrPqoHMWotvCS2FDEUzr1Tu1Q6E73xugDhyzuug8HEfWgknTtL21jHFW2eXexB1549nBxc1B1vbnH73uqLfTVjjgm7mD44LcEDgqELwFqVMV93i6pKo9ftmMu1gNpSP7QsXj8iBh7ZZbPRSJXWY1oLRvEcPYuanahEN5mc6zzPJri4sYY3nGJ5dcdcscgkYuaw1WStVJgzS4tu6AibsEE4UeoBkYXkBN4zYrcPqmJtsHkccz2wKsAV8m978MP2fvG18tiL9eEgdFPXbs6hqqeNhBqJruvxnocPXwyRKVud1rdT7pFy3cW7nQBr6HpmiybMfxPvo4464zwk7YqyTPXmhamn4V7n3JjeDa9R8ubSjK9B7fbTXDy38YyZhstEJ1MBP9uRDN4NgaQsN7KxPMiThiqxZSmTwJdJmYoFijn2FzqsXmXjgDUfDj488jzftJCXnNHcDAU4hPs3mwm8bSXkj9iL4AwDRXUC2Qw7ueWfcTEzrmcKYghNbfkvSAAQHqmSXpcVuYdUoHZYan5e3a4kMXUaFgLeBRqUrdv4cPMPP5zVaqHMUq7pQK6NksBrF3cSTaLkCbkYV7K2wRhYSYcLwwF4V79GTdhiBTvNPuVWw1BykpdTpifapddvGsWgRzDMc6tmATaDuz5CSomPemgdfycCBv76GHrpBPDmyEUVb7zdg58Y3U1YQWWf2Yx7PSGn7z7SNhYpUmptbc28gd7WBGNySTBw19mP6Lnd7eTjTv1DamzWvSWMwWpUhE1e6o26CB2S5zADoJZGvMU1m3eeiGB9vuFbjuYcL53mmUmFdYnAB9oKqdE1DuKw8hZGKrn4ReeyvExEDAWQZowbqXSJXU5qgRRVHXnnTJqiya9KZCe9GZ14U79kX8UKjzZfgk1JAVGH3d1V3ya9hUdnCcjQLFFDHUSS3oP5hfgbKZJFG6vAj5XCbPUR6TFd2c73MUcUbZvbuTSk8UgpFqURSL9uHUcLdtNNkuK67f6DJMwtuftypLB61XmwdakorrVtr9q9h1Jk79iRwdUPjrHm3whTY5QweCfuMRor7QDW6VUGi5WxLhMpM73KiCoC7F3ZbF7mbHQTeDvK6tVQUwYZ2kLQRZdEhVq26765KDZAQx4522D6vvGjhj3VYYX2cj1udv5b4xSjg7ynbN3ggxUums1RTCkqyoEiwSckXyT94P9Qd6S52iUuxs7XcxTSVao7Rx8wnJdt6KhH8mU3yGbwzELu44ssUv3NHTPoq84cQEmxDz6YEb6hcDtyAhkXv1ppU1LuPbkCKp2VeeLed1qAV95m5Y1fwivwrEpm8P4RKNw7pkKNfZePdBfZEAF4h8C87rMC3SFkZXmpJDLz3UVLng3zZGZjvdgbksYt6msWCnX3U2xcwR8whLCCeYNzcDEnuAuPVNL3icYGHF5upXr14MLSv3pzFVpsbthGQsi1H2LEPL9C5QSFo5CWMdX8okunzqhA8aWSBmyV1woo61XpmuKF1ianVcA4c5wAJkSHdnf141WzDXxSqXwfggyga6pPUcLZFjiVFeNnJcq9QoEXPChibdKWfDpb8i41JA34UKRdXRbFhw7FPCc4WUYoJEBM2CJZqupdkdz9UNaMJPiTcY2k4Ad4H1eMjtkbsLhbMh9RVoCbYwaPcobAVETSq7zcTyQZJgF9oG5ccfB8fR2xcN2893jSRKUboma8V8epNssx9YaYwtYTD8G9anMzaTCKBGdSob1faFUyx28M4vuMsPfoeVgGL92iT4maCFYhrN6fGqWPHGs4DcWAym2vmoMZaXs";
            const string mdpkPkh = "tz5bPKDvBoDDurqNMU2DG4yzMu2PuVgzMtAY";

            // XMSS (xmpk) public key hashes to tz6.
            const string xmpk = "xmpkHxeMnW6QPwN4eh3RPHdhd7XwDGFiMMbRqYkmKDp6eDa8UQ7d";
            const string xmpkPkh = "tz6E6K4euitDcvMLakLBiNTsdFcprMvBUaRs";

            // A regular ed25519 key must still resolve via Netezos (the fallback path).
            const string edpk = "edpktzNbDAUjUk697W7gYg2CRuBQjyPxbEg8dLccYYwKSKvkPvjtV9";
            const string edpkPkh = "tz1gjaF81ZRRvdzjobyfVNsAeSC6PScjfQwN";

            Assert(mdpk, mdpkPkh);
            Assert(xmpk, xmpkPkh);
            Assert(edpk, edpkPkh);
        }

        static void Assert(string publicKey, string expectedPkh)
        {
            var pkh = PublicKeys.GetPublicKeyHash(publicKey);
            if (pkh != expectedPkh)
                throw new Exception($"Invalid public key hash: expected {expectedPkh}, got {pkh}");
        }
    }
}
