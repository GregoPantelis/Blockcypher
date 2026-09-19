using ICMarkets.Blockcypher.Api.Contracts.Responses;
using ICMarkets.Blockcypher.Application.DataObjects.Dtos;
using ICMarkets.Blockcypher.Application.DataObjects.Enums;
using System.Text.Json.Nodes;

namespace ICMarkets.Blockcypher.Api.Mappers
{
    internal static class BlockchainContractsMapper
    {
        internal static BlockchainSnapshotResponse MapToBlockchainSnapshotResponse(this BlockchainData blockchainData)
        {
            if (blockchainData == null) return null;

            return new BlockchainSnapshotResponse
            {
                Chain = blockchainData.Chain,
                Coin = blockchainData.Coin,
                CreateAt = blockchainData.CreatedAt,
                UtcCreatedAt = blockchainData.UtcCreatedAt,
                BlockchainData = blockchainData.BChainData is null
                    ? null
                    : JsonNode.Parse(blockchainData.BChainData.ToString()),
            };
        }

        internal static BlockchainHistoryResponse MapToBlockchainHistoryResponse(this IEnumerable<BlockchainData> blockchainDataList)
        {
            if (blockchainDataList == null || !blockchainDataList.Any()) return null;

            string coin = blockchainDataList.FirstOrDefault()?.Coin ?? Coin.Undefined.ToString();
            string chain = blockchainDataList.FirstOrDefault()?.Chain ?? Chain.Undefined.ToString();

            List<JsonNode> bchainHistoryData = new List<JsonNode>();
            foreach (BlockchainData blockchainData in blockchainDataList)
            {
                if (blockchainData.BChainData == null) continue;

                JsonNode data = JsonNode.Parse(blockchainData.BChainData.ToString());

                data["createdAt"] = blockchainData.CreatedAt;
                data["utcCreatedAt"] = blockchainData.UtcCreatedAt;

                bchainHistoryData.Add(data);
                bchainHistoryData.OrderByDescending(x => x?["createdAt"]);
            }

            return new BlockchainHistoryResponse
            {
                Coin = coin,
                Chain = chain,
                BlockchainHistoryData = bchainHistoryData
            };
        }
    }
}
