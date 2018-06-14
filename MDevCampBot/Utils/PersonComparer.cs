using MDevCampBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDevCampBot
{
    public class PersonComparer : IEqualityComparer<Person>
    {
        public bool Equals(Person x, Person y)
        {
            if (x == null || y == null)
                return false;

            return 
                (x.Company == y.Company) 
                && (x.Name == y.Name) 
                && (x.PhotoUrl == y.PhotoUrl);
        }

        public int GetHashCode(Person obj)
        {
            throw new NotImplementedException();
        }
    }
}