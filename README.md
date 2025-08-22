WindowsSearchApp

Minimal Windows 11-style floating search overlay (WPF) with voice input and a FastAPI backend.

Run

* Backend:

  * Create and activate a Python venv
  * pip install -r backend/requirements.txt
  * Set GOOGLE\_API\_KEY in backend/.env
  * python backend/main.py

* Frontend (WPF):

  * dotnet build
  * dotnet run

To Use:

  * &nbsp;-Ctrl+space \[Hide/show]
  * &nbsp;-Ctrl+q     \[Close the app]
