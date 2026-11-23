using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WizardLibrary
{
    [Serializable]
    public class SpellList
    {
        [JsonIgnore]
        public string filePath;
        public string mod;
        public List<NPC>? characters;

        public override string ToString()
        {
            var result = $"mod: {mod} \n";
            if (characters.IsNull())
            {
                result += "character list was null";
            }
            else
            {
                foreach (var item in characters)
                {
                    if (item.IsNull())
                    {
                        result += "character was null";

                    }
                    else if (item.id.IsNull() || item.name.IsNull())
                    {
                        result += "character name/is was null";
                    }
                    
                    else if (item.expressions.IsNull() || item.expressions.Count == 0)
                    {
                        result += "character expressions was null";
                    }
                    else
                    {
                        result += $"character: {item.id} {item.name} expressions: {item.expressions.Count}\n";
                    }
                }
            }
            

            return result;
        }

        public string GetNpcExpression(string npcId, string expression)
        {
            var npc = characters.FirstOrDefault(x => x.id == npcId);
            return npc.expressions.FirstOrDefault(x => x.Key == expression).Value;
        }

        public string GetNpcName(string npcId)
        {
            var npc = characters.FirstOrDefault(x => x.id == npcId);
            return npc.name;
        }
    }

    [Serializable]
    public struct NPC
    {
        public string id;
        public string name;
        public Dictionary<string, string> expressions;

    }
}
