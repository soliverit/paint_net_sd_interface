# Stable diffusion for Paint.net
A dirty Stable diffusion plugin for Paint.net using the CodeLab development tools.

### Define dirty?
Was just a weekend project. It's written in CodeLab which has limited .Net access. Loads of hack and slash to make it work without access to System.*.

## Getting started

### Setup

- Python 3.x with `pillow` (`pip install pillow`)
- Stable diffusion web-UI (https://github.com/AUTOMATIC1111/stable-diffusion-webui)
- CodeLabs for Paint.net (https://www.boltbait.com/pdn/CodeLab/)
- From the web-UI folder, run `webui.py --api`
- From this project's root folder run `python sd_service.py`

### Use (script)

- Open CodeLabs from the Effects->Advanced menu
- Open `sd_service.cs`
- `Ctrl + p` to start the plugin
- Add `$` to Prompt end to send request (see comments on CodeLabs and events triggering plugins)

### Use (.dll)

-  Open CodeLabs from the Effects->Advanced menu
-  Select File->Build DLL and build
-  Close Paint.net
-  Extract the new .zip folder and run the `<install_plugin_name>.bat`
-  Open Paint.net. Select option for assigned menu. E.g, Effects->sd_canvas
