/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 15MAY19
 * 
 * Purpose:
 * Unit test class - for testing the DnD5e Api access methods.
 */

using NUnit.Framework;
using System;

using DndBuilder.WebApi.Dnd5eApiAccess;
using NUnit.Framework.SyntaxHelpers;

namespace DndBuilder.WebApi.Test
{
    [Obsolete("For mono version of NUnit",false)]
    [TestFixture()]
    public class DndApiTests
    {
        [Test()]
        public void GetClassUrlListTest()
        {
            // Hardcoded result for test purposes
            string expected = "BarbarianBardClericDruidFighterMonkPaladinRangerRogueSorcererWarlockWizard";
            string actual = "";
            foreach (var race in DndApi.GetClassUrlList())
            {
                actual += race.Key;
            }

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
