using System.Text.Json;

namespace Tzkt.Sync.Protocols.Proto26
{
    class DalPublishCommitmentCommit(ProtocolHandler protocol) : Proto25.DalPublishCommitmentCommit(protocol)
    {
        // In Alpha (ZODA DAL) the published commitment moved under "publication_commitment"
        // and is now a pair of Merkle roots { row_root, col_root } (each a Fixed.bytes ->
        // 32-byte BLAKE3 digest -> 64-char hex string, no Base58 prefix), instead of the old
        // Base58-encoded KZG commitment at slot_header.commitment. Both roots are durable
        // on-chain (row_root for page proofs, col_root for column-line trap denunciations),
        // so we store the full commitment as row_root ++ col_root (128 hex chars; split at 64).
        protected override string GetCommitment(JsonElement content)
        {
            var commitment = content.Required("slot_header").Required("publication_commitment").Required("commitment");
            return commitment.RequiredString("row_root") + commitment.RequiredString("col_root");
        }
    }
}
