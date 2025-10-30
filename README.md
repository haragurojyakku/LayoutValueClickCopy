
# LayoutValueClickCopy

## 概要

**LayoutValueClickCopy** は、解像度やサイズを黄金比や2の倍数に基づいて計算し、それらをクリップボードにコピーできるツールです。

![image](https://github.com/user-attachments/assets/0e35fee7-84d7-45d7-bd01-308fd595c409)

## 使用方法

1. **解像度のコピー**
   - ワイドやハイトなどの下部の数値のボタンをクリックすると、その値がクリップボードにコピーされます。

2. **スペーシングの入力と再計算**
   - 「Spacing (px)」フィールドに間隔を入力し、「Recalculate」ボタンを押すと、その間隔を考慮して解像度が再計算されます。

## 注意

クリップボードの仕様で素早く連続でコピーするとうまくいかない場合があります。
その場合はゆっくり確実にボタンをクリックしてください。

## アーキテクチャ（リファクタリング後）

   - View: `MainWindow.xaml`（各列を ItemsControl + DataTemplate でボタン生成）
   - ViewModel: `ViewModels/MainWindowViewModel.cs`（計算ロジック・状態・コマンド）
   - Command: `Commands/RelayCommand.cs`
   - `SpacingText`: スペーシング入力（文字列）
   - `WidthValues`/`HeightValues`: 黄金比で算出した値
   - `WidthDecrements`/`HeightDecrements`: 前値からの減少分（ボタン表示は `-X`、コピーは `X`）
   - `MultipleOfTwoValues`: 4096 から 1 までの 2 の累乗
   - `RecalculateCommand`/`CopyValueCommand`

挙動は従来どおりで、コードビハインドのロジックをViewModelへ移行し、テストや保守がしやすい構造になりました。

## 新機能: バイナリースケール近似（±10%以内）

- Width/Height の各黄金比値に対して、2の累乗の数（例: 1024, 512, 128, ...）を最大3個まで足し合わせて、元の値の±10%以内に収まる近似値を計算します。
- 例: 1186 → 1024 + 128 = 1152（誤差約2.9%）
- 優先順位: 誤差が小さい → 要素数が少ない → 過大評価より過小評価を優先
- UI: 「Width Approx」「Height Approx」列に「合計値 (分解)」形式で表示（クリックで合計値をコピー）
 - 追加制約: 使用できる2の累乗は、組み合わせ中の「最大の数値」を L としたとき、範囲 [L/4, L*4] のみ（上下2段）。
    - 例: L=32 のとき、使用可: 8,16,32,64,128／使用不可: 4,256 など

## 配布: 単一EXE と インストーラ

- 単一EXE（win-x64, self-contained, 非トリミング）
   - プロファイル: `Properties/PublishProfiles/Win-x64-SingleFile.pubxml`
   - 出力: `publish/win-x64-single/`
   - 実行例（PowerShell）: `scripts/publish-singlefile.ps1 -Release`

- インストーラ（Inno Setup）
   - スクリプト: `installer/LayoutValueClickCopy.iss`
   - 前提: Inno Setup がインストールされていること
   - 手順: 先に単一EXEを発行 → Inno Setup で `.iss` を開いてビルド → `installer/output/` にセットアップEXE生成

注意: WPF の互換性を優先し、トリミングは無効化しています（`PublishTrimmed=false`）。

## 設定の保存

- 保存場所: `%AppData%/LayoutValueClickCopy/settings.json`
- 保存項目: `SpacingText`, `SelectedTabIndex`, `SelectedLanguage`
- 実装: `Models/AppSettings.cs`, `Services/SettingsService.cs`

## アイコン設定

- プロジェクト直下に `Assets/` フォルダを追加済みです。
   - `Assets/app.ico` が存在すれば、ビルド時に EXE アイコンとして自動で埋め込まれます。
   - 実行時には、`Assets/app.ico`（なければ `Assets/app.png`）をウィンドウアイコンに使用します。
- Inno Setup のインストーラでセットアップ EXE のアイコンを合わせたい場合は、`installer/LayoutValueClickCopy.iss` の `SetupIconFile` をコメント解除して `..\Assets\app.ico` を指定してください。
