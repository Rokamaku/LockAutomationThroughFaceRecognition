from StreamFaceDetectionCaffe import StreamFaceDetectionCaffe
from StreamFaceDetectionHaarCascade import StreamFaceDetectionHaarCascade
import sys
import ConfigAll

def main():
    algorithmType = sys.argv[1]
    if algorithmType == ConfigAll.Caffe:
        stream = StreamFaceDetectionCaffe()
    elif algorithmType == ConfigAll.Haar:
        stream = StreamFaceDetectionHaarCascade()
    stream.run()
    
    
if __name__ == "__main__":
    main()