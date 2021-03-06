﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Accord;
using FP.DAL.DAO;
using Minyar.Extensions;
using Paraiba.Collections.Generic;
using Paraiba.IO;
using Paraiba.Linq;

namespace Minyar.MachineLearning {
    public class DataProcessor {
        public class DataShortageException : Exception {
            
        }

        public class MlItem {
            public HashSet<string> Items { get; set; }
            public string Tokens { get; set; }
            public string PullUrl { get; set; }
            public int Addition { get; set; }
            public int Deletion { get; set; }
            public bool OrgIsInner { get; set; }
            public bool CmpIsInner { get; set; }

            public MlItem(HashSet<string> items, string tokens, string pullUrl, int add, int del, bool orgIsInner, bool cmpIsInner) {
                Items = items;
                Tokens = tokens;
                PullUrl = pullUrl;
                Addition = add;
                Deletion = del;
                OrgIsInner = orgIsInner;
                CmpIsInner = cmpIsInner;
            }
        }

        private string negativeFilePath;
        private string positiveFilePath;
        private List<AstChange> negativeItems;
        private List<AstChange> positiveItems;

        public List<MlItem> NegativeItems {
            get {
                return new List<MlItem>(
                    negativeItems.Select(
                        i => new MlItem(
                            new HashSet<string>(i.Items.Select(j => j.Symbol)),
                            string.Join(" ", i.Items.Where(j => j.NodeType == "identifier").Select(j => j.ChangedToken)),
                            i.GithubUrl,
                            i.Addition,
                            i.Deletion,
                            i.OrgIsInnerOfMethod,
                            i.CmpIsInnerOfMethod
                            )
                        )
                    ).ToList();
            }
        } 

        public List<MlItem> PositiveItems {
            get {
                return new List<MlItem>(
                    positiveItems.Select(
                        i => new MlItem(
                            new HashSet<string>(i.Items.Select(j => j.Symbol)),
                            string.Join(" ", i.Items.Where(j => j.NodeType == "identifier").Select(j => j.ChangedToken)),
                            i.GithubUrl,
                            i.Addition,
                            i.Deletion,
                            i.OrgIsInnerOfMethod,
                            i.CmpIsInnerOfMethod
                            )
                        )
                    ).ToList();
            }
        } 

        public DataProcessor(string negativePath, string positivePath) {
            negativeFilePath = negativePath;
            positiveFilePath = positivePath;
            negativeItems = new List<AstChange>();
            positiveItems = new List<AstChange>();
        }

        public void Sample(int count, int minItemCount) {
            SampleRandomly(negativeFilePath, ref negativeItems, count, minItemCount);
            SampleRandomly(positiveFilePath, ref positiveItems, count, minItemCount);
        }

        private void SampleRandomly(string path, ref List<AstChange> itemList, int count, int minItemCount) {
            using (var reader = new StreamReader(path)) {
                var items = new List<AstChange>();
                foreach (var line in reader.ReadLines()) { 
                    //var itemWrapper = AstChange.Deserialize(line.Trim());
                    var astChange = JsonConverter.Deserialize<AstChange>(line.Trim());
                    if (astChange.Items/*.Select(i => i.Symbol).ToHashSet()*/.Count < minItemCount) continue;
                    items.Add(astChange);
                }
                if (items.Count < count) throw new DataShortageException();
                itemList.AddRange(items.Shuffle().Take(count));
            }
        }
    }
}
