# MornInput

## 概要

Unity Input Systemのラッパーで、複数プレイヤー入力管理を統一化するライブラリ。

## 依存関係

| 種別 | 名前 |
|------|------|
| 外部パッケージ | Unity Input System, UniRx |
| Mornライブラリ | MornLib |

## 使い方

### 入力の取得

```csharp
// MornInputProviderでPlayerInputを管理
var handler = MornInputProvider.GetHandler(playerIndex);

// アクション名で入力を取得
bool isPressed = handler.IsPressed("Jump");
bool isPerformed = handler.IsPerformed("Attack");
Vector2 move = handler.ReadValue<Vector2>("Move");
```

### スキーム切り替え検知

```csharp
// キーボード/ゲームパッドの切り替えを検知
handler.OnSchemeChanged.Subscribe(scheme => {
    Debug.Log($"スキーム変更: {scheme}");
});
```
