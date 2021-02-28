using System;
using System.Collections.Generic;
using UnityEngine;

namespace VmodMonkeMapLoader.Models
{
    public class TeleportData
    {
        public List<GameObject> Triggers { get; set; } = new List<GameObject>();
        public List<TeleportTarget> Targets { get; set; } = new List<TeleportTarget>();
    }
}