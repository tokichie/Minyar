using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Livet.Commands;
using Minyar.Database;
using Minyar.Extensions;
using Newtonsoft.Json;

namespace MinyarClient.ViewModel {
    public class ReviewViewModel : Livet.ViewModel, IDisposable {
        private MinyarModel model;
        private List<review_comments> comments;
        private HashSet<review_comments> sampleComments;
        private string comment;
        private Uri uri;
        private int sampleCount;
        private List<int> cursors; 
        private int cursor;
        private int totalComments;

        private static readonly string path = Path.Combine("..", "..", "..", "data", "Negative.json");

        public ReviewViewModel() {
            comments = new List<review_comments>();
            sampleComments = new HashSet<review_comments>();
            model = new MinyarModel();
        }

        public int TotalComments {
            get { return totalComments; }
            set {
                totalComments = value;
                RaisePropertyChanged();
            }
        }

        public int Cursor {
            get { return cursor + 1; }
            set {
                cursor = value;
                RaisePropertyChanged();
            }
        }

        public int SampleCount {
            get { return sampleCount; }
            set {
                sampleCount = value;
                RaisePropertyChanged();
            }
        }

        public string Comment {
            get { return comment; }
            set {
                comment = value;
                RaisePropertyChanged();
            }
        }

        public Uri CommentUri {
            get { return uri; }
            set {
                uri = value;
                Clipboard.SetText(value.ToString());
                RaisePropertyChanged();
            }
        }

        public void Next() {
            if (cursor == comments.Count - 1) return;
            Cursor = cursor + 1;
            if (sampleComments.Contains(comments[cursors[cursor]])) Next();
            Refresh();
        }

        public void Prev() {
            if (cursor == 0) return;
            Cursor = cursor - 1;
            Refresh();
        }

        public void AddComment() {
            sampleComments.Add(comments[cursors[cursor]]);
            SampleCount = sampleComments.Count;
        }

        public void RemoveComment() {
            sampleComments.Remove(comments[cursors[cursor]]);
            SampleCount = sampleComments.Count;
        }

        public void LoadComments() {
            var json = new StreamReader(path).ReadToEnd();
            var savedComments = JsonConvert.DeserializeObject<List<int>>(json);
            foreach (var commentId in savedComments) {
                var c = model.review_comments.First(rc => rc.original_id == commentId);
                sampleComments.Add(c);
            }
            SampleCount = savedComments.Count;
            comments = model.review_comments.Where(
                rc => rc.repository_id == 9342529 && rc.for_diff == 1).ToList();
            TotalComments = comments.Count;
            cursors = Enumerable.Range(0, comments.Count).Shuffle().ToList();
            Refresh();
        }

        public void Save() {
            var comment_ids = sampleComments.Select(c => c.original_id);
            using (var writer = new StreamWriter(path)) {
                writer.Write(JsonConvert.SerializeObject(comment_ids));
            }
        }

        private void Refresh() {
            var c = comments[cursors[cursor]];
            Comment = c.body;
            CommentUri = new Uri(c.html_url);
        }

        void IDisposable.Dispose() {
            model.Dispose();
        }
    }
}
