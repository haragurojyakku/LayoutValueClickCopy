Place your application icon files here.

- app.ico  (recommended, used for EXE icon and Window icon)
- app.png  (optional fallback for Window icon if .ico is not provided)

Notes:
- When `Assets/app.ico` exists, it will be embedded as the EXE icon automatically (no build errors if missing).
- At runtime, the app tries to use `app.ico`, then `app.png` as the Window icon.
