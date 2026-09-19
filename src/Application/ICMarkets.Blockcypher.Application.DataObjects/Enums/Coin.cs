using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMarkets.Blockcypher.Application.DataObjects.Enums
{
    public enum Coin
    {
        Undefined = 0,
        /// <summary>
        /// Bitcoin
        /// </summary>
        BTC = 1,
        /// <summary>
        /// Dash
        /// </summary>
        DASH = 2,
        /// <summary>
        /// Dogecoin
        /// </summary>
        DOGE = 3,
        /// <summary>
        /// Litecoin
        /// </summary>
        LTC = 4,
        /// <summary>
        /// BlockCypher
        /// </summary>
        BCY = 5,
        /// <summary>
        /// Ethereum
        /// </summary>
        ETH = 6,
    }
}
