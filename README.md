<div align="center">
<h1>WinAiAgent</h1>

**WinAiAgent is a powerful automation agent that interact directly with the Windows at GUI layer. It bridges the gap between AI Agents and the Windows OS to perform tasks such as opening apps, clicking buttons, typing, executing shell commands, and capturing UI state all without relying on traditional computer vision models. Enabling any LLM to perform computer automation instead of relying on specific models for it.**
</div>


Prerequisites:
 Python 3.12 or higher
 Windows 7 or 8 or 10 or 11

Run

* Backend:

  * Create and activate a Python venv 
  ```bash
  pip install -r backend/requirements.txt
   ```
  * Set GOOGLE\_API\_KEY in backend/.env 
  ```bash
  python backend/main.py
  ```

* Frontend (WPF):

  * dotnet build
  * dotnet run

To Use:

  * &nbsp;-Ctrl+space \[Hide/show]
  * &nbsp;-Ctrl+q     \[Close the app]


---

## 🎥 Demos

**PROMPT:** Write a short note about LLMs and save to the desktop

<https://github.com/user-attachments/assets/46b64c4c-f4b2-4a4d-a2f9-7b33b74a7f62>

## 📈 Grounding

![Image](https://github.com/user-attachments/assets/e1d32725-e28a-4821-9c89-24b5ba2e583f)
![Image](https://github.com/user-attachments/assets/be72ad43-c320-4831-95cf-6f1f30df18de)
![Image](https://github.com/user-attachments/assets/d91b513e-13a0-4451-a6e9-f1e16def36e3)
![Image](https://github.com/user-attachments/assets/b5ef5bcf-0e15-4c87-93fe-0f9a983536e5)
![Image](https://github.com/user-attachments/assets/2b5cada6-4ca1-4e0c-8a10-2df29911b1cb)

## Vision

Talk to your computer. Watch it get things done.

## ⚠️ Caution

Agent interacts directly with your Windows OS at GUI layer to perform actions. While the agent is designed to act intelligently and safely, it can make mistakes that might bring undesired system behaviour or cause unintended changes. Try to run the agent in a sandbox envirnoment.

## 🪪 License

This project is licensed under the MIT License - see the [LICENSE] file for details.

---








