using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace MDevCampBot.Models
{
    [Serializable]
    public class PeopleCollection : List<Person>
    {
        public List<Person> FindByName(string name)
        {
            // birch => Joe Birch
            // božena rezab => Bozena Rezab
            // james => James Montemagno / James Thomas
            // jmaes => -
            return this.Where(p => Utils.NormalizeText(p.Name).Contains(Utils.NormalizeText(name))).ToList();
        }

        public Person GetRandomPerson()
        {
            Random rand = new Random();
            return this.ElementAt(rand.Next(0, this.Count));
        }

        public List<Person> GetRandomPeople(Person include, int count = 3, IEnumerable<Person> guessedPoeple = null)
        {
            var res = new Person[count];
            Random rand = new Random();
            var shuffle = rand.Next(0, count); // na kterou pozici přijde správná odpověď
            res[shuffle] = include;
            for (int i = 0; i < count; i++)
            {
                if (i == shuffle)
                    continue;

                Person p;
                do
                {
                    p = this.GetRandomPerson();
                } while (res.Contains(p, new PersonComparer()) || p.Name == include.Name);

                if (guessedPoeple == null || !guessedPoeple.Contains(p))
                    res[i] = p;
            }

            return res.ToList();
        }

        public static PeopleCollection FromCsv(string csvContent)
        {
            var res = new PeopleCollection();

            var lines = csvContent.Split(Environment.NewLine.ToCharArray());
            if (lines.Length == 0)
            {
                throw new ArgumentException("People list empty or invalid.");
            }

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                var person = Person.FromCsv(line);
                res.Add(person);
            }

            return res;
        }
    }

    [Serializable]
    public class Person
    {
        public string Name { get; set; }
        public string Company { get; set; }
        public string PhotoUrl { get; set; }

        public static Person FromCsv(string csvContent)
        {
            var expanded = csvContent.Split(',', ';');
            if (expanded.Length == 3)
            {
                var res = new Person()
                {
                    Name = expanded[0],
                    Company = expanded[1],
                    PhotoUrl = expanded[2]
                };

                return res;
            }
            else
            {
                throw new ArgumentException("CSV content should contain 3 columns.");
            }
        }
    }
}