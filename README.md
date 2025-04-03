# 🎮 Dimension Core

Unityで制作した2D縦画面のゲームプロジェクトです。  
C#とUnityを用いて、UI制御やアニメーション演出など、実践的な機能を実装しています。

---

## 📌 概要

このプロジェクトでは以下のような機能を実装しています：

- 🎞️ DOTweenによるUIアニメーション・演出
- 🔁 UniRxを使ったリアクティブなデータバインディング
- 🎬 シーン遷移とイベント駆動の実装

> ※ 外部ライブラリはGitHubに含まれていません。導入方法は下記をご確認ください。

---

## 💾 ダウンロード（ビルド済みアプリ）

下記より、ビルド済みのゲームをダウンロードしてすぐに遊べます。  
**Unity環境がなくてもプレイ可能です。**

- 🪟 [Windows版（Google Drive）](https://drive.google.com/drive/folders/1wuDAlWIzz1fvZcequPqCIFtsmaBXiLnX?usp=drive_link)  
- 🍎 [macOS版（Google Drive）](https://drive.google.com/file/d/1_gm1yyi2riMLDa-9etspMhWmh6sGlwdj/view?usp=drive_link)
- macでの実行の際はターミナルで
- 1 「　xattr -rd com.apple.quarantine [ここにAppをドラッグしてEnter]　」
- 2  「　chmod -R 755 ~/Downloads/Dimension\ Core\ For\ Apple\ Silicon.app
　」 を実行する必要があります。


> ※ `.zip` ファイルを解凍後、`.exe` または `.app` を実行してください。  
> Windowsではセキュリティ警告が出る場合がありますが、問題ありません。

---

## 🛠️ 動作環境

- Unity バージョン：**2022.3.x** 推奨  
- プラットフォーム：Windows / macOS  
- 開発言語：C#

---

## 🧩 使用ライブラリ（別途導入が必要）

| ライブラリ名 | 説明 | 導入方法 |
|--------------|------|----------|
| [DOTween](http://dotween.demigiant.com/) | アニメーション制御ライブラリ | Asset Store または [公式サイト](http://dotween.demigiant.com/) |
| [UniRx](https://github.com/neuecc/UniRx) | Unity向けリアクティブ拡張 | [GitHub](https://github.com/neuecc/UniRx) または UnityPackage 手動導入 |

---

## 🚀 セットアップ手順

1. このリポジトリをクローン or ZIPでダウンロード  
2. Unity Hubでプロジェクトを開く  
3. 必要なライブラリ（UniRx / DOTween）をインポート  
4. `Assets/Scenes/Title` を開いて実行！

---

## 🎓 学んだこと・技術的ポイント

- Unityにおけるスクリプト設計（`MonoBehaviour`, イベント管理）
- UniRxによるデータ駆動型UI（`ReactiveProperty`, `Subscribe`）
- DOTweenによるアニメーション演出（`Sequence`, `Ease` など）
- Git / GitHubによるプロジェクト管理

---

## 📃 ポートフォリオについて

- 📦 GitHubリポジトリ（このページ）  
- 📄 [PDF資料（ポートフォリオ）](このリポジトリ内に添付「Dimension Core PDF資料」)  
- 🎥 [ゲーム紹介映像（YouTube）](https://youtu.be/Y6EHVeLFPqE)
- 🎮 ビルド済みアプリ(このREADME上部)

---

## 🙋‍♀️ 作者について

文系学部出身。UnityとC#を独学で学び、約1年にわたって制作に取り組んでいます。  
現在、ゲーム会社のプログラマー職を志望して新卒就職活動中です。

---
