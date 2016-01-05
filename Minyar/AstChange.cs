﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Minyar {
    public class AstChange {
        [DataMember(Name = "GithubUrl")] public string GithubUrl;
        [DataMember(Name = "Items")] public HashSet<ChangePair> Items;
        [DataMember(Name = "DiffHunk")] public string DiffHunk;

        public AstChange(string githubUrl, HashSet<ChangePair> changeSet, string diffHunk) {
            GithubUrl = githubUrl;
            Items = changeSet;
            DiffHunk = diffHunk;
        }

        public AstChange() {
            
        }

        public override string ToString() {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter());
            var serializer = JsonSerializer.Create(settings);
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb)) {
                serializer.Serialize(sw, this);
            }
            return sb.ToString();
        }
    }
}
