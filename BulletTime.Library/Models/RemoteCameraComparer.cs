using System;
using System.Collections.Generic;

namespace BulletTime.Models
{
    public class RemoteCameraComparer : IEqualityComparer<RemoteCameraModel>
    {
        public bool Equals(RemoteCameraModel x, RemoteCameraModel y)
        {
            if (x == null || y == null)
            {
                return x == y;
            }

            return string.Equals(x.IPAddress.ToString(), y.IPAddress.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(RemoteCameraModel obj)
        {
            return obj.IPAddress.ToString().GetHashCode();
        }
    }
}