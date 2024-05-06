public string PROMPT_FILE_PATH = "c:/workspaces/__sandbox__/codelab/data.txt";
#region UICode
TextboxControl Prompt          = ""; // [0,100] Prompt
TextboxControl Negative        = ""; // [0,100] Negative prompt
IntSliderControl Steps         = 5; // [0,100] Steps
DoubleSliderControl CFG        = 7.5; // [0,30] CFG Scale
DoubleSliderControl Strength   = 0.5; // [0,1] Denoising strength
#endregion
/*
    READ ME!

    tl;dr: Lots of hacks to get round CodeLab limitations.

    $ = End Prompt with "$" to tell to send the request. CodeLab entry points are events and it was easier
        just to do this.

    - Didn't have access to System.Draw so we create RGBa arrays for each canvas pixel in the prompt file
    - ... and we convert them to an image in sd_service.py. 
    - For some reason, changing interface values changes the current layer. Only applies if you press Ok.
    - Try/Catch: Kind of lazy. CodeLab doesn't appear to have System.IO for checking file accessibility
      and OnRender is an event, just fail quietly and try again.
*/
protected override void OnRender(IBitmapEffectOutput output)
{
    /*
        Only send request IF Prompt ends with $. Lazy fix for having event-triggered plugin calls
    */
    if(Prompt[Prompt.Length - 1] != '$'){
        return;
    }
    // Remove the $
    Prompt      = Prompt.Replace("$", "");

    /*
        Need these constant.
    */
    int WIDTH   = Environment.Document.Size.Width;
    int HEIGHT  = Environment.Document.Size.Height;
    /*
        Honestly, for just proving it works, try/catch is fine.
    */
    try{
        /*
            The magic. It's read-only but somehow it edits the active layer
        */
        using IEffectInputBitmap<ColorBgra32> sourceBitmap = Environment.Document.GetBitmapBgra32();
        using IBitmapLock<ColorBgra32> sourceLock = Environment.Document.GetBitmapBgra32().Lock(new RectInt32(0, 0, sourceBitmap.Size));
        RegionPtr<ColorBgra32> sourceRegion = sourceLock.AsRegionPtr();
        /*
            Generate the prompt JSON pixels data. [[[r, g, b, a]]] as a string.
        */
        // Lots of concatenation, it's mega slow without this - and StringBuilder isn't a thing
        StringWriter stringWriter   = new StringWriter();    
        // Open file for writing
        using(StreamWriter writer = new StreamWriter(PROMPT_FILE_PATH)){ 
            // For X
            for (int cellID = 0; cellID < Environment.Document.Size.Width; cellID++){
                stringWriter.Write("[");
                // For Y
                for (int rowID = 0; rowID < Environment.Document.Size.Height; rowID++){
                    // Create array of RGBA 255 values for the current pixel
                    string temp = "[" + 
                    sourceRegion[cellID, rowID].R.ToString() + "," +
                    sourceRegion[cellID, rowID].G.ToString() + "," +
                    sourceRegion[cellID, rowID].B.ToString() + "," +
                    "255]";
                    // Add a comma if this isn't the last pixel in the row
                    if(HEIGHT - 1 != rowID){
                        temp += ",";
                    }
                    // Cache the line for later
                    stringWriter.Write(temp);
                }
                // Add comma if this isn't the last row
                if(WIDTH- 1 != cellID){
                    stringWriter.Write("],");
                }else{
                    stringWriter.Write("]");
                }
            }
            // JSON string
            string json = "";
            json += "\"negative_prompt\":\"" + Negative + "\",";
            json += "\"prompt\":\"" + Prompt + "\",";
            json += "\"steps\":" + Steps.ToString()  + ",";
            json += "\"cfg\":" + CFG + ",";
            json += "\"strength\":" + Strength.ToString() + ",";
            json += "\"pixels\":[" + stringWriter.ToString() + "]";
            json    = "{" + json + "}";
            // Write file
            writer.Write(json);
        }
    }catch{}
}