from imutils.video import VideoStream
import datetime
import numpy as np
import imutils
import time
import cv2
from StreamFaceDetection import StreamFaceDetection
import ConfigAll

class StreamFaceDetectionHaarCascade(StreamFaceDetection):
    def __init__(self):
        super().__init__()
        
    def config(self):
        self.faceCascade = cv2.CascadeClassifier(ConfigAll.CASCPATH)
        
        
    def processFrame(self, frame):
        gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)

        faces = self.faceCascade.detectMultiScale(
            gray,
            scaleFactor=1.1,
            minNeighbors=5,
            minSize=(30, 30)
        )
        
        if len(faces) != 0:
            super().sendFaceDetected(frame)

        # Draw a rectangle around the faces
        for (x, y, w, h) in faces:
            cv2.rectangle(frame, (x, y), (x+w, y+h), (0, 255, 0), 2)

        # draw the timestamp on the frame
        timestamp = datetime.datetime.now()
        ts = timestamp.strftime("%A %d %B %Y %I:%M:%S%p")
        cv2.putText(frame, ts, (10, frame.shape[0] - 10), cv2.FONT_HERSHEY_SIMPLEX,
                0.35, (0, 0, 255), 1)
