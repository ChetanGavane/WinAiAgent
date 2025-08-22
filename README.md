WindowsSearchApp

Minimal Windows 11-style floating search overlay (WPF) with voice input and a FastAPI backend.

Run
- Backend:
  - Create and activate a Python venv
  - pip install -r backend/requirements.txt
  - Set GOOGLE_API_KEY in backend/.env
  - python backend/main.py
- Frontend (WPF):
  - dotnet build
  - dotnet run

Hotkey: Ctrl+Space toggles the overlay. Press Enter to send. Esc to hide.




