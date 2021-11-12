import sys
import threading
import ConfigAll
import os
import time
from queue import Queue
from azure.storage.blob import blockblobservice, models


class ImageUploadThread(threading.Thread):
    def __init__(self, queue, args=(), kwargs=None):
        threading.Thread.__init__(self, args=(), kwargs=None)
        self.daemon = True
        self.queue = queue
        self.iot_hub_init()
        
    def iot_hub_init(self):
        self.blob_service = blockblobservice.BlockBlobService(account_name = ConfigAll.ACCOUNTNAME,
                                             account_key = ConfigAll.ACCOUNTKEY,
                                             connection_string = ConfigAll.STORAGE_CONNECTION_STRING)
        
        
    def run(self):
        while True:
            img_str = self.queue.get()
            if img_str is None:
                print("Image uploader terminated")
                break
            self.upload_blob_run(img_str)
            
        
    def upload_blob_run(self, image):
        try:
            currentTime = str(time.time())
            self.blob_service.create_blob_from_text(ConfigAll.CONTAINERNAME,
                                                    ConfigAll.FILENAME + '_' + currentTime + ConfigAll.FILEEXTENSION,
                                                    content_settings = models.ContentSettings(content_type='image/jpeg'),
                                                    progress_callback = self.blob_upload_conf_callback,
                                                    text = image)

            print ( "" )
            print ( "File upload initiated..." )

        except KeyboardInterrupt:
            print ( "IoTHubClient stopped" )
##        except:
##            print ( "Generic error on upload image" )
            
            
    def blob_upload_conf_callback(self, current, total):
        if current == total:
            print ( "...file uploaded successfully." )

