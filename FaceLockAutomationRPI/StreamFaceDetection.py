from imutils.video import VideoStream
import datetime
import numpy as np
import imutils
import time
import cv2
import CloudCommunicatorThread as Communicator
import ImageUploadThread as ImageUploader
import LockControlThread as LockController
from threading import Thread
from queue import Queue
import ConfigAll
from abc import ABC, abstractmethod
import traceback


class StreamFaceDetection(ABC):
    def __init__(self):
        self.setUpThread()
        
        self.frame_send_cout = 0
        self.detect_frame_count = 0
        self.isDetectPrevFrame = False
        self.videoStream = VideoStream(src=0, usePiCamera=False, resolution=(640, 480), framerate = 10).start()
        time.sleep(2.0)
        
    def setUpThread(self):
        self.lockQueue = Queue(1)
        self.lockControlThread = LockController.LockControlThread(queue = self.lockQueue)
        self.lockControlThread.start()
        
        self.cloudQueue = Queue(1)
        self.cloudThread = Communicator.CloudCommunicatorThread(queue = self.cloudQueue, lockQueue = self.lockQueue)
        self.cloudThread.start()
        
        self.imageQueue = Queue(10)
        self.uploadImageThread = ImageUploader.ImageUploadThread(queue = self.imageQueue)
        self.uploadImageThread.start()
        
        
    def run(self):
        self.config()
        try:
            while True:
                # grab the frame from the threaded video stream and resize it
                frame = self.videoStream.read()
                self.processFrame(frame)
                
                # show the frame
                cv2.imshow("Frame", frame)
                key = cv2.waitKey(1) & 0xFF

                # if the `q` key was pressed, break from the loop
                if self.frame_send_cout == 100:
                    break
                if key == ord("q"):
                    self.terminateAllThread()
                    break

            # do a bit of cleanup
            cv2.destroyAllWindows()
            self.videoStream.stop()
        except KeyboardInterrupt:
            print("Stream face detection stopped")
            self.terminateAllThread()
        except Exception as ex:
            print("Generic Error on Stream face detection")
            print(ex)
            traceback.print_exc()
            self.terminateAllThread()
        
            
    def terminateAllThread(self):
        self.cloudQueue.put(None)
        self.imageQueue.put(None)
        self.lockQueue.put(None)
        
    def sendFaceDetected(self, frame):
        if self.isDetectPrevFrame == False:
            self.detect_frame_count = 1
            self.isDetectPrevFrame = True
        else:
            self.detect_frame_count += 1
            if self.detect_frame_count >= ConfigAll.DETECT_FRAME_LIMIT:
                self.isDetectPrevFrame = False
                self.frame_send_cout += 1
                print("Frame send " + str(self.frame_send_cout))
                img_str = cv2.imencode(ConfigAll.FILEEXTENSION, frame, [int(cv2.IMWRITE_JPEG_QUALITY), 96])[1].tostring()
                self.imageQueue.put(img_str)
        
    @abstractmethod
    def processFrame(self, frame):
        pass
        
    @abstractmethod
    def config(self):
        pass

    