using System;
using System.Collections.Generic;
using BulletTime.Models;

namespace BulletTime.Controllers
{
    public class ResolutionComparer : IEqualityComparer<RemoteResolutionModel>
    {
        public bool Equals(RemoteResolutionModel x, RemoteResolutionModel y)
        {
            if (x == null || y == null)
            {
                return x == y;
            }

            return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(RemoteResolutionModel obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}