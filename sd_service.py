from json import loads
from PIL import Image
from requests	import post
from base64		import b64decode, b64encode
from os.path	import isfile
from os			import remove
from time		import sleep
## Config
URL 			= "http://127.0.0.1:7860/sdapi/v1/img2img"
DATA_PATH		= "data.txt"		# The prompt file: Script looks for this then does stuff
IN_IMAGE_PATH	= "process_me.png"	# An image file for the prompt file pixel definitions
OUT_IMAGE_PATH	= "output.png"		# The generated file
##
# Go!
##
## Load prompt data
while True:
	## Kill some time between listens
	sleep(2)
	## If a prompt file is found
	if not isfile(DATA_PATH):
		continue
	## Try to parse and delete model file. If it doesn't work, don't worry but skip this loop.
	try:
		with open("data.txt") as file:
			json = loads(file.read())
		remove(DATA_PATH)
	except Exception as e:
		print(e)
		continue
	## Define prompt stuff
	prompt		= json["prompt"] if "prompt" in json else "Sky diver"
	negative	= json["negative_prompt"] if "negative_prompt" in json else ""
	steps		= json["steps"] if "steps" in json else 5
	strength	= json["strength"] if "strength" in json else 5
	cfg			= json["cfg"] if "cfg" in json else 7.5
	pixels		= json["pixels"] 
	## Make the image from the pdn output
	image	= Image.new("RGBA", (len(pixels), len(pixels[0])))
	# pixels is a 2D array of RGBa int arrays that represent the current paint.net layer 
	for x in range(len(pixels)):
		for y in range(len(pixels[0])):
			image.putpixel((x, y), tuple(pixels[x][y]))
	# Net says Pillow is lazy with image data, so it needs to be read from the file system
	image.save(IN_IMAGE_PATH)
	with open(IN_IMAGE_PATH, "rb") as file:
		inImage	= file.read()
	## Do the request
	payload	= {
		"width":				len(pixels),
		"height":				len(pixels[0]),
		"cfg_scale":			cfg,
		"prompt": 				prompt,
		"negative_prompt":		negative,
		"steps":				steps,
		"denoising_strength":	strength,
		"init_images": 			[b64encode(inImage).decode('utf-8')]
	}
	# Print the config in the console
	print("---")
	print("Prompt:   %s" %(prompt))
	print("Width:    %s" %(len(pixels)))
	print("Height:   %s" %(len(pixels[0])))
	print("CFG:      %s" %(cfg))	
	print("Strength: %s" %(strength))	
	print("Steps:    %s" %(steps))	
	response = post(url=URL, json=payload)
	## Write image from SD to output file
	with open(OUT_IMAGE_PATH, "wb") as file:
		file.write(b64decode(response.json()["images"][0]))
	image = Image.open(OUT_IMAGE_PATH)
	## Open the image for me, please.
	image.show()
	






with open(IN_IMAGE_PATH, "rb") as file:
	inImage	= file.read()

payload	= {
	"prompt": prompt,
	"steps": steps,
	"init_images": [b64encode(inImage).decode('utf-8')]
}
response = post(url=URL, json=payload)
with open(OUT_IMAGE_PATH, "wb") as file:
	file.write(b64decode(response.json()["images"][0]))

image = Image.open(OUT_IMAGE_PATH)
image.show()


exit()
