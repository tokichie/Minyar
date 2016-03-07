# Minyar:cat:
Github mining tools :cat2:

## Preparing for Test
This program needs Standard NLP model data.
If required files are not found, test will download and extract data automatically.

## credentials
- Minyar/Resources/AuthInfo.txt（GitHubAPIのアクセスTokenを1行だけ書く）
- Minyar/Resources/credentials.json（DB用のアクセス情報を書く）
  - {“Database”: {“Server”: “localhost”, “DatabaseName”: ~~, “UserId”: ~~, “Password”: ~~}}

## DB構築手順
- プログラムを走らせcrawlコマンドを叩く
- 条件に合うリポジトリと、リポジトリに紐付いたレビューコメントをGitHubから取得 (CommentCrawler#Search)
- PRのクロールなどもCommentCrawler#SearchPullsやCommentCrawler#CrawlFilesAsyncあたりにある
	
## 実験
- プログラムを走らせdbコマンドを叩くとDBのデータを用いてASTのDiffを作成し、変更あり／なしの判定をしてそれぞれ<timestamp>-changed.txtと<timestamp>-unchanged.txtを出力。txtファイルはAstChangeクラスをシリアライズしたものが1行ずつ出てくる。
- 出てきたtxtファイルを必要であれば結合 (ItTreeMinerTest#ConcatData)、重複データを排除 (ItTreeMinerTest#RemoveDuplicatedLines)
- ここまでのデータを特徴作成のマイニング用と、トレーニング用に分ける (ItTreeMinerTest#ShuffleAndTake)
- プログラムを走らせminingコマンドを叩くと、頻出パターンを特定しjsonとして出力。
- 出てきた頻出パターンは特徴ベクトルにするには多すぎ＆似たものが多く含まれるので、ItTreeMinerTest#CalcPatternsSimを使って特徴抽出に用いる頻出パターンを絞る
- ↑で作成した特徴ベクトル用頻出パターンデータを用いてClassifierTest#ClassifyTestを実行すると分類結果がtxtファイルで出てくる
