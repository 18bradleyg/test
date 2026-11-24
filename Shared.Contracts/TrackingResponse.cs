using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace Shared.Contracts
{
    /// <summary>
    /// Returned by Shipping.Api when a package is tracked.
    /// </summary>
    public class TrackingResponse
    {
        /// <summary>The tracking number that was looked up.</summary>
        public string TrackingNumber { get; set; }

        /// <summary>"Processing", "In Transit" or "Delivered".</summary>
        public string Status { get; set; }

        /// <summary>Date the package is expected to arrive.</summary>
        public DateTime EstimatedDelivery { get; set; }

        /// <summary>Date the package was delivered. Null until it arrives.</summary>
        public DateTime? ActualDelivery { get; set; }
    }
}
