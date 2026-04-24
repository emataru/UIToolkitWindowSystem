# UIToolkit Window System

Unity の UI Toolkit 上で、アプリケーション風の「ウィンドウ」「モーダルダイアログ」「メニューバー」「テーマ切替」を扱うための軽量なウィンドウ管理ユーティリティです。

## 特徴

- **ウィンドウ管理**
  - 通常ウィンドウとモーダルウィンドウのオープン/クローズ/フォーカス
  - `Esc` によるクローズ（ウィンドウ設定で有効化）
  - オープン時のセンタリング（`CenterOnOpen`）
- **ダイアログ**
  - `MessageBox` / `ConfirmDialog`（`WindowService` 経由）
  - `Enter` キーによるサブミット（ダイアログ側で対応）
- **メニューバー**
  - `MenuBuilder` によるメニュー/サブメニュー/チェック項目の構築
  - メニュー外クリック・`Esc` でメニューを閉じる
- **テーマ切替**
  - `WindowThemeController` によるライト/ダークなどのスタイルシート切替

## 依存関係

- Unity の **UI Toolkit**（ランタイム UI）
- **UniTask**（`Cysharp.Threading.Tasks`）

## クイックスタート

### 1. `WindowSystemHost` を配置

1. シーン内に GameObject を作成し、`WindowSystemHost` を追加します。
2. `UIDocument` を割り当てます（`UIDocument.rootVisualElement` がルートになります）。
3. `CommonWindowViewAssets` を割り当てます。
   - `WindowFrameUxml`
   - `WindowFrameStyleSheet`
   - `MessageBoxContentUxml`
   - `ConfirmDialogContentUxml`

`WindowSystemHost` は初期化時に以下を生成・接続します。

- `WindowLayerRoot`（通常/モーダル/ブロッカーの各レイヤー）
- `WindowManager`
- `WindowContext`
- `WindowService`

### 2. スタイルシートを適用

`WindowFrameStyleSheet` は `WindowSystemHost` がルートに自動で追加します。追加の見た目（テーマやメニューバーなど）は、必要に応じて `UIDocument` のルートに `uss` を追加してください。

### 3. テーマ切替

ライト/ダーク等の切替を使う場合は、`WindowThemeController` を配置し、`UIDocument` と `WindowThemeAssets` を割り当てます。

## 使い方

### UPM Package
https://github.com/emataru/UIToolkitWindowSystem.git?path=Assets/UIToolkit%20Window%20System

### ウィンドウを開く

- `WindowManager.Open(window)` で通常ウィンドウを開きます。
- `WindowManager.OpenModal(window)` でモーダルを開きます。

ウィンドウは `WindowBase` を継承して実装します。

// 例: WindowBase を継承したカスタムウィンドウ
public sealed class MyToolWindow : WindowBase
{
    public MyToolWindow(WindowContext context)
        : base(new WindowOptions
        {
            Title = "My Tool",
            Width = 480,
            Height = 320,
            Closable = true,
            Draggable = true,
            Resizable = true,
            CenterOnOpen = true,
        }, context.CommonViews.WindowFrameUxml)
    {
        // ContentRoot に UXML を追加するなど
    }
}

### ダイアログを表示する

`WindowService` を使うと `MessageBox` / `ConfirmDialog` を非同期で表示できます。

// 例: Message
await context.WindowService.ShowMessageAsync("Hello", "This is a message box.");

// 例: Confirm
var result = await context.WindowService.ShowConfirmAsync("Confirm", "Continue?");

### メニューバーを構築する

`MenuBuilder` を `rootVisualElement` に対して使用します。

var menu = new MenuBuilder(root);
menu.BeginMenuBar();

if (menu.BeginMenu("File"))
{
    menu.MenuItem("Open", OnOpen);
    menu.MenuItem("Exit", OnExit);
    menu.EndMenu();
}

menu.EndMenuBar();

## サンプル

- `SampleMenuController` : メニューバーからツール/ダイアログ/テーマを操作
- `SampleToolWindowController` / `SampleToolWindow` : ウィンドウ表示と UI 要素のバインド例
- `DialogController` : `WindowService` 経由でのメッセージ/確認ダイアログ呼び出し例

## カスタマイズ指針

- 見た目は `uss` を差し替え（テーマ）または追加（独自ウィンドウ）で調整します。
- 入力（`Esc` クローズ等）は `WindowOptions` と `WindowManager` の責務です。
- ダイアログの種類を増やしたい場合は、`DialogWindow<TResult>` を継承して実装し、`WindowService` に追加します。

## ライセンス

リポジトリの `LICENSE` を参照してください。

