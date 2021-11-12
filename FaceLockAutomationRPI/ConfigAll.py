from iothub_client import IoTHubTransportProvider

#Face Dection library
Haar = "Haar"
Caffe =  "Caffe"

#Azure IoT Hub
IOT_HUB_CONNECTION_STRING = "HostName=para-iothub.azure-devices.net;DeviceId=para-rpi;SharedAccessKey=SXQLijpeapUxKY44ScZeP+8m8p7Ev7hz9XZ5Is2WA0Q="
CLOUD_COMMUNICATE_PROTOCOL = IoTHubTransportProvider.MQTT

TIMEOUT = 241000
MINIMUM_POLLING_TIME = 9


#Azure Storage 
FILENAME = "Detect_Face"
FILEEXTENSION = '.jpg'

ACCOUNTNAME = "facelogstorage"
ACCOUNTKEY = "tF9DPfyQQB8nh7zkJrENCuGW0XLzASdKtLQGUVdl3BRHXzOEy72wEMoN/lXpQeMKtiJMIJT2x/euIngEsME6jw=="
STORAGE_CONNECTION_STRING = "DefaultEndpointsProtocol=https;AccountName=facelogstorage;AccountKey=tF9DPfyQQB8nh7zkJrENCuGW0XLzASdKtLQGUVdl3BRHXzOEy72wEMoN/lXpQeMKtiJMIJT2x/euIngEsME6jw==;EndpointSuffix=core.windows.net" 
CONTAINERNAME= "face-rpi"

#ControlFormat
ISON = "isOn"
TIME = "time"

#Face Detection
DETECT_FRAME_LIMIT = 3

#Caffe
MINCONF = 0.5
PROTOTXT = "deploy.prototxt.txt"
MODEL = "res10_300x300_ssd_iter_140000.caffemodel"


#HaarCascade
CASCPATH = "/usr/local/share/OpenCV/haarcascades/haarcascade_frontalface_default.xml"

#GPIO Pin
LockPin = 18
ServoPin = 17
PWMServoPin = 19