from fastapi import FastAPI
from pydantic import BaseModel
from langchain_google_genai import ChatGoogleGenerativeAI
from windows_use.agent import Agent
from dotenv import load_dotenv
from pathlib import Path
import os
import uvicorn
import logging
import traceback

# Load .env alongside this file
load_dotenv(dotenv_path=Path(__file__).parent / ".env")

logging.basicConfig(level=logging.INFO, format='%(asctime)s %(levelname)s %(message)s')
logger = logging.getLogger("windowssearchapp")

app = FastAPI()


class CommandRequest(BaseModel):
    command: str


_model_preferences = (
    os.getenv("GOOGLE_MODEL"),
    "gemini-2.0-flash",
)


def _init_llm_with_fallback():
    last_error: Exception | None = None
    for name in [m for m in _model_preferences if m]:
        try:
            logger.info("Trying model: %s", name)
            return ChatGoogleGenerativeAI(model=name)
        except Exception as e:  # keep trying
            last_error = e
            logger.warning("Model init failed for %s: %s", name, e)
    raise last_error or RuntimeError("No model could be initialized. Set GOOGLE_MODEL.")


def _ensure_agent():
    global llm, agent
    if "llm" not in globals() or llm is None:
        llm = _init_llm_with_fallback()
    if "agent" not in globals() or agent is None:
        agent = Agent(llm=llm, browser="chrome", use_vision=True)


@app.get("/health")
async def health():
    return {"status": "ok", "google_api_key_present": bool(os.getenv("GOOGLE_API_KEY"))}


@app.post("/execute-command")
async def execute_command(request: CommandRequest):
    try:
        logger.info("Incoming command: %s", request.command)
        _ensure_agent()
        agent_result = agent.invoke(query=request.command)
        content = getattr(agent_result, "content", str(agent_result))
        logger.info("Agent result: %s", content)
        return {"status": "success", "result": content}
    except Exception as e:
        logger.error("Agent error: %s\n%s", e, traceback.format_exc())
        # Fallback: echo the command so the UI has feedback while debugging
        return {"status": "error", "message": str(e), "echo": request.command}


if __name__ == "__main__":
    uvicorn.run(app, host="127.0.0.1", port=8000)


