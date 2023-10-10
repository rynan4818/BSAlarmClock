# BSAlarmClock
このBeatSaberプラグインはタイマー・アラーム付きの時計です。

## 特徴
* 時計表示は好きな位置に掴んで配置できます。
* メニューとゲームプレイ画面で別々のサイズや位置、表示・非表示が設定できます。
* アラーム音が自由に追加できます。
* ゲームプレイ中はアラームを鳴らさないようにできます。

# インストール方法
1. [リリースページ](https://github.com/rynan4818/BSAlarmClock/releases)から最新のBSAlarmClockのリリースをダウンロードします。
2. ダウンロードしたzipファイルを`Beat Saber`フォルダに解凍して、`Plugin`フォルダに`BSAlarmClock.dll`ファイルをコピーします。
3. [CameraUtils](https://github.com/Reezonate/CameraUtils)に依存するため、`CameraUtils.dll`をダウンロードして`Plugin`フォルダにコピーしてください。

    ModAssistantのLibrariesに登録がある場合はそちらからインストールするのが簡単です。
    ![image](https://user-images.githubusercontent.com/14249877/222885321-0d0a2b5b-ccaf-4868-86e9-8d77ca375d38.png)
    v1.0.6はBS1.30.2以降用なので、1.29.1の人はModAssistantのLibrariesに登録がありますので、そちらからインストールして下さい(BeatSaber 1.26.0以上)

# 使い方
左のMODSタブにBS ALARM CLOCKが追加されます。


* Alarm `ON`/`OFF`ボタンでアラームを鳴らすか設定します。アラームONの状態では時計の下にタイマー表示が出ます。
* `Alarm H・M` でを鳴らす時刻を設定します。
* `SET TIMER TO ALARM`ボタンで、下のTimerの時間でAlarmを設定します。
* `Timer H・M` でタイマー時間を設定します。
* `Alarm Sound Enabled`で、アラーム音を鳴らすか設定します。
* `Alarm Sound` で鳴らす音を選択します。

    `Alarm1.wav`と`Alarm2.wav`はデフォルトで入っています。他のアラーム音を追加したい場合は`UserData\BSAlarmClockSound`フォルダにwavファイルを入れて下さい。なおアラームの駆動は1秒単位になります。
* `TEST`ボタンでアラーム音のテストをします。
* `Alarm Sound Volume`はアラーム音の音量です。
* `Alarm Sound Menu Only`を有効にすると、ゲームプレイ中はアラーム音が鳴らなくなります。(アラームを止めなければ、メニューに戻ったときにアラーム音が鳴ります)
* `Menu/Game Scene Lock Position`を有効にすると、移動用のハンドル表示が消えて位置が固定されます。(メニューとゲームシーン別々に設定があります。)
* `Menu/Game Scene Screen Hidden`を有効にすると、時計表示が消えます。アラーム音が有効な場合はアラーム音は鳴ります。
* `Menu/Game Scene HMD Only`を有効にすると、HMDのみ表示してデスクトップ画面では表示が消えます。
* `Menu/Game Scene Screen Size`は時計表示のサイズです。
* `RESET MENU/GAME SCENE POSITION`を押すと、位置が初期値に戻ります。
* `Screen Size X/Y`はスクリーンの基本サイズです。
* `Time Font Size`は時計表示文字サイズです。
* `Timer Font Size`はタイマー表示文字サイズです。
* `Alarm Stop Button Size`はアラームストップボタンのサイズです。


