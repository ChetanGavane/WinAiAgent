<div align="center">
<h1>WinAiAgent</h1>

**WinAiAgent is a powerful automation agent that interact directly with the Windows at GUI layer. It bridges the gap between AI Agents and the Windows OS to perform tasks such as opening apps, clicking buttons, typing, executing shell commands, and capturing UI state all without relying on traditional computer vision models. Enabling any LLM to perform computer automation instead of relying on specific models for it.**
</div>
Prerequisites:
 Python 3.12 or higher
 Windows 7 or 8 or 10 or 11

Run

* Backend:

  Create and activate a Python venv 
  ```bash pip install -r backend/requirements.txt ```
  * Set GOOGLE\_API\_KEY in backend/.env 
  ```bash python backend/main.py ```

* Frontend (WPF):

  * dotnet build
  * dotnet run

To Use:

  * &nbsp;-Ctrl+space \[Hide/show]
  * &nbsp;-Ctrl+q     \[Close the app]



