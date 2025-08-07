## Install 
First clone the repo, and then install environment:
```python
conda create -n "omni" python==3.12
conda activate omni
pip install -r requirements.txt
```

Ensure you have the V2 weights downloaded in weights folder (ensure caption weights folder is called icon_caption_florence). If not download them with:
```
   # download the model checkpoints to local directory OmniParser/weights/
   for f in icon_detect/{train_args.yaml,model.pt,model.yaml} icon_caption/{config.json,generation_config.json,model.safetensors}; do huggingface-cli download microsoft/OmniParser-v2.0 "$f" --local-dir weights; done
   mv weights/icon_caption weights/icon_caption_florence
```

<!-- ## [deprecated]
Then download the model ckpts files in: https://huggingface.co/microsoft/OmniParser, and put them under weights/, default folder structure is: weights/icon_detect, weights/icon_caption_florence, weights/icon_caption_blip2. 

For v1: 
convert the safetensor to .pt file. 
```python
python weights/convert_safetensor_to_pt.py

For v1.5: 
download 'model_v1_5.pt' from https://huggingface.co/microsoft/OmniParser/tree/main/icon_detect_v1_5, make a new dir: weights/icon_detect_v1_5, and put it inside the folder. No weight conversion is needed. 
``` -->

## Examples:
We put together a few simple examples in the demo.ipynb. 

## Gradio Demo
To run gradio demo, simply run:
```python
python gradio_demo.py
```

