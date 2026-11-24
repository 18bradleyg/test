using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts
{
    /// <summary>
    /// Returned by Shipping.Api when a shipment is created.
    /// </summary>
    public class ShipmentResponse
    {
        /// <summary>Unique tracking number for the new shipment.</summary>
        public string TrackingNumber { get; set; }
    }
}