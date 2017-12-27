using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheWorld.Models
{
    public class WorldUser : IdentityUser
    {
        public DateTime FirstTrip { get; set; }     // save this property along with the identity user pieces, username, password, email. it will be stored in the database.
    }
}
