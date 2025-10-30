#define MyAppName "LayoutValueClickCopy"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "haragurojyakku"
#define MyAppExeName "LayoutValueClickCopy.exe"

; 単一EXEの出力先（PublishProfilesの出力と合わせる）
#define PublishDir "..\publish\win-x64-single"

[Setup]
AppId={{A2B2A1D8-6B4B-4A7E-9A2F-2F2D9DE1C1A3}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=output
OutputBaseFilename=LayoutValueClickCopy-Setup
Compression=lzma
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64
; Setup アイコンをプロジェクトの app.ico にしたい場合は、以下のコメントを外してパスを調整してください
; SetupIconFile=..\Assets\app.ico

[Files]
Source: "{#PublishDir}\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Tasks]
Name: desktopicon; Description: "デスクトップにショートカットを作成"; GroupDescription: "追加のタスク:"; Flags: unchecked

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{#MyAppName} を起動"; Flags: nowait postinstall skipifsilent
