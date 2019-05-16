﻿/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 15MAY19
 * 
 * Purpose:
 * "Unit test" class - for testing the DnD5e Api access methods. Does interact with the API.
 * ... I guess this class is Integration testing?
 * Not conclusive - really only testing for expected responses, to make sure I don't break anything when updating my code.
 * Uses the "Arrange-Act-Assert" approach & hardcoded results for test purposes.
 */

using NUnit.Framework;
using System;
using NUnit.Framework.SyntaxHelpers;

using DndBuilder.WebApi.Dnd5eApiAccess;
using DndBuilder.WebApi.Models;

namespace DndBuilder.WebApi.Test
{
    [Obsolete("For mono version of NUnit",false)]
    [TestFixture()]
    public class DndApiTests
    {
        [Test()]
        public void GetClassUrlListTest()
        {
            string expectedNames = "BarbarianBardClericDruidFighterMonkPaladinRangerRogueSorcererWarlockWizard";
            string expectedIds = "123456789101112";
            string actualNames = "";
            string actualIds = "";

            foreach (var race in DndApi.GetRaceOrClassesNameIdList(isRaceRequest: false))
            {
                actualNames += race.Key;
                actualIds += race.Value;
            }

            Assert.That(actualNames, Is.EqualTo(expectedNames));
            Assert.That(actualIds, Is.EqualTo(expectedIds));
        }

        [Test()]
        public void GetRaceUrlListTest()
        {
            string expectedNames = "DwarfElfHalflingHumanDragonbornGnomeHalf-ElfHalf-OrcTiefling";
            string expectedIds = "123456789";
            string actualNames = "";
            string actualIds = "";

            foreach (var race in DndApi.GetRaceOrClassesNameIdList(isRaceRequest: true))
            {
                actualNames += race.Key;
                actualIds += race.Value;
            }

            Assert.That(actualNames, Is.EqualTo(expectedNames));
            Assert.That(actualIds, Is.EqualTo(expectedIds));
        }

        [Test()]
        public void GetClassByIdTest()
        {
            DndClass expected = new DndClass() { Name = "Barbarian", HitDie = 12, IsSpellCaster = false };

            var actual = DndApi.GetClassById(1);

            Assert.That(actual.Name, Is.EqualTo(expected.Name));
            Assert.That(actual.HitDie, Is.EqualTo(expected.HitDie));
            Assert.That(actual.IsSpellCaster, Is.EqualTo(expected.IsSpellCaster));
        }

        [Test()]
        public void GetClassByIdTestSpellcaster()
        {
            DndClass expected = new DndClass() { Name = "Wizard", HitDie = 6, IsSpellCaster = true };

            var actual = DndApi.GetClassById(12);

            Assert.That(actual.Name, Is.EqualTo(expected.Name));
            Assert.That(actual.HitDie, Is.EqualTo(expected.HitDie));
            Assert.That(actual.IsSpellCaster, Is.EqualTo(expected.IsSpellCaster));
        }

        [Test()]
        public void GetRaceById()
        {
            DndRace expected = new DndRace() { Name = "Dwarf", RacialBonuses = new int[] {0,0,2,0,0,0} };

            var actual = DndApi.GetRaceById(1);

            Assert.That(actual.Name, Is.EqualTo(expected.Name));
            Assert.That(actual.RacialBonuses, Is.EqualTo(expected.RacialBonuses));
        }
    }
}
