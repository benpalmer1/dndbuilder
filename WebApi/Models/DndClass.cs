/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 18MAY19
 * 
 * Purpose:
 * Model class to hold information about a DnD class from the DnD5e api.
 */

using System;
using Newtonsoft.Json;

namespace DndBuilder.WebApi.Models
{
    [Serializable]
    public class DndClass
    {
        [JsonProperty("Name")]
        private string _Name;

        [JsonProperty("HitDie")]
        private int _HitDie;

        [JsonProperty("IsSpellCaster")]
        private bool _IsSpellCaster;

        public string Name
        {
            get => _Name;
            set
            {
                _Name = HtmlUtil.Encode(HtmlUtil.Decode(value));
            }
        }

        public int HitDie
        {
            get => _HitDie;
            set
            {
                _HitDie = value;
            }
        }

        public bool IsSpellCaster
        {
            get => _IsSpellCaster;
            set
            {
                _IsSpellCaster = value;
            }
        }

        public bool Equals(DndClass b)
        {
            if (this.Name == b.Name &&
                this.HitDie == b.HitDie &&
                this.IsSpellCaster == b.IsSpellCaster)
            {
                return true;
            }

            return false;
        }

        public bool IsValid =>
            !string.IsNullOrEmpty(this.Name) &&
            this.HitDie >= 0;
    }
}