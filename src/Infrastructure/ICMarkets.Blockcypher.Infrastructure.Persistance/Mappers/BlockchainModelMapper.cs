using ICMarkets.Blockcypher.Application.DataObjects.Dtos;
using ICMarkets.Blockcypher.Domain.Entities;
using ICMarkets.Blockcypher.Infrastructure.Persistance.Models;
using Newtonsoft.Json;

namespace ICMarkets.Blockcypher.Infrastructure.Persistance.Mappers
{
    internal static class BlockchainModelMapper
    {
        internal static BlockchainModel MapToBlockchainModel(this BlockchainEntity blockchainEntity)
        {
            if (blockchainEntity == null) return null;

            string blockChainData = blockchainEntity.ToString();

            return new BlockchainModel
            {
                Coin = blockchainEntity.Coin,
                Chain = blockchainEntity.Chain,
                BlockchainData = blockChainData,
                CreatedAt = blockchainEntity.CreatedAt == default(DateTime) ? DateTime.Now : blockchainEntity.CreatedAt,
                UpdatedAt = DateTime.UtcNow,  
                UtcCreatedAt = blockchainEntity.UtcCreatedAt == default(DateTime) ? DateTime.UtcNow : blockchainEntity.UtcCreatedAt
            };
        }

        internal static BlockchainModel MapToBlockchainModel(this BlockchainData blockchaindata)
        {
            if (blockchaindata == null) return null;

            return new BlockchainModel
            {
                Coin = blockchaindata.Coin,
                Chain = blockchaindata.Chain,
                BlockchainData = blockchaindata.BChainData.ToString(),
                CreatedAt = blockchaindata.CreatedAt == default(DateTime) ? DateTime.Now : blockchaindata.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                UtcCreatedAt = blockchaindata.UtcCreatedAt == default(DateTime) ? DateTime.UtcNow : blockchaindata.UtcCreatedAt
            };

        }

        internal static BlockchainData MapToBlockchainData(this BlockchainModel blockchainModel)
        {
            if (blockchainModel == null) return null;

            return new BlockchainData
            {
                Coin = blockchainModel.Coin,
                Chain = blockchainModel.Chain,
                BChainData = JsonConvert.DeserializeObject(blockchainModel.BlockchainData) as Newtonsoft.Json.Linq.JObject,
                CreatedAt = blockchainModel.CreatedAt,
                UtcCreatedAt = blockchainModel.UtcCreatedAt
            };

        }

        internal static BlockchainEntity MapToBlockchainEntity(this BlockchainModel blockchainModel)
        {
            if (blockchainModel == null) return null;

            BlockchainEntity entity = JsonConvert.DeserializeObject<BlockchainEntity>(blockchainModel.BlockchainData);

            if (entity == null)
            {
                throw new InvalidOperationException("Failed to deserialize BlockchainData to BlockchainEntity.");
            }

            entity.Chain = blockchainModel.Chain;
            entity.Coin = blockchainModel.Coin;
            entity.CreatedAt = blockchainModel.CreatedAt;
            entity.UtcCreatedAt = blockchainModel.UtcCreatedAt;

            return entity;
        }
    }
}
